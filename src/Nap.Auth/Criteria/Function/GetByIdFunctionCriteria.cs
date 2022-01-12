using Fanex.Data.Repository;

namespace Nap.Auth.Criteria.Function;

public class GetByIdFunctionCriteria : CriteriaBase
{
    public int FunctionId { get; set; }

    public override string GetSettingKey()
    {
        return "NAP_Function_GetById";
    }

    public override bool IsValid()
    {
        return true;
    }
}