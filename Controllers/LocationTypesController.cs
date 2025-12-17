using Microsoft.AspNetCore.Mvc;
using HntdApi.Data;
using HntdApi.DTOs;

namespace HntdApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationTypesController : ControllerBase
{
    private HntdApiDbContext _dbContext;
    public LocationTypesController(HntdApiDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var types = _dbContext.LocationTypes
            .Select(lt => new LocationTypeDTO
            {
                Id = lt.Id,
                Name = lt.Name
            }).ToList();

        return Ok(types);
    }
}