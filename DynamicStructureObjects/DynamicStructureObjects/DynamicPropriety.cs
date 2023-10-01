using DynamicSQLFetcher;

namespace DynamicStructureObjects
{
    public class DynamicPropriety
    {
        public long id { get; internal set; }
        public string Name { get; internal set; }
        public bool IsMain { get; internal set; }
        public bool ReadOnly { get; internal set; }
        public List<DynamicValidator> Validators { get; internal set; }
        public ShowTypes ShowType { get; internal set; }
        public DynamicMapperGenerator MapperGenerator { get; internal set; }
        public Dictionary<long, bool> roles { get; internal set; }
        internal string getCBOIdName() => ShowType == ShowTypes.CBO ? MapperGenerator.parametersToLink[SQLExecutor.KEY_FOR_CBO] : null;
        internal static readonly Query getRoles = Query.fromQueryString(QueryTypes.CBO, "SELECT id, canModify FROM PermissionProprieties INNER JOIN Roles ON id = id_role WHERE id_propriety = @ProprietyID", true, true);
        internal static readonly Query getValidators = Query.fromQueryString(QueryTypes.SELECT, "SELECT value AS Value, id_ValidatorType AS ValidatorTypeID FROM ValidatorProprietyValues WHERE id_Propriety = @ProprietyID", true, true);
        internal static readonly Query getMapperGenerator = Query.fromQueryString(QueryTypes.SELECT, "SELECT lnk.id AS id, c.id AS controllerID, c.Name AS controllerName, urlR.id AS RouteID, SQLString AS queryString, id_queryType AS QueryTypeID, completeCheck AS CompleteCheck, p.name AS ProprietyName FROM LinkProprietiesControllers lnk INNER JOIN Controllers c ON c.id = lnk.id_controller INNER JOIN URLRoutes urlR ON urlR.id_controller = c.id INNER JOIN RouteQueries rq ON rq.id_route = urlR.id INNER JOIN Proprieties p ON p.id = lnk.id_propriety WHERE urlR.id_baseRoute = @BaseRouteID AND rq.ind = 1 AND lnk.id_propriety = @ProprietyID", true, true);
        internal static readonly Query insertPropriety = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO Proprieties (name, isMain, isReadOnly,id_ShowType, id_controller) VALUES (@Name, @IsMain, @IsReadOnly, @ShowTypeID, @ControllerID)", true, true);
        internal static readonly Query insertRole = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO PermissionProprieties(id_propriety, id_role, canModify) VALUES(@ProprietyID, @RoleID, @CanModify)", true, true);
        internal DynamicPropriety(long id, string Name, bool IsMain, bool ReadOnly, long ShowTypeID)
        {
            this.id = id;
            this.Name = Name;
            this.IsMain = IsMain;
            this.ReadOnly = ReadOnly;
            this.ShowType = (ShowTypes)ShowTypeID;
            this.Validators = new List<DynamicValidator>();
            this.MapperGenerator = null;
        }
        internal static async Task<DynamicPropriety> init(DynamicPropriety propriety)
        {
            propriety.Validators = (
                await DynamicController.executor.SelectQuery<DynamicValidator>(
                    getValidators
                        .setParam("ProprietyID", propriety.id)
                    )
                ).ToList();
            if (propriety.ShowType == ShowTypes.Ref || propriety.ShowType == ShowTypes.CBO)
            {
                propriety.MapperGenerator = 
                    await DynamicController.executor.SelectSingle<DynamicMapperGenerator>(
                        getMapperGenerator
                            .setParam("BaseRouteID", propriety.ShowType == ShowTypes.Ref ? (long)BaseRoutes.GETALL : (long)BaseRoutes.CBO)
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
            return propriety;
        }
        public async static Task<DynamicPropriety> addPropriety(string Name, bool IsMain, bool IsReadOnly, ShowTypes showType, long ControllerID)
        {
            return new DynamicPropriety(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertPropriety
                        .setParam("Name", Name)
                        .setParam("IsMain", IsMain)
                        .setParam("IsReadOnly", IsReadOnly)
                        .setParam("ShowTypeID", (long)showType)
                        .setParam("ControllerID", ControllerID)
                    )
                , Name
                , IsMain
                , IsReadOnly
                , (long)showType
            );
        }
        public async Task<DynamicPropriety> addMapperGenerator(string ControllerName, params ParamLinker[] linkers)
        {
            if (ShowType != ShowTypes.Ref)
                throw new Exception("cannot add mapper on not ref proprety");
            MapperGenerator = await DynamicMapperGenerator.addMapperGenerator(ControllerName, id, linkers);
            return this;
        }
        public async Task<DynamicPropriety> addCBOInfo(string ControllerName, string key, params ParamLinker[] linkers)
        {
            if (ShowType != ShowTypes.CBO)
                throw new Exception("cannot add cboInfo on not cbo proprety");
            MapperGenerator = await DynamicMapperGenerator.addMapperGenerator(ControllerName, id, key, linkers);
            return this;
        }
        public async Task<DynamicPropriety> addValidator(string Value, ValidatorTypes ValidatorType)
        {
            Validators.Add(await DynamicValidator.addValidator(Value, id, ValidatorType, true));
            return this;
        }
        public async Task<DynamicPropriety> addAuthorizedRole(long RoleID, bool CanModify)
        {
            await DynamicController.executor.ExecuteInsertWithLastID(
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
            return rolesUser.Any(role => roles.ContainsKey(role));
        }
        internal bool CanModify(IEnumerable<long> rolesUser)
        {
            return rolesUser.Any(role => roles.ContainsKey(role) && roles[role]);
        }
    }
}
