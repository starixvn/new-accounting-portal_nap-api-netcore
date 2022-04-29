using Fanex.Data.Repository;
using Nap.Auth.Business;
using Nap.Auth.Criteria.Function;
using Nap.Auth.Model;
using Nap.Auth.Model.ValueObject;

namespace Nap.Auth.Service;

public interface FunctionService
{
    Task<IEnumerable<FunctionPolicy>> GetByUserAndSiteIncludeGroup(int userId, int siteId, string env = Constant.Uat);

    Task<int> GetByKeyAndSite(string key, int siteId);
}

public class FunctionServiceImp : FunctionService
{
    private readonly IDynamicRepository repository;


    public FunctionServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IEnumerable<FunctionPolicy>> GetByUserAndSiteIncludeGroup(
        int userId,
        int siteId,
        string env = Constant.Uat)
    {
        var criteria = new GetByUserAndSiteIncludeGroupFunctionCriteria
        {
            UserId = userId,
            SiteId = siteId,
            Environment = env
        };

        var functionPoliciesEnumerable = await repository.FetchAsync<FunctionPolicy>(criteria).ConfigureAwait(false);

        var functionPolicies = functionPoliciesEnumerable.Select(f => f.ToNonNullFunctionPolicy()).ToList();

        await AssignParentFunctionKey(functionPolicies).ConfigureAwait(false);

        return functionPolicies;
    }

    private async Task AssignParentFunctionKey(List<FunctionPolicy> functionPolicies)
    {
        var childFunctionPolicies = functionPolicies.Where(f => f.ParentId > 0);
        foreach (var functionPolicy in childFunctionPolicies)
        {
            var parentId = functionPolicy.ParentId;
            var parentFunction = await GetById(parentId).ConfigureAwait(false);
            functionPolicy.ParentKey = StringUtility.ToNonNull(parentFunction?.Key);
        }
    }

    private async Task<Function> GetById(int id)
    {
        var criteria = new GetByIdFunctionCriteria
        {
            FunctionId = id
        };

        var function = await repository.GetAsync<Function>(criteria).ConfigureAwait(false) ?? new Function();

        return function.ToNonNullFunction();
    }

    public async Task<int> GetByKeyAndSite(string key, int siteId)
    {
        var criteria = new GetByKeyAndSiteFunctionCriteria
        {
            FunctionKey = key,
            SiteId = siteId
        };

        var function = await repository.GetAsync<Function>(criteria);
        return function == null ? 0 : function.Id;
    }
}