using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]  // serve token valido per default
public class ExamRegistrationController : ControllerBase
{
    private readonly UniversityDbContext _context;

    public ExamRegistrationController(UniversityDbContext context)
    {
        _context = context;
    }

    // 1) GET /api/examregistration/session/{sessionId}
    // Solo Professore può vedere chi è iscritto a una sessione
    [HttpGet("session/{sessionId}")]
    [Authorize(Policy = "ProfessoreOnly")]
    public async Task<IActionResult> GetBySession(int sessionId)
    {
        var regs = await _context.Examregistrations
            .Where(r => r.Examsessionid == sessionId)
            .Include(r => r.Student)
                .ThenInclude(s => s.User)
            .ToListAsync();
        return Ok(regs);
    }

    // 2) PUT /api/examregistration/{registrationId}/grade
    // Solo Professore può assegnare o modificare il voto
    [HttpPut("{registrationId}/grade")]
    [Authorize(Policy = "ProfessoreOnly")]
    public async Task<IActionResult> AssignGrade(int registrationId, [FromBody] int grade)
    {
        var reg = await _context.Examregistrations.FindAsync(registrationId);
        if (reg == null) return NotFound();

        reg.Grade = grade;
        reg.Status = "Completed";
        _context.Entry(reg).Property(r => r.Grade).IsModified = true;
        _context.Entry(reg).Property(r => r.Status).IsModified = true;
        await _context.SaveChangesAsync();
        return Ok(reg);
    }
}
