using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    public class DynamicPropriety
    {
        public long id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsMain { get; internal set; }
        public bool IsUpdatable { get; internal set; }
        public string description { get; internal set; }
        public string displayName { get; internal set; }
        public int ind { get; internal set; }
        public List<DynamicValidator> Validators { get; internal set; }
        public ShowTypes ShowType { get; internal set; }
        public DynamicMapperGenerator MapperGenerator { get; internal set; }
        public Dictionary<long, bool> roles { get; internal set; }
        public static readonly long AnonymousRoleID = 0;
        internal static readonly Query getRoles = Query.fromQueryString(QueryTypes.CBO, "SELECT id, canModify FROM PermissionProprieties INNER JOIN Roles ON id = id_role WHERE id_propriety = @ProprietyID");
        internal static readonly Query getValidators = Query.fromQueryString(QueryTypes.SELECT, "SELECT value AS Value, id_ValidatorType AS ValidatorTypeID, message FROM ValidatorProprietyValues WHERE id_Propriety = @ProprietyID", true);
        internal static readonly Query getMapperGenerator = Query.fromQueryString(QueryTypes.SELECT, "SELECT lnk.id AS id, c.id AS controllerID, c.Name AS controllerName, urlR.id AS routeID, SQLString AS QueryString, id_queryType AS QueryTypeID, completeCheck AS CompleteCheck, completeAuth AS CompleteAuth, p.name AS ProprietyName FROM LinkProprietiesControllers lnk INNER JOIN Controllers c ON c.id = lnk.id_controller INNER JOIN URLRoutes urlR ON urlR.id_controller = c.id INNER JOIN RouteQueries rq ON rq.id_route = urlR.id INNER JOIN Proprieties p ON p.id = lnk.id_propriety WHERE urlR.id_baseRoute = @BaseRouteID AND rq.ind = 1 AND lnk.id_propriety = @ProprietyID", true);
        internal static readonly Query insertPropriety = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Proprieties (name, isMain, isUpdatable, id_ShowType, id_controller, ind, displayName, description) VALUES (@Name, @IsMain, @IsUpdatable, @ShowTypeID, @ControllerID, @ind, @displayName, @description)", true);
        internal static readonly Query insertRole = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO PermissionProprieties (id_propriety, id_role, canModify) VALUES (@ProprietyID, @RoleID, @CanModify)", true);
        internal DynamicPropriety(long id, string Name, bool IsMain, bool IsUpdatable, long ShowTypeID, int ind, string description, string displayName)
        {
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
            this.IsUpdatable = IsUpdatable;
            this.ShowType = (ShowTypes)ShowTypeID;
            this.Validators = new List<DynamicValidator>();
            this.MapperGenerator = null;
            this.roles = new Dictionary<long, bool>();
            this.ind = ind;
            this.description = description;
            this.displayName = displayName;
        }
        internal static async Task<DynamicPropriety> init(DynamicPropriety propriety)
        {
            propriety.Validators = (
                await DynamicController.executor.SelectQuery<DynamicValidator>(
                    getValidators
                        .setParam("ProprietyID", propriety.id)
                    )
                ).ToList();
            if (propriety.ShowType.IsRef() || propriety.ShowType.IsCBO())
            {
                propriety.MapperGenerator =
                    await DynamicController.executor.SelectSingle<DynamicMapperGenerator>(
                        getMapperGenerator
                            .setParam("BaseRouteID", propriety.ShowType.IsRef() ? (long)BaseRoutes.GETALL : (long)BaseRoutes.CBO)
                            .setParam("ProprietyID", propriety.id)
                        );
                if (propriety.MapperGenerator is not null)
                    await DynamicMapperGenerator.init(propriety.MapperGenerator);
            }
            propriety.roles = (
                await DynamicController.executor.SelectDictionary<long, bool>(
                    getRoles
                        .setParam("ProprietyID", propriety.id)
                    )
                );
            if (propriety.ShowType.IsRef() && !propriety.roles.Any())//!propriety.roles.ContainsKey(AnonymousRoleID))
                propriety.roles[AnonymousRoleID] = false;
            return propriety;
        }
        public async static Task<DynamicPropriety> addPropriety(string Name, bool IsMain, bool IsUpdatable, ShowTypes showType, string description, string displayName, int ind, long ControllerID, params ValidatorBundle[] validatorBundle)
        {
            var dynamicPropriety = new DynamicPropriety(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertPropriety
                        .setParam("Name", Name)
                        .setParam("IsMain", IsMain)
                        .setParam("IsUpdatable", IsUpdatable)
                        .setParam("ShowTypeID", (long)showType)
                        .setParam("ControllerID", ControllerID)
                        .setParam("ind", ind)
                        .setParam("description", description)
                        .setParam("displayName", displayName)
                    )
                , Name
                , IsMain
                , IsUpdatable
                , (long)showType
                , ind
                , description
                , displayName
            );

            foreach (var validator in validatorBundle)
            {
                dynamicPropriety.Validators.Add(await DynamicValidator.addValidator(validator, dynamicPropriety.id, true));
            }
            return dynamicPropriety;
        }
        public async Task<DynamicPropriety> addMapperGenerator(string ControllerName, params ParamLinker[] linkers)
        {
            if (!ShowType.IsRef())
                throw new Exception("cannot add mapper on not ref proprety");
            MapperGenerator = await DynamicMapperGenerator.addMapperGenerator(ControllerName, id, false, linkers);
            return this;
        }
        public async Task<DynamicPropriety> addCBOInfo(string ControllerName, string value, params ParamLinker[] linkers)
        {
            if (!ShowType.IsCBO())
                throw new Exception("cannot add cboInfo on not cbo proprety");
            MapperGenerator = await DynamicMapperGenerator.addMapperGenerator(ControllerName, id, value, linkers);
            return this;
        }
        public async Task<DynamicPropriety> addValidator(string Value, ValidatorTypes ValidatorType)
        {
            Validators.Add(await DynamicValidator.addValidator(Value, id, ValidatorType, true));
            return this;
        }
        public Task<DynamicPropriety> Anonymous()
        {
            return addAuthorizedRole(AnonymousRoleID, false);
        }
        public async Task<DynamicPropriety> addAuthorizedRoles(params KeyValuePair<long, bool>[] RolesID)
        {
            foreach (var RoleID in RolesID)
                await addAuthorizedRole(RoleID.Key, RoleID.Value);
            return this;
        }
        public async Task<DynamicPropriety> addAuthorizedRole(long RoleID, bool CanModify)
        {
            await DynamicController.executor.ExecuteQueryWithTransaction(
                insertRole
                    .setParam("RoleID", RoleID)
                    .setParam("ProprietyID", id)
                    .setParam("CanModify", CanModify)
                );
            roles.Add(RoleID, CanModify);
            return this;
        }
        internal bool CanSee(params long[] rolesUser)
        {
            return CanSee((IEnumerable<long>)rolesUser);
        }
        internal bool CanModify(params long[] rolesUser)
        {
            return CanModify((IEnumerable<long>)rolesUser);
        }
        internal bool CanSee(IEnumerable<long> rolesUser)
        {
            return roles.ContainsKey(AnonymousRoleID) || rolesUser.Any(role => roles.ContainsKey(role));
        }
        internal bool CanModify(IEnumerable<long> rolesUser)
        {
            return (roles.ContainsKey(AnonymousRoleID) && roles[AnonymousRoleID]) || rolesUser.Any(role => roles.ContainsKey(role) && roles[role]);
        }
    }
}
