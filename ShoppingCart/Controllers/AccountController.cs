using Authentication;
using BusinessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private IAuthHelper authHelper;

    private IUserService _userService;

    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(
       IAuthHelper authHelper,
       IUserService userService,
       UserManager<IdentityUser> userManager)
    {
        this.authHelper = authHelper;
        _userService = userService;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationRequestDto)
    {
        if (ModelState.IsValid)
        {
            var userExist = await _userManager.FindByEmailAsync(registrationRequestDto.Email);

            if (userExist != null)
            {
                return BadRequest(new UserRegistrationResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                     "Email already in use"
                    }
                });
            }

            var newUser = new IdentityUser()
            {
                Email = registrationRequestDto.Email,
                UserName = registrationRequestDto.Email,
                EmailConfirmed = true, //TODO: conform email
            };

            var isCreated = await _userManager.CreateAsync(newUser,registrationRequestDto.Password);

            if (!isCreated.Succeeded)
            {
                return BadRequest(new UserRegistrationResponseDto()
                {
                    Success = false,
                    Errors = isCreated.Errors.Select(x=>x.Description).ToList(),
                });
            }

            registrationRequestDto.IdentityId = Guid.Parse( newUser.Id);
            await _userService.SaveUser(registrationRequestDto);
            var token = await authHelper.GenerateJWTToken(newUser);
            return Ok(new UserRegistrationResponseDto() 
            { 
                Success=true,
                Token = token.JwtToken,
                RefreshToken= token.RefreshToken
            });
            
        }
        else
        {
            return BadRequest(new UserRegistrationResponseDto
            { 
                Success = false,
                Errors = new List<string>() { 
                    "Invalied payload"
                }
            });
        }
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginRequestDto userLoginRequestDto)
    {
        if (ModelState.IsValid)
        {
            var userExist = await _userManager.FindByEmailAsync(userLoginRequestDto.Email);

            if (userExist == null)
            {
                return BadRequest(new UserLoginResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                      "Invalied Authentication Request"
                    }
                });
            }

            var isCorrect = await _userManager.CheckPasswordAsync(userExist,userLoginRequestDto.Password);

            if (isCorrect)
            {
                var token = await authHelper.GenerateJWTToken(userExist);
                return Ok(new UserLoginResponseDto()
                {
                    Success = true,
                    Token = token.JwtToken,
                    RefreshToken = token.RefreshToken
                });
            }
            else
            {
                return BadRequest(new UserLoginResponseDto()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                      "Invalied Authentication Request"
                    }
                });
            }
        }
        else
        {
            return BadRequest(new UserLoginResponseDto
            {
                Success = false,
                Errors = new List<string>() {
                    "Invalied payload"
                }
            });
        }
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<ActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
    {
        if (ModelState.IsValid)
        {
            var result =await authHelper.GenerateTokenByRefreshToken(tokenRequest);
            if (result == null)
            {
                return BadRequest(new UserLoginResponseDto
                {
                    Success = false,
                    Errors = new List<string>() {
                    "token validation failed"
                }
                });
            }
            return Ok(result);
        }
        else
        {
            return BadRequest(new UserLoginResponseDto
            {
                Success = false,
                Errors = new List<string>() {
                    "Invalied payload"
                }
            });
        }
    }
}
