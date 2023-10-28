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
}
