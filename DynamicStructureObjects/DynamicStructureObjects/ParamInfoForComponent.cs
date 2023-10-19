using System.ComponentModel;

namespace DynamicStructureObjects
{
    public record ParamInfoResume(string Name, bool IsMain, string Description, string Placeholder, long ShowTypeID, int ind, params ParamAffectedResume[] ParamAffecteds)
    {

    }
    public record ParamAffectedResume(string Name, bool IsRequired, params ValidatorResume[] Validators)
    {

    }
    public record ValidatorResume(object ValueToUse, long ValidatorTypeID, string Message)
    {

    }
}
