using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HntdApi.Data;
using HntdApi.DTOs;
using HntdApi.Models;

namespace HntdApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private HntdApiDbContext _dbContext;

    public FavoritesController(HntdApiDbContext context)
    {
        _dbContext = context;
    }

    // get current user's favorites
    [HttpGet]
    [Authorize]
    public IActionResult GetMyFavorites()
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(u => u.IdentityUserId == identityUserId);

        if (userProfile == null)
        {
            return Unauthorized();
        }

        var favorites = _dbContext.UserFavorites
            .Where(uf => uf.UserId == userProfile.Id)
            .Include(uf => uf.HauntedLocation)
                .ThenInclude(hl => hl.LocationType)
            .Include(uf => uf.HauntedLocation)
                .ThenInclude(hl => hl.ActivityLevel)
            .Select(uf => new UserFavoriteDTO
            {
                Id = uf.Id,
                UserId = uf.UserId,
                HauntedLocationId = uf.HauntedLocationId,
                DateAdded = uf.DateAdded,
                HauntedLocation = new HauntedLocationDTO
                {
                    Id = uf.HauntedLocation.Id,
                    Name = uf.HauntedLocation.Name,
                    Address = uf.HauntedLocation.Address,
                    City = uf.HauntedLocation.City,
                    State = uf.HauntedLocation.State,
                    Latitude = uf.HauntedLocation.Latitude,
                    Longitude = uf.HauntedLocation.Longitude,
                    Description = uf.HauntedLocation.Description,
                    ImageUrl = uf.HauntedLocation.ImageUrl,
                    DateAdded = uf.HauntedLocation.DateAdded,
                    LocationTypeId = uf.HauntedLocation.LocationTypeId,
                    LocationType = new LocationTypeDTO
                    {
                      Id = uf.HauntedLocation.LocationType.Id,
                      Name = uf.HauntedLocation.LocationType.Name  
                    },
                    ActivityLevelId = uf.HauntedLocation.ActivityLevelId,
                    ActivityLevel  = new ActivityLevelDTO
                    {
                        Id = uf.HauntedLocation.ActivityLevel.Id,
                        Name = uf.HauntedLocation.ActivityLevel.Name,
                        Description = uf.HauntedLocation.ActivityLevel.Description
                    }
                }
            })
            .ToList();
        return Ok(favorites);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Add(CreateUserFavoriteDTO dto)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(u => u.IdentityUserId == identityUserId);

        if (userProfile == null)
        {
            return Unauthorized();
        }

        // check if already favorited
        var existing = _dbContext.UserFavorites
            .FirstOrDefault(uf => uf.UserId == userProfile.Id && uf.HauntedLocationId == dto.HauntedLocationId);

        if (existing != null)
        {
            return BadRequest("Location already in favorites");
        }

        // check if location exists
        var location = _dbContext.HauntedLocations.FirstOrDefault(hl => hl.Id == dto.HauntedLocationId);
        if (location == null)
        {
            return NotFound("Location not found");
        }

        var favorite = new UserFavorite
        {
            UserId = userProfile.Id,
            HauntedLocationId = dto.HauntedLocationId,
            DateAdded = DateTime.UtcNow
        };

        _dbContext.UserFavorites.Add(favorite);
        _dbContext.SaveChanges();

        return Created($"/api/favorites/{favorite.Id}", favorite);
    }

    [HttpDelete("{locationId}")]
    [Authorize]
    public IActionResult Remove(int locationId)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(u => u.IdentityUserId == identityUserId);

        if (userProfile == null)
        {
            return Unauthorized();
        }

        var favorite = _dbContext.UserFavorites
            .FirstOrDefault(uf => uf.UserId == userProfile.Id && uf.HauntedLocationId == locationId);

        if (favorite == null)
        {
            return NotFound();
        }

        _dbContext.UserFavorites.Remove(favorite);
        _dbContext.SaveChanges();

        return NoContent();
    }
}