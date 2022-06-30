using Fanex.Data.Repository;
using Nap.Auth.Business;
using Nap.Auth.Criteria.Site;
using Nap.Auth.Model;
using Nap.Auth.Model.ValueObject;

namespace Nap.Auth.Service;

public interface SiteService
{
    Task<Site> GetByIdAsync(int id);

    Task<IEnumerable<SiteLevel>> GetSiteLevelByUserAsync(int userId, string env = Constant.Uat);
}

public class SiteServiceImp : SiteService
{
    private readonly IDynamicRepository repository;

    public SiteServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Site> GetByIdAsync(int id)
    {
        var criteria = new GetByIdSiteCriteria
        {
            SiteId = id
        };

        var site = await repository.GetAsync<Site>(criteria).ConfigureAwait(false);

        return site != null ? site.ToNonNullSite() : null;
    }

    public async Task<IEnumerable<SiteLevel>> GetSiteLevelByUserAsync(int userId, string env = Constant.Uat)
    {
        var criteria = new GetByUserSiteCriteria
        {
            UserId = userId,
            Environment = env,
            IsActive = true
        };

        return await repository.FetchAsync<SiteLevel>(criteria).ConfigureAwait(false);
    }
}