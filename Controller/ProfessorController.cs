using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
