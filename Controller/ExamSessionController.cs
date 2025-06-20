using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // richiede token valido per default
public class ExamSessionController : ControllerBase
{
    private readonly UniversityDbContext _context;

    public ExamSessionController(UniversityDbContext context)
    {
        _context = context;
    }

    // 1) GET /api/examsession
    // Chiunque (anche anonimo) può vedere tutte le sessioni
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var sessions = await _context.Examsessions
            .Include(es => es.Course)
                .ThenInclude(c => c.Subject)
            .Include(es => es.Course)
                .ThenInclude(c => c.Professor)
                    .ThenInclude(p => p.User)
            .ToListAsync();
        return Ok(sessions);
    }

    // 2) GET /api/examsession/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var session = await _context.Examsessions
            .Include(es => es.Course)
                .ThenInclude(c => c.Subject)
            .Include(es => es.Course)
                .ThenInclude(c => c.Professor)
                    .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(es => es.Examsessionid == id);
        if (session == null) return NotFound();
        return Ok(session);
    }

    // 3) GET /api/examsession/course/{courseId}
    [HttpGet("course/{courseId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var sessions = await _context.Examsessions
            .Where(es => es.Courseid == courseId)
            .Include(es => es.Course)
                .ThenInclude(c => c.Subject)
            .Include(es => es.Course)
                .ThenInclude(c => c.Professor)
                    .ThenInclude(p => p.User)
            .ToListAsync();
        return Ok(sessions);
    }

    // 4) POST /api/examsession
    // Solo Professore può creare nuove sessioni
    [HttpPost]
    [Authorize(Policy = "ProfessoreOnly")]
    public async Task<IActionResult> Create([FromBody] Examsession session)
    {
        _context.Examsessions.Add(session);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = session.Examsessionid }, session);
    }

    // 5) PUT /api/examsession/{id}
    // Solo Professore può modificare le proprie sessioni
    [HttpPut("{id}")]
    [Authorize(Policy = "ProfessoreOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] Examsession updated)
    {
        if (id != updated.Examsessionid)
            return BadRequest("ID non corrispondente.");

        _context.Entry(updated).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Examsessions.AnyAsync(es => es.Examsessionid == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // 6) DELETE /api/examsession/{id}
    // Professore o Rettore possono eliminare una sessione
    [HttpDelete("{id}")]
    [Authorize(Policy = "DocenteOrRettore")]
    public async Task<IActionResult> Delete(int id)
    {
        var session = await _context.Examsessions.FindAsync(id);
        if (session == null) return NotFound();

        _context.Examsessions.Remove(session);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
