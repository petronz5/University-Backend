using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Controller;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly UniversityDbContext _context;

    public StudentController(UniversityDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await _context.Students
            .Include(s => s.User)
            .ToListAsync();
        return Ok(students);
    }
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Studentid == id);
        if (student == null)
            return NotFound();
        return Ok(student);
    }
    
    [HttpGet("byEmail")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.User.Email == email);

        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpGet("{id}/grades")]
    public async Task<IActionResult> GetStudentGrades(int id)
    {
        var grades = await _context.Studentgrades
            .Where(g => g.Studentid == id)
            .ToListAsync();

        return Ok(grades);
    }
    
    //4. Corsi a cui è iscritto lo studente
    [HttpGet("{id}/courses")]
    public async Task<IActionResult> GetStudentCourses(int id)
    {
            var courses = await _context.Courseenrollments 
            .Where(e => e.Studentid == id)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Subject)
                    .Include(e => e.Course)
                        .ThenInclude(c => c.Professor)
                            .ThenInclude(p => p.User)
                    .Select(e => e.Course)
                    .ToListAsync();

            return Ok(courses);
    }
    
    // 5. Esami a cui lo studente è iscritto
    [HttpGet("{id}/examregistrations")]
    public async Task<IActionResult> GetStudentExamRegistrations(int id)
    {
            var exams = await _context.Examregistrations
                                 .Where(r => r.Studentid == id)
                    .Include(r => r.Examsession)
                        .ThenInclude(es => es.Course)
                            .ThenInclude(c => c.Subject)
                    .Include(r => r.Examsession)
                        .ThenInclude(es => es.Course)
                            .ThenInclude(c => c.Professor)
                                .ThenInclude(p => p.User)
                    .ToListAsync();
        return Ok(exams);
    }
    
    // 6. Esami disponibili per lo studente
    [HttpGet("{id}/availableexams")]
    public async Task<IActionResult> GetAvailableExams(int id)
    {
        var exams = await _context.Examregistrations
            .Where(r => r.Studentid == id)
            .Include(r => r.Examsession)
            .ThenInclude(es => es.Course)
            .Select(r => r.Examsession)
            .ToListAsync();
        return Ok(exams);
    }
    
    // 7. Iscrizione lo studente a un esame
    [HttpPost("id/registerexam")]
    public async Task<IActionResult> RegisterExam(int id, [FromBody] int examSessionId)
    {
        var registration = new Models.Examregistration
        {
            Studentid = id,
            Examsessionid = examSessionId,
            Registrationdate = DateTime.Now,
            Status = "Registered"
        };
        _context.Examregistrations.Add(registration);
        await _context.SaveChangesAsync();
        return Ok(registration);
    }
    
    // 8. Annulla iscrizione a un mese
    [HttpDelete("{id}/unregisterexam/{examsessionId}")]
    public async Task<IActionResult> UnregiterExam(int id , int examSessionId)
    {
        var reg = await _context.Examregistrations
            .FirstOrDefaultAsync(r => r.Studentid == id && r.Examsessionid == examSessionId);
        if (reg == null)
            return NotFound();  
        
        _context.Examregistrations.Remove(reg);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id}/degreecourse")]
    public async Task<IActionResult> GetCourseDegrees(int id)
    {
        var student = await _context.Students
            .Include(s => s.Degreecourse)
            .FirstOrDefaultAsync(s => s.Studentid == id);
        if (student == null)
            return NotFound();
        
        return Ok(student.Degreecourse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetStudentById), new { id = student.Studentid }, student);
    }
    
    // Elimina uno studente
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    // Materie del corso di laurea
    [HttpGet("{id}/subjects")]
    public async Task<IActionResult> GetDegreeCourseSubjects(int id)
    {
        var student = await _context.Students
            .Include(s => s.Degreecourse)
            .ThenInclude(dc => dc.Subjects)
            .FirstOrDefaultAsync(s => s.Studentid == id);
        if (student == null)
            return NotFound();

        var subjects = await _context.Subjects
            .Where(sub => sub.Degreecourseid == student.Degreecourseid)
            .ToListAsync();
        
        return Ok(subjects);
    }
    
    [HttpPost("{id}/enrollcourse/{courseid}")]
    public async Task<IActionResult> EnrollCourse(int id, int courseid)
    {
        if (await _context.Courseenrollments.AnyAsync(e => e.Studentid == id && e.Courseid == courseid))
            return BadRequest("Già iscritto a questo corso.");
        var enrollment = new Models.Courseenrollment
        {
            Studentid = id,
            Courseid = courseid,
            Enrollmentdate = DateTime.UtcNow
        };
        _context.Courseenrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return Ok(enrollment);
    }
    
    // 17. Annulla iscrizione a un corso
    [HttpDelete("{id}/unenrollcourse/{courseid}")]
    public async Task<IActionResult> UnenrollCourse(int id, int courseid)
    {
        var enrollment = await _context.Courseenrollments
            .FirstOrDefaultAsync(e => e.Studentid == id && e.Courseid == courseid);
        if (enrollment == null)
            return NotFound();
        _context.Courseenrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    

    // 20. Statistiche (media voti, CFU, ecc.)
    [HttpGet("{id}/statistics")]
    public async Task<IActionResult> GetStudentStatistics(int id)
    {
        var grades = await _context.Studentgrades
            .Where(g => g.Studentid == id && g.Grade != null)
            .ToListAsync();

        var avg = grades.Any() ? grades.Average(g => g.Grade) : 0;
        var cfu = await _context.Subjects
            .Where(s => grades.Select(g => g.Subject).Contains(s.Name))
            .SumAsync(s => (int?)s.Credits) ?? 0;

        return Ok(new { Media = avg, cfu = cfu });
    }

    
    
    
}