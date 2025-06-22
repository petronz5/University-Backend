using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_University.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // serve token valido per default
public class ProfessorController : ControllerBase
{
    private readonly UniversityDbContext _context;

    public ProfessorController(UniversityDbContext context)
    {
        _context = context;
    }

    // 1) GET /api/professor
    // Chiunque può vedere la lista dei professori
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var profs = await _context.Professors
            .Include(p => p.User)
            .ToListAsync();
        return Ok(profs);
    }

    // 2) GET /api/professor/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var prof = await _context.Professors
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Professorid == id);
        if (prof == null) return NotFound();
        return Ok(prof);
    }

    [HttpGet("registrations/pending")]
    [Authorize(Policy = "ProfessoreOnly")]
    public async Task<IActionResult> GetPendingRegistrations()
    {
        // prendi l’email dalla identity (assume che sia la Name)
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
            return Unauthorized();

        // carica il docente col suo utente
        var prof = await _context.Professors
            .Include(p => p.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.User.Email == email);
        if (prof == null) 
            return Forbid();

        // esami da correggere
        var pending = await _context.Examregistrations
            .Where(r =>
                r.Examsession.Course.Professorid == prof.Professorid &&
                r.Grade == null)
            .Include(r => r.Student).ThenInclude(s => s.User)
            .Include(r => r.Examsession)
                .ThenInclude(es => es.Course)
                    .ThenInclude(c => c.Subject)
            .ToListAsync();

        return Ok(pending);
    }


    [HttpGet("mycourses")]
    [Authorize(Policy = "ProfessoreOnly")]
    public async Task<IActionResult> GetMyCourses()
    {
        var email = User.Identity!.Name!;

        var list = await _context.Courses
            .Include(c => c.Subject)                   // materia
                .ThenInclude(s => s.Degreecourse)      // ← qui
            .Where(c => c.Professor.User.Email == email)
            .ToListAsync();

        return Ok(list);          // restituisce Course con Subject.Degreecourse valorizzato
    }


    // ========== Crea una sessione d’esame per un corso ==========
    [HttpPost("courses/{courseId}/examsessions")]
    [Authorize(Policy = "ProfessoreOnly")]
    public async Task<IActionResult> CreateSession(int courseId,
                                [FromBody] Examsession ses)
    {
        var email  = User.Identity!.Name!;
        var course = await _context.Courses
            .Include(c => c.Professor).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.Courseid == courseId &&
                                    c.Professor.User.Email == email);
        if (course is null) return Forbid();

        // campi minimi
        ses.Courseid = courseId;
        ses.Isactive = true;
        _context.Examsessions.Add(ses);
        await _context.SaveChangesAsync();

        return Created("", ses);
    }

    // 3) POST /api/professor
    // Solo Rettore può creare un nuovo docente
    [HttpPost]
    [Authorize(Policy = "RettoreOnly")]
    public async Task<IActionResult> Create([FromBody] Professor professor)
    {
        _context.Professors.Add(professor);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = professor.Professorid }, professor);
    }

    // 4) PUT /api/professor/{id}
    // Solo Rettore può modificare un docente
    [HttpPut("{id}")]
    [Authorize(Policy = "RettoreOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] Professor updated)
    {
        if (id != updated.Professorid)
            return BadRequest("ID non corrispondente.");

        _context.Entry(updated).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Professors.AnyAsync(p => p.Professorid == id))
                return NotFound();
            throw;
        }

        return NoContent();
    }


    [HttpPut("registrations/{registrationId}/grade")]
    [Authorize(Policy = "ProfessoreOnly")]
    public async Task<IActionResult> SetGrade(int registrationId,
                                            [FromBody] int grade)
    {
        var reg = await _context.Examregistrations
                                .FirstOrDefaultAsync(r => r.Registrationid == registrationId);
        if (reg == null) return NotFound();

        reg.Grade = grade;
        reg.Status = "Passed";
        await _context.SaveChangesAsync();
        return NoContent();
    }


    // 5) DELETE /api/professor/{id}
    // Solo Rettore può eliminare un docente
    [HttpDelete("{id}")]
    [Authorize(Policy = "RettoreOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var prof = await _context.Professors.FindAsync(id);
        if (prof == null) return NotFound();

        _context.Professors.Remove(prof);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
