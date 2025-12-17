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
public class HauntedLocationsController : ControllerBase
{
    private HntdApiDbContext _dbContext;
    public HauntedLocationsController(HntdApiDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var locations = _dbContext.HauntedLocations
            .Include(hl => hl.LocationType)
            .Include(hl => hl.ActivityLevel)
            .Include(hl => hl.User)
            .Select(hl => new HauntedLocationDTO
            {
               Id = hl.Id,
               Name = hl.Name,
               Address = hl.Address,
               City = hl.City,
               State = hl.State,
               Latitude = hl.Latitude,
               Longitude = hl.Longitude,
               Description = hl.Description,
               History = hl.History,
               ImageUrl = hl.ImageUrl,
               DateAdded = hl.DateAdded,
               LocationTypeId = hl.LocationTypeId,
               LocationType = new LocationTypeDTO
               {
                   Id = hl.LocationType.Id,
                   Name = hl.LocationType.Name
               },
               ActivityLevelId = hl.ActivityLevelId,
               ActivityLevel = new ActivityLevelDTO
               {
                   Id = hl.ActivityLevel.Id,
                   Name = hl.ActivityLevel.Name,
                   Description = hl.ActivityLevel.Description
               },
               UserId = hl.UserId,
               User = new UserDTO
               {
                   Id = hl.User.Id,
                   Username = hl.User.Username,
                   Email = hl.User.Email,
                   DateCreated = hl.User.DateCreated
               },
               ParanormalActivities = _dbContext.LocationActivities
                .Where(la => la.HauntedLocationId == hl.Id)
                .Select(la => new ParanormalActivityDTO
                {
                    Id = la.ParanormalActivity.Id,
                    Name = la.ParanormalActivity.Name,
                    Description = la.ParanormalActivity.Description,
                    IconName = la.ParanormalActivity.IconName
                })
                .ToList()
            })
            .ToList();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var location = _dbContext.HauntedLocations
            .Include(hl => hl.LocationType)
            .Include(hl => hl.ActivityLevel)
            .Include(hl => hl.User)
            .Where(hl => hl.Id == id)
            .Select(hl => new HauntedLocationDTO
            {
               Id = hl.Id,
               Name = hl.Name,
               Address = hl.Address,
               City = hl.City,
               State = hl.State,
               Latitude = hl.Latitude,
               Longitude = hl.Longitude,
               Description = hl.Description,
               History = hl.History,
               ImageUrl = hl.ImageUrl,
               DateAdded = hl.DateAdded,
               LocationTypeId = hl.LocationTypeId,
               LocationType = new LocationTypeDTO
               {
                 Id = hl.LocationType.Id,
                 Name = hl.LocationType.Name  
               },
               ActivityLevelId = hl.ActivityLevelId,
               ActivityLevel = new ActivityLevelDTO
               {
                 Id = hl.ActivityLevel.Id,
                 Name = hl.ActivityLevel.Name,
                 Description = hl.ActivityLevel.Description  
               },
               UserId = hl.UserId,
               User = new UserDTO
               {
                 Id = hl.User.Id,
                 Username = hl.User.Username,
                 Email = hl.User.Email,
                 DateCreated = hl.User.DateCreated  
               },
               ParanormalActivities = _dbContext.LocationActivities
                .Where(la => la.HauntedLocationId == hl.Id)
                .Select(la => new ParanormalActivityDTO
                {
                   Id = la.ParanormalActivity.Id,
                   Name = la.ParanormalActivity.Name,
                   Description = la.ParanormalActivity.Description,
                   IconName = la.ParanormalActivity.IconName 
                }).ToList()
            })
            .FirstOrDefault();
        
        if (location == null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Update(int id, CreateHauntedLocationDTO dto)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(u => u.IdentityUserId == identityUserId);

        if (userProfile == null)
        {
            return Unauthorized();
        }

        var location = _dbContext.HauntedLocations.FirstOrDefault(hl => hl.Id == id);

        if (location == null)
        {
            return NotFound();
        }

        // Check to see if logged in user owns the location
        if (location.UserId != userProfile.Id)
        {
            return Forbid();
        }

        // update properties
        location.Name = dto.Name;
        location.Address = dto.Address;
        location.City = dto.City;
        location.State = dto.State;
        location.Latitude = dto.Latitude;
        location.Longitude = dto.Longitude;
        location.Description = dto.Description;
        location.History = dto.History;
        location.ImageUrl = dto.ImageUrl;
        location.LocationTypeId = dto.LocationTypeId;
        location.ActivityLevelId = dto.ActivityLevelId;

        _dbContext.SaveChanges();

        // update paranormal activities (remove all and add new)
        var existingActivities = _dbContext.LocationActivities
            .Where(la => la.HauntedLocationId == id)
            .ToList();
        
        _dbContext.LocationActivities.RemoveRange(existingActivities);

        if (dto.ParanormalActivityIds != null && dto.ParanormalActivityIds.Any())
        {
            foreach (var activityId in dto.ParanormalActivityIds)
            {
                _dbContext.LocationActivities.Add(new LocationActivity
                {
                   HauntedLocationId = location.Id,
                   ParanormalActivityId = activityId 
                });
            }
        }
        _dbContext.SaveChanges();

        return Ok(location);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(u => u.IdentityUserId == identityUserId);

        if (userProfile == null)
        {
            return Unauthorized();
        }

        var location = _dbContext.HauntedLocations.FirstOrDefault(hl => hl.Id == id);

        if (location == null)
        {
            return NotFound();
        }

        // Check to see if logged in user owns the location
        if (location.UserId != userProfile.Id)
        {
            return Forbid();
        }

        // remove associated paranormal activites
        var activities = _dbContext.LocationActivities
            .Where(la => la.HauntedLocationId == id)
            .ToList();
        _dbContext.LocationActivities.RemoveRange(activities);

        // remove associated favorites
        var favorites = _dbContext.UserFavorites
            .Where(uf => uf.HauntedLocationId == id)
            .ToList();
        _dbContext.UserFavorites.RemoveRange(favorites);

        // finally, remove location
        _dbContext.HauntedLocations.Remove(location);
        _dbContext.SaveChanges();

        return NoContent();
    }

    [HttpPost]
    [Authorize]
    public IActionResult Create(CreateHauntedLocationDTO dto)
    {
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(u => u.IdentityUserId == identityUserId);

        if (userProfile == null)
        {
            return Unauthorized();
        }

        var location = new HauntedLocation
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Description = dto.Description,
            History = dto.History,
            ImageUrl = dto.ImageUrl,
            LocationTypeId = dto.LocationTypeId,
            ActivityLevelId = dto.ActivityLevelId,
            UserId = userProfile.Id,
            DateAdded = DateTime.UtcNow
        };

        _dbContext.HauntedLocations.Add(location);
        _dbContext.SaveChanges();

        if (dto.ParanormalActivityIds != null && dto.ParanormalActivityIds.Any())
        {
            foreach (var activityId in dto.ParanormalActivityIds)
            {
                _dbContext.LocationActivities.Add(new LocationActivity
                {
                    HauntedLocationId = location.Id,
                    ParanormalActivityId = activityId
                });
            }
            _dbContext.SaveChanges();
        }

        return Created($"/api/hauntedlocations/{location.Id}", location);
    }
}