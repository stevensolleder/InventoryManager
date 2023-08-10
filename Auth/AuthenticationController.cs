/*using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using InventoryManager.DataTransferObjects;
using InventoryManager.Model;
using InventoryManager.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManager.Auth;

[ApiController]
[Route("/v1/authentication")]
public class AuthenticationController:ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public AuthenticationController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _jwtSecurityTokenHandler = new ();
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Registration registration)
    {
        ApplicationUser newUser = new ApplicationUser(registration.Username, registration.Email);
        IdentityResult result = await _userManager.CreateAsync(newUser, registration.Password);
        if (!result.Succeeded) return BadRequest();
        
        return NoContent();
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] Login login)
    {
        ApplicationUser? foundUser=await _userManager.FindByNameAsync(login.Username);
        if(foundUser==null) return BadRequest();

        if(!await _userManager.CheckPasswordAsync(foundUser, login.Password)) return Unauthorized();
        
        return GenerateJwtToken(foundUser.Id);
    }
    
    [HttpPost("refresh-token"), Authorize]
    public async Task<ActionResult<string?>> Logout()
    {
        return User.FindFirstValue("refreshToken");
    }
    
    [HttpGet("invalidate-tokens"), Authorize]
    public async Task<IActionResult> InvalidateToken()
    {
        return NoContent();
    }
    
    [HttpGet("reset-email"), Authorize]
    public async Task<IActionResult> ResetEmail()
    {
        await _userManager.GetUserAsync(User);
        
        return NoContent();
    }
    
    [HttpGet("confirm-email"), Authorize]
    public async Task<IActionResult> ConfirmEmail()
    {
        return NoContent();
    }
    
    
    [HttpGet("me"), Authorize]
    public async Task<ActionResult<ApplicationUser>> Me()
    {
        ApplicationUser? foundUser = await _userManager.GetUserAsync(User);
        if (foundUser == null) return BadRequest();

        return foundUser;
    }
    
    
    
    public string GenerateJwtToken(string userId)
    {
        return _jwtSecurityTokenHandler.WriteToken(
            _jwtSecurityTokenHandler.CreateToken(
                new SecurityTokenDescriptor
                {
                    Issuer = Constants.Issuer,
                    Audience = Constants.Audience,
                    Expires = DateTime.UtcNow.AddSeconds(Constants.TokenExpirationTime),
                    SigningCredentials = new SigningCredentials(Constants.SigningKey, Constants.SigningAlgorithm),
                    Subject = new ClaimsIdentity(new List<Claim>
                        {
                            new (ClaimTypes.NameIdentifier, userId),
                            new (
                                "refreshToken", 
                                Guid.NewGuid().ToString().Replace("-", string.Empty)
                            ),
                            new (
                                "refreshTokenExpires", 
                                ((DateTimeOffset) DateTime.UtcNow.AddSeconds(Constants.TokenExpirationTime*10)).ToUnixTimeSeconds().ToString(), 
                            ClaimValueTypes.Integer
                            )
                        }
                    )
                }
            )
        );
    }
}

internal record class RefreshToken(string userId, string refreshToken, string refreshTokenExpires);*/