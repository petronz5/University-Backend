using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // serve un token valido per default
public class SubjectController : ControllerBase
{
    private readonly UniversityDbContext _context;

    public SubjectController(UniversityDbContext context)
    {
        _context = context;
    }

    // 1) GET /api/subject
    // Chiunque può vedere la lista delle materie
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var subjects = await _context.Subjects
            .Include(s => s.Degreecourse)
            .ToListAsync();
        return Ok(subjects);
    }

    // 2) GET /api/subject/{id}
    // Chiunque può vedere i dettagli di una materia
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var subject = await _context.Subjects
            .Include(s => s.Degreecourse)
            .FirstOrDefaultAsync(s => s.Subjectid == id);
        if (subject == null) return NotFound();
        return Ok(subject);
    }

    // 3) POST /api/subject
    // Solo Professori o Rettore possono creare una materia
    [HttpPost]
    [Authorize(Policy = "DocenteOrRettore")]
    public async Task<IActionResult> Create([FromBody] Subject subject)
    {
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = subject.Subjectid }, subject);
    }

    // 4) PUT /api/subject/{id}
    // Solo Professori o Rettore possono modificare una materia
    [HttpPut("{id}")]
    [Authorize(Policy = "DocenteOrRettore")]
    public async Task<IActionResult> Update(int id, [FromBody] Subject updated)
    {
        if (id != updated.Subjectid)
            return BadRequest("ID non corrispondente.");

        _context.Entry(updated).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Subjects.AnyAsync(s => s.Subjectid == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // 5) DELETE /api/subject/{id}
    // Solo il Rettore può eliminare una materia
    [HttpDelete("{id}")]
    [Authorize(Policy = "RettoreOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null) return NotFound();

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
