using System.Data;

namespace DynamicStructureObjects
{
    public record ParamInfoResume(string name, bool isMain, string description, string placeholder, long showTypeID, MapperResume? mapper, int ind, params ParamAffectedResume[] paramAffecteds)
    {

    }
    public record ParamAffectedResume(string name, bool isRequired, params ValidatorResume[] validators)
    {

    }
    public record ValidatorResume(object valueToUse, long validatorTypeID, string message)
    {

    }
    public record MapperResume(string refController, Dictionary<string, string> parametersToLink, Dictionary<string, object> baseParameters)
    {

    }
    public record RouteResume(string name, RouteDisplayTypes routeDisplayType, RouteTypes routeType)
    {
        internal bool RequireAuthorization { get; }
        internal List<long> Roles { get; }

        internal RouteResume(string name, RouteDisplayTypes routeDisplayType, RouteTypes routeType, bool requireAuthorization, List<long> roles) : this(name, routeDisplayType, routeType)
        {
            this.RequireAuthorization = requireAuthorization;
            this.Roles = roles;
        }

        internal bool CanUse(IEnumerable<long> rolesUser)
        {
            return !RequireAuthorization || Roles.Any(role => rolesUser.Contains(role));
        }
    }
}
