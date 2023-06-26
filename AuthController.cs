using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace MyBooks;

[Route("/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("google-login")]
    public async Task<ActionResult> GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/"
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }
}