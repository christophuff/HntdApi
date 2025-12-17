using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HntdApi.Data;
using HntdApi.DTOs;
using HntdApi.Models;

namespace HntdApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActivitiesController: ControllerBase
{
    private HntdApiDbContext _dbContext;

    public ActivitiesController(HntdApiDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var activities = _dbContext.ParanormalActivities
            .Select(pa => new ParanormalActivityDTO
            {
                Id = pa.Id,
                Name = pa.Name,
                Description = pa.Description,
                IconName = pa.IconName
            })
            .ToList();
        
        return Ok(activities);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var activity = _dbContext.ParanormalActivities
            .Where(pa => pa.Id == id)
            .Select(pa => new ParanormalActivityDTO
            {
                Id = pa.Id,
                Name = pa.Name,
                Description = pa.Description,
                IconName = pa.IconName
            })
            .FirstOrDefault();
        
        if (activity == null)
        {
            return NotFound();
        }

        return Ok(activity);
    }

    // admin CRUD on activities
    [HttpPost]
    [Authorize]
    public IActionResult Create(ParanormalActivityDTO dto)
    {
        var activity = new ParanormalActivity
        {
            Name = dto.Name,
            Description = dto.Description,
            IconName = dto.IconName
        };

        _dbContext.ParanormalActivities.Add(activity);
        _dbContext.SaveChanges();

        return Created($"/api/activities/{activity.Id}", new ParanormalActivityDTO
        {
           Id = activity.Id,
           Name = activity.Name,
           Description = activity.Description,
           IconName = activity.IconName 
        });
    }

    [HttpPut("{id}")]
    [Authorize]
    public IActionResult Update(int id, ParanormalActivityDTO dto)
    {
        var activity = _dbContext.ParanormalActivities.FirstOrDefault(pa => pa.Id == id);

        if (activity == null)
        {
            return NotFound();
        }

        activity.Name = dto.Name;
        activity.Description = dto.Description;
        activity.IconName = dto.IconName;

        _dbContext.SaveChanges();

        return Ok(new ParanormalActivityDTO
        {
           Id = activity.Id,
           Name = activity.Name,
           Description = activity.Description,
           IconName = activity.IconName 
        });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var activity = _dbContext.ParanormalActivities.FirstOrDefault(pa => pa.Id == id);

        if (activity == null)
        {
            return NotFound();
        }

        // remove from locations first
        var locationActivities = _dbContext.LocationActivities
            .Where(la => la.ParanormalActivityId == id)
            .ToList();
        _dbContext.LocationActivities.RemoveRange(locationActivities);

        _dbContext.ParanormalActivities.Remove(activity);
        _dbContext.SaveChanges();

        return NoContent();
    }
}