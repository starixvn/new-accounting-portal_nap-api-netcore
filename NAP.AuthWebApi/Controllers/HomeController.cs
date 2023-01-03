using Microsoft.AspNetCore.Mvc;

namespace NAP.AuthWebApi.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [Route("")]
    [HttpGet]
    public string Index() => "Swagger link: /swagger/index.html";
}
