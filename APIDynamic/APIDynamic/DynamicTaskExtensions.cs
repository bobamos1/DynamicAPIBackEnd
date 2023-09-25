﻿using DynamicSQLFetcher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading.Tasks;

namespace APIDynamic
{
    public static class DynamicTaskExtensions
    {
        public async static Task<Dictionary<string, DynamicController>> addController(this Dictionary<string, DynamicController> task, string Name, bool IsMain)
        {
            task.Add(Name, await DynamicController.addController(Name, IsMain));
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addRoute(this Dictionary<string, DynamicController> task, string controllerName, BaseRoutes baseRoute)
        {
            await task[controllerName].addRoute(baseRoute);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addRoute(this Dictionary<string, DynamicController> task, string controllerName, string Name)
        {
            await task[controllerName].addRoute(Name);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addRouteQuery(this Dictionary<string, DynamicController> task, string controllerName, string routeName, string queryString, QueryTypes QueryType, bool CompleteCheck, bool CompleteAuth)
        {
            await task[controllerName].addRouteQuery(routeName, queryString, QueryType, CompleteCheck, CompleteAuth);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addFilter(this Dictionary<string, DynamicController> task, string controllerName, string routeName, int index, string name, ShowTypes showType, string VarAffected)
        {
            await task[controllerName].addFilter(routeName, index, name, showType, VarAffected);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addSQLParamInfo(this Dictionary<string, DynamicController> task, string controllerName, string routeName, int index, string varAffected, long ProprietyID)
        {
            await task[controllerName].addSQLParamInfo(routeName, index, varAffected, ProprietyID);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addValidatorForSQLParam(this Dictionary<string, DynamicController> task, string controllerName, string routeName, int indexQuery, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            await task[controllerName].addValidatorForSQLParam(routeName, indexQuery, VarAffected, Value, ValidatorType);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addPropriety(this Dictionary<string, DynamicController> task, string controllerName, string Name, bool IsMain, bool IsReadOnly, ShowTypes showType)
        {
            await task[controllerName].addPropriety(Name, IsMain, IsReadOnly, showType);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addValidatorForPropriety(this Dictionary<string, DynamicController> task, string controllerName, string ProprietyName, string Value, ValidatorTypes ValidatorType)
        {
            await task[controllerName].addValidatorForPropriety(ProprietyName, Value, ValidatorType);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addMapperGenerator(this Dictionary<string, DynamicController> task, string controllerName, string ProprietyName, string ControllerToLinkName, params ParamLinker[] linkers)
        {
            await task[controllerName].addMapperGenerator(ProprietyName, ControllerToLinkName, linkers);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addAuthorizedRouteRole(this Dictionary<string, DynamicController> task, string controllerName, string routeName, long RoleID)
        {
            await task[controllerName].addAuthorizedRouteRole(routeName, RoleID);
            return task;
        }
        public async static Task<Dictionary<string, DynamicController>> addAuthorizedProprietyRole(this Dictionary<string, DynamicController> task, string controllerName, string ProprietyName, long RoleID, bool CanModify)
        {
            await task[controllerName].addAuthorizedProprietyRole(ProprietyName, RoleID, CanModify);
            return task;
        }



        public async static Task<Dictionary<string, DynamicController>> addController(this Task<Dictionary<string, DynamicController>> task, string Name, bool IsMain)
        {
            return await (await task).addController(Name, IsMain);
        }
        public async static Task<Dictionary<string, DynamicController>> addRoute(this Task<Dictionary<string, DynamicController>> task, string controllerName, BaseRoutes baseRoute)
        {
            return await (await task).addRoute(controllerName, baseRoute);
        }
        public async static Task<Dictionary<string, DynamicController>> addRoute(this Task<Dictionary<string, DynamicController>> task, string controllerName, string Name)
        {
            return await (await task).addRoute(controllerName, Name);
        }
        public async static Task<Dictionary<string, DynamicController>> addRouteQuery(this Task<Dictionary<string, DynamicController>> task, string controllerName, string routeName, string queryString, QueryTypes QueryType, bool CompleteCheck, bool CompleteAuth)
        {
            return await (await task).addRouteQuery(controllerName, routeName, queryString, QueryType, CompleteCheck, CompleteAuth);
        }
        public async static Task<Dictionary<string, DynamicController>> addFilter(this Task<Dictionary<string, DynamicController>> task, string controllerName, string routeName, int index, string name, ShowTypes showType, string VarAffected)
        {
            return await (await task).addFilter(controllerName, routeName, index, name, showType, VarAffected);
        }
        public async static Task<Dictionary<string, DynamicController>> addSQLParamInfo(this Task<Dictionary<string, DynamicController>> task, string controllerName, string routeName, int index, string varAffected, long ProprietyID)
        {
            return await (await task).addSQLParamInfo(controllerName, routeName, index, varAffected, ProprietyID);
        }
        public async static Task<Dictionary<string, DynamicController>> addValidatorForSQLParam(this Task<Dictionary<string, DynamicController>> task, string controllerName, string routeName, int indexQuery, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            return await (await task).addValidatorForSQLParam(controllerName, routeName, indexQuery, VarAffected, Value, ValidatorType);
        }
        public async static Task<Dictionary<string, DynamicController>> addPropriety(this Task<Dictionary<string, DynamicController>> task, string controllerName, string Name, bool IsMain, bool IsReadOnly, ShowTypes showType)
        {
            return await (await task).addPropriety(controllerName, Name, IsMain, IsReadOnly, showType);
        }
        public async static Task<Dictionary<string, DynamicController>> addValidatorForPropriety(this Task<Dictionary<string, DynamicController>> task, string controllerName, string ProprietyName, string Value, ValidatorTypes ValidatorType)
        {
            return await (await task).addValidatorForPropriety(controllerName, ProprietyName, Value, ValidatorType);
        }
        public async static Task<Dictionary<string, DynamicController>> addMapperGenerator(this Task<Dictionary<string, DynamicController>> task, string controllerName, string ProprietyName, string ControllerToLinkName, params ParamLinker[] linkers)
        {
            return await (await task).addMapperGenerator(controllerName, ProprietyName, ControllerToLinkName, linkers);
        }
        public async static Task<Dictionary<string, DynamicController>> addAuthorizedRouteRole(this Task<Dictionary<string, DynamicController>> task, string controllerName, string routeName, long RoleID)
        {
            return await (await task).addAuthorizedRouteRole(controllerName, routeName, RoleID);
        }
        public async static Task<Dictionary<string, DynamicController>> addAuthorizedProprietyRole(this Task<Dictionary<string, DynamicController>> task, string controllerName, string ProprietyName, long RoleID, bool CanModify)
        {
            return await (await task).addAuthorizedProprietyRole(controllerName, ProprietyName, RoleID, CanModify);
        }




        public async static Task<DynamicController> addRoute(this Task<DynamicController> task, BaseRoutes baseRoute)
        {
            return await (await task).addRoute(baseRoute);
        }
        public async static Task<DynamicController> addRoute(this Task<DynamicController> task, string Name)
        {
            return await (await task).addRoute(Name);
        }
        public async static Task<DynamicController> addRouteQuery(this Task<DynamicController> task, string routeName, string queryString, QueryTypes QueryType, bool CompleteCheck, bool CompleteAuth)
        {
            return await (await task).addRouteQuery(routeName, queryString, QueryType, CompleteCheck, CompleteAuth);
        }
        public async static Task<DynamicController> addSQLParamInfo(this Task<DynamicController> task, string routeName, int index, string varAffected, long ProprietyID)
        {
            return await (await task).addSQLParamInfo(routeName, index, varAffected, ProprietyID);
        }
        public async static Task<DynamicController> addValidatorForSQLParam(this Task<DynamicController> task, string routeName, int indexQuery, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            return await (await task).addValidatorForSQLParam(routeName, indexQuery, VarAffected, Value, ValidatorType);
        }
        public async static Task<DynamicController> addFilter(this Task<DynamicController> task, string routeName, int index, string name, ShowTypes showType, string VarAffected)
        {
            return await (await task).addFilter(routeName, index, name, showType, VarAffected);
        }
        public async static Task<DynamicController> addPropriety(this Task<DynamicController> task, string Name, bool IsMain, bool IsReadOnly, ShowTypes showType)
        {
            return await (await task).addPropriety(Name, IsMain, IsReadOnly, showType);
        }
        public async static Task<DynamicController> addValidatorForPropriety(this Task<DynamicController> task, string ProprietyName, string Value, ValidatorTypes ValidatorType)
        {
            return await (await task).addValidatorForPropriety(ProprietyName, Value, ValidatorType);
        }
        public async static Task<DynamicController> addMapperGenerator(this Task<DynamicController> task, string ProprietyName, string ControllerName, params ParamLinker[] linkers)
        {
            return await (await task).addMapperGenerator(ProprietyName, ControllerName, linkers);
        }
        public async static Task<DynamicController> addAuthorizedRouteRole(this Task<DynamicController> task, string routeName, long RoleID)
        {
            return await (await task).addAuthorizedRouteRole(routeName, RoleID);
        }
        public async static Task<DynamicController> addAuthorizedProprietyRole(this Task<DynamicController> task, string ProprietyName, long RoleID, bool CanModify)
        {
            return await (await task).addAuthorizedProprietyRole(ProprietyName, RoleID, CanModify);
        }




        public async static Task<DynamicRoute> addRouteQuery(this Task<DynamicRoute> task, string queryString, QueryTypes QueryType, bool CompleteCheck, bool CompleteAuth)
        {
            return await (await task).addRouteQuery(queryString, QueryType, CompleteCheck, CompleteAuth);
        }
        public async static Task<DynamicRoute> addSQLParamInfo(this Task<DynamicRoute> task, int index, string varAffected, long ProprietyID)
        {
            return await (await task).addSQLParamInfo(index, varAffected, ProprietyID);
        }
        public async static Task<DynamicRoute> addValidator(this Task<DynamicRoute> task, int indexQuery, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            return await (await task).addValidator(indexQuery, VarAffected, Value, ValidatorType);
        }
        public async static Task<DynamicRoute> addFilter(this Task<DynamicRoute> task, int index, string name, ShowTypes showType, string VarAffected)
        {
            return await (await task).addFilter(index, name, showType, VarAffected);
        }
        public async static Task<DynamicRoute> addAuthorizedRole(this Task<DynamicRoute> task, long RoleID)
        {
            return await (await task).addAuthorizedRole(RoleID);
        }




        public async static Task<DynamicQueryForRoute> addSQLParamInfo(this Task<DynamicQueryForRoute> task, string varAffected, long ProprietyID)
        {
            return await (await task).addSQLParamInfo(varAffected, ProprietyID);
        }
        public async static Task<DynamicQueryForRoute> addValidator(this Task<DynamicQueryForRoute> task, string VarAffected, string Value, ValidatorTypes ValidatorType)
        {
            return await (await task).addValidator(VarAffected, Value, ValidatorType);
        }
        public async static Task<DynamicQueryForRoute> addFilter(this Task<DynamicQueryForRoute> task, string name, ShowTypes showType, string VarAffected)
        {
            return await (await task).addFilter(name, showType, VarAffected);
        }



        public async static Task<DynamicSQLParamInfo> addValidator(this Task<DynamicSQLParamInfo> task, string Value, ValidatorTypes ValidatorTypeID)
        {
            return await (await task).addValidator(Value, ValidatorTypeID);
        }




        public async static Task<DynamicPropriety> addMapperGenerator(this Task<DynamicPropriety> task, string ControllerName, params ParamLinker[] linkers)
        {
            return await (await task).addMapperGenerator(ControllerName, linkers);
        }
        public async static Task<DynamicPropriety> addValidator(this Task<DynamicPropriety> task, string Value, ValidatorTypes ValidatorTypeID)
        {
            return await (await task).addValidator(Value, ValidatorTypeID);
        }
        public async static Task<DynamicPropriety> addAuthorizedRole(this Task<DynamicPropriety> task, long RoleID, bool CanModify)
        {
            return await (await task).addAuthorizedRole(RoleID, CanModify);
        }
    }
}