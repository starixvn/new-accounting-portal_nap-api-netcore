using Microsoft.AspNetCore.Mvc;
using Nap.Auth.Model;
using Nap.Auth.Service;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupController(GroupService groupService) : ControllerBase
{
    [Route("{id:int}")]
    [HttpGet]
    public async Task<Group> Get(int id)
    {
        return await groupService.GetByIdAsync(id).ConfigureAwait(false);
    }
}
