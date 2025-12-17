using Microsoft.AspNetCore.Mvc;
using HntdApi.Data;
using HntdApi.DTOs;
using Microsoft.VisualBasic;

namespace HntdApi.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ActivityLevelsController : ControllerBase
{
    private HntdApiDbContext _dbContext;

    public ActivityLevelsController(HntdApiDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var levels = _dbContext.ActivityLevels
            .Select(al => new ActivityLevelDTO
            {
                Id = al.Id,
                Name = al.Name,
                Description = al.Description
            }).ToList();
        
        return Ok(levels);
    }
}