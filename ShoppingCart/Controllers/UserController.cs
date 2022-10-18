using BusinessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{

    private IUserService _userService;

    private readonly UserManager<IdentityUser> _userManager;

    public UserController
    (
        IUserService userService,
        UserManager<IdentityUser> userManager
    )
    { 
        _userService = userService;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("all")]
    public async Task<ActionResult> GetUsers()
    { 
        var users = await _userService.GetUsers();
        return Ok(users);
    }

    [HttpGet]
    [Route("profile")]
    public async Task<ActionResult> GetProfile()
    {
        var logedInUser= await _userManager.GetUserAsync(HttpContext.User);
       
        if (logedInUser==null)
        {
            return BadRequest("User not found");
        }

        var user = await _userService.GetUserByIdentityId(Guid.Parse(logedInUser.Id));
        return Ok(user);
    }

    [HttpGet("{userId}", Name = "GetUser")]
    public async Task<ActionResult> GetUser(Guid userId)
    {
        var user = await _userService.GetUser(userId);
        return Ok(user);
    }

    [HttpPut]
    [Route("")]
    public async Task<ActionResult> PutUser(UpdateUserModel model)
    {
        var user = await _userService.UpdateUserProfile(model);
        return Ok(user);
    }
}

