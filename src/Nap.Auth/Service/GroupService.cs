using Nap.Auth.Model;

namespace Nap.Auth.Service;

using DataCachingPull;

public interface GroupService
{
    Group GetById(int id);
}

public class GroupServiceImp(MemoryCachingStorage cachingService) : GroupService
{
    public Group GetById(int id)
    {
        var group = cachingService.GetGroup(id);

        return group;
    }
}