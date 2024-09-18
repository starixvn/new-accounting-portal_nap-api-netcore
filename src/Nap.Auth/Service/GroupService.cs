using Nap.Auth.Model;

namespace Nap.Auth.Service;

using DataCachingPull;

public interface GroupService
{
    Task<Group> GetByIdAsync(int id);
}

public class GroupServiceImp(MemoryCachingStorage cachingService) : GroupService
{
    public async Task<Group> GetByIdAsync(int id)
    {
        var group = cachingService.GetGroup(id);

        return await Task.FromResult(group);
    }
}