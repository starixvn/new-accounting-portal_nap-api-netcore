using Fanex.Data.Repository;
using Nap.Auth.Criteria.Group;
using Nap.Auth.Model;

namespace Nap.Auth.Service;

public interface GroupService
{
    Task<Group> GetByIdAsync(int id);
}

public class GroupServiceImp : GroupService
{
    private readonly IDynamicRepository repository;

    public GroupServiceImp(IDynamicRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Group> GetByIdAsync(int id)
    {
        var criteria = new GetByIdGroupCriteria
        {
            GroupId = id
        };

        var group = await repository.GetAsync<Group>(criteria).ConfigureAwait(false);

        return group?.ToNonNullGroup();
    }
}