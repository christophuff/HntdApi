using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using HntdApi.Data;
using HntdApi.DTOs;
using HntdApi.Models;

namespace HntdApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private HntdApiDbContext _dbContext;
    private UserManager<IdentityUser> _userManager;

    public AuthController(HntdApiDbContext context, UserManager<IdentityUser> userManager)
    {
        _dbContext = context;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public IActionResult Login([FromHeader(Name = "Authorization")] string authHeader)
    {
        try
        {
            string encodedCreds = authHeader.Substring(6).Trim();
            string creds = Encoding
                .GetEncoding("iso-8859-1")
                .GetString(Convert.FromBase64String(encodedCreds));

            int separator = creds.IndexOf(':');
            string email = creds.Substring(0, separator);
            string password = creds.Substring(separator + 1);

            var identityUser = _dbContext.UserProfiles
                .Where(u => u.Email == email)
                .Select(u => _userManager.Users.FirstOrDefault(iu => iu.Id == u.IdentityUserId))
                .FirstOrDefault();

            if (identityUser == null)
                return Unauthorized();

            var hasher = new PasswordHasher<IdentityUser>();
            var result = hasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, password);

            if (result == PasswordVerificationResult.Success)
            {
                var userProfile = _dbContext.UserProfiles.FirstOrDefault(up => up.IdentityUserId == identityUser.Id);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                    new Claim(ClaimTypes.Name, userProfile.Username),
                    new Claim(ClaimTypes.Email, userProfile.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

                HttpContext.SignInAsync(
                    IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(claimsIdentity)).Wait();

                return Ok(new UserDTO
                {
                    Id = userProfile.Id,
                    Username = userProfile.Username,
                    Email = userProfile.Email,
                    DateCreated = userProfile.DateCreated,
                    ImageUrl = userProfile.ImageUrl
                });
            }

            return Unauthorized();
        }
        catch
        {
            return Unauthorized();
        }
    }

    [HttpGet("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme).Wait();
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(up => up.IdentityUserId == identityUserId);

        if (userProfile == null)
            return NotFound();

        return Ok(new UserDTO
        {
            Id = userProfile.Id,
            Username = userProfile.Username,
            Email = userProfile.Email,
            DateCreated = userProfile.DateCreated,
            ImageUrl = userProfile.ImageUrl
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegistrationDTO registration)
    {
        var identityUser = new IdentityUser
        {
            UserName = registration.Email,
            Email = registration.Email
        };

        var result = await _userManager.CreateAsync(identityUser, registration.Password);

        if (result.Succeeded)
        {
            var userProfile = new User
            {
                IdentityUserId = identityUser.Id,
                Username = registration.Username,
                Email = registration.Email,
                DateCreated = DateTime.UtcNow
            };

            _dbContext.UserProfiles.Add(userProfile);
            _dbContext.SaveChanges();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                new Claim(ClaimTypes.Name, userProfile.Username),
                new Claim(ClaimTypes.Email, userProfile.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Ok(new UserDTO
            {
                Id = userProfile.Id,
                Username = userProfile.Username,
                Email = userProfile.Email,
                DateCreated = userProfile.DateCreated
            });
        }

        return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
    }

    [HttpPut("profile")]
    [Authorize]
    public IActionResult UpdateProfile(UpdateProfileDTO dto)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(u => u.IdentityUserId == identityUserId);

        if (userProfile == null)
        {
            return Unauthorized();
        }

        userProfile.Username = dto.Username;
        userProfile.ImageUrl = dto.ImageUrl;

        _dbContext.SaveChanges();

        return Ok(new UserDTO
        {
            Id = userProfile.Id,
            Username = userProfile.Username,
            Email = userProfile.Email,
            ImageUrl = userProfile.ImageUrl,
            DateCreated = userProfile.DateCreated
        });
    }
}