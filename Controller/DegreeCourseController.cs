namespace Backend_University.Controller;

using Backend_University.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "DocenteOrRettore")]
public class DegreeCourseController : Controller
{
    private readonly UniversityDbContext _context;
    public DegreeCourseController(UniversityDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.Degreecourses
                             .Select(dc => new { dc.Degreecourseid, dc.Name })
                             .OrderBy(dc => dc.Name)
                             .ToListAsync();
        return Ok(list);
    }
}