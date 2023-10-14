﻿using DynamicSQLFetcher;
using System.Reflection.Metadata;

namespace DynamicStructureObjects
{
    public class DynamicSQLParamInfo
    {
        public long id { get; internal set; }
        public long ProprietyID { get; internal set; }
        public string VarAffected { get; internal set; }
        public bool isRequired { get; internal set; }
        public ShowTypes showType { get; internal set; }
        public int ind { get; internal set; }
        internal List<DynamicValidator> Validators { get; set; }
        internal static readonly Query getValidators = Query.fromQueryString(QueryTypes.SELECT, "SELECT value AS Value, id_ValidatorType AS ValidatorTypeID, message FROM ValidatorSQLParamInfoValues WHERE id_SQLParamInfo = @SQLParamInfoID UNION ALL SELECT value AS Value, id_ValidatorType AS ValidatorTypeID, message FROM ValidatorProprietyValues WHERE id_Propriety = @ProprietyID", true);
        internal static readonly Query insertSQLParamInfo = Query.fromQueryString(QueryTypes.INSERT, "INSERT INTO SQLParamInfos (id_Propriety, id_RouteQuery, varAffected, id_ShowType, ind) VALUES (@PropretyID, @RouteQueryID, @VarAffected, @ShowTypeID, @ind)", true);
        internal static readonly Query getProprietyShowType = Query.fromQueryString(QueryTypes.VALUE, "SELECT id_ShowType FROM Proprieties WHERE id = @ID");
        internal DynamicSQLParamInfo(long id, string VarAffected, long ProprietyID, long showTypeID, int ind)
        {
            this.id = id;
            this.VarAffected = VarAffected;
            this.ProprietyID = ProprietyID;
            this.Validators = new List<DynamicValidator>();
            this.isRequired = false;
            this.showType = (ShowTypes)showTypeID;
            this.ind = ind;
        }
        internal DynamicSQLParamInfo(DynamicSQLParamInfo paramInfo, bool isRequired)
        {
            this.id = paramInfo.id;
            this.VarAffected = paramInfo.VarAffected;
            this.ProprietyID = paramInfo.ProprietyID;
            this.Validators = paramInfo.Validators;
            this.isRequired = isRequired;
        }
        internal static async Task<DynamicSQLParamInfo> init(DynamicSQLParamInfo paramInfo)
        {
            paramInfo.Validators = (
                await DynamicController.executor.SelectQuery<DynamicValidator>(
                    getValidators
                        .setParam("SQLParamInfoID", paramInfo.id)
                        .setParam("ProprietyID", paramInfo.ProprietyID)
                    )
                ).ToList();
            foreach (var query in paramInfo.Validators)
                await DynamicValidator.init(query);
            var requiredItem = paramInfo.Validators.FirstOrDefault(item => item.ValidatorType == ValidatorTypes.REQUIRED);

            if (requiredItem != null)
            {
                paramInfo.isRequired = true;
                paramInfo.Validators.Remove(requiredItem);
            }
            if (paramInfo.ProprietyID != 1)
                paramInfo.showType = (ShowTypes)(await DynamicController.executor.SelectValue<long>(getProprietyShowType.setParam("ID", paramInfo.ProprietyID)));
            return paramInfo;
        }
        public async static Task<DynamicSQLParamInfo> addSQLParam(string VarAffected, long ProprietyID, long RouteQueryID, ShowTypes? showType, int ind)
        {
            return new DynamicSQLParamInfo(
                await DynamicController.executor.ExecuteInsertWithLastID(
                    insertSQLParamInfo
                        .setParam("PropretyID", ProprietyID)
                        .setParam("RouteQueryID", RouteQueryID)
                        .setParam("VarAffected", VarAffected)
                        .setParam("ShowTypeID", showType is null ? ShowTypes.NONE : (long)showType)
                        .setParam("ind", ind)
                    )
                , VarAffected
                , ProprietyID
                , (long)showType
                , ind
            );
        }
        public async Task<DynamicSQLParamInfo> addValidator(string Value, ValidatorTypes ValidatorType)
        {
            Validators.Add(await DynamicValidator.addValidator(Value, id, ValidatorType, false));
            return this;
        }
        public Task<DynamicSQLParamInfo> addValidator(params ValidatorBundle[] bundles)
        {
            return addValidator(false, bundles);
        }
        public async Task<DynamicSQLParamInfo> addValidator(bool addRequired, params ValidatorBundle[] bundles)
        {
            foreach (var bundle in bundles)
                Validators.Add(await DynamicValidator.addValidator(bundle, id, false));
            if (addRequired)
                Validators.Add(await DynamicValidator.addValidator(ValidatorTypes.REQUIRED.SetValue("true", "needed"), id, false));
            return this;
        }
        internal bool validateParam(object value)
        {
            return Validators.All(validator => validator.validateParam(value));
        }
    }
}
