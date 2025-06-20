using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly UniversityDbContext _context;
    public CourseController(UniversityDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _context.Courses
            .Include(c => c.Subject)
            .Include(c => c.Professor)
                .ThenInclude(p => p.User)
            .ToListAsync();
        return Ok(courses);
    }



    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Subject)
            .Include(c => c.Professor)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.Courseid == id);
        if (course == null)
            return NotFound();
        return Ok(course);
    }

    [HttpPost]
    [Authorize(Policy = "DocenteOrRettore")]
    public async Task<IActionResult> CreateCourse([FromBody] Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = course.Courseid }, course);
    }


    [HttpPut("{id}")]
    [Authorize(Policy = "DocenteOrRettore")]
    public async Task<IActionResult> Update(int id, [FromBody] Course updated)
    {
        if (id != updated.Courseid)
            return BadRequest("ID non corrispondono");

        _context.Entry(updated).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Courses.AnyAsync(c => c.Courseid == id))
                return NotFound();
            throw;
        }
        return NoContent();
    }


    [HttpDelete("{id}")]
    [Authorize(Policy = "RettoreOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}