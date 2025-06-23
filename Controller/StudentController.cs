using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_University.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentController : ControllerBase
{
    private readonly UniversityDbContext _context;

    public StudentController(UniversityDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Policy = "DocenteOrRettore")]
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await _context.Students
            .Include(s => s.User)
            .ToListAsync();
        return Ok(students);
    }


    [HttpGet("{id}")]
    [Authorize]
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
    [Authorize(Policy = "DocenteOrRettore")]
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
    [Authorize(Policy = "StudenteOnly")]
    public async Task<IActionResult> GetStudentGrades(int id)
    {
        var grades = await _context.Studentgrades
            .Where(g => g.Studentid == id)
            .ToListAsync();

        return Ok(grades);
    }

    //4. Corsi a cui è iscritto lo studente
    [HttpGet("{id}/courses")]
    [Authorize(Policy = "StudenteOnly")]
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
    [Authorize(Policy = "StudenteOnly")]
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
    [Authorize(Policy = "StudenteOnly")]
    public async Task<IActionResult> GetAvailableExams(int id)
    {
        // 1) tutti i subject già superati dallo studente
        var passedSubjects = await _context.Studentgrades
            .Where(g => g.Studentid == id && g.Grade != null)
            .Select(g => g.Subject)
            .ToListAsync();

        // 2) tutti gli esami già registrati
        var registeredIds = await _context.Examregistrations
            .Where(r => r.Studentid == id)
            .Select(r => r.Examsessionid)
            .ToListAsync();

        // 3) il corso di laurea dello studente
        var degreeCourseId = await _context.Students
            .Where(s => s.Studentid == id)
            .Select(s => s.Degreecourseid)
            .FirstOrDefaultAsync();

        // 4) sessioni attive, non registrate, nel suo corso di laurea e non superate
        var exams = await _context.Examsessions
            .Include(es => es.Course)
                .ThenInclude(c => c.Subject)
            .Where(es =>
                es.Isactive == true &&                                // <— forza bool, non bool?
                !registeredIds.Contains(es.Examsessionid) &&
                es.Course.Subject.Degreecourseid == degreeCourseId &&
                !passedSubjects.Contains(es.Course.Subject.Name)
            )
            .ToListAsync();

        return Ok(exams);
    }


    // 7. Iscrizione lo studente a un esame
    [HttpPost("{id}/registerexam")]
    [Authorize(Policy = "StudenteOnly")]
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
    [Authorize(Policy = "StudenteOnly")]
    public async Task<IActionResult> UnregiterExam(int id, int examSessionId)
    {
        var reg = await _context.Examregistrations
            .FirstOrDefaultAsync(r => r.Studentid == id && r.Examsessionid == examSessionId);
        if (reg == null)
            return NotFound();

        _context.Examregistrations.Remove(reg);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 9. Corsi disponibili per iscrizione
    [HttpGet("{id}/availablecourses")]
    [Authorize(Policy = "StudenteOnly")]
    public async Task<IActionResult> GetAvailableCourses(int id)
    {
        // 1) prendi lo student con degree
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Studentid == id);
        if (student == null)
            return NotFound($"Studente {id} non trovato.");

        // 2) tutti i corsi del suo corso di laurea
        var allCourses = _context.Courses
            .Include(c => c.Subject)
            .Include(c => c.Professor!)
                .ThenInclude(p => p.User)
            .Where(c => c.Subject.Degreecourseid == student.Degreecourseid);

        // 3) escludi quelli già iscritti
        var enrolledCourseIds = await _context.Courseenrollments
            .Where(e => e.Studentid == id)
            .Select(e => e.Courseid)
            .ToListAsync();

        var available = await allCourses
            .Where(c => !enrolledCourseIds.Contains(c.Courseid))
            .Select(c => new
            {
                c.Courseid,
                Subject = c.Subject.Name,
                c.Academicyear,
                c.Semester,
                Professor = c.Professor!.User.Firstname + " " + c.Professor!.User.Lastname
            })
            .ToListAsync();

        return Ok(available);
    }

    // 10. Iscrivi lo studente a un corso (con controlli aggiuntivi)
    [HttpPost("{id}/enrollcourse/{courseid}")]
    [Authorize(Policy = "StudenteOnly")]
    public async Task<IActionResult> EnrollCourse(int id, int courseid)
    {
        // 1) Verifica studente esiste
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Studentid == id);
        if (student == null)
            return NotFound($"Studente {id} non trovato.");

        // 2) Verifica corso esiste e carica la materia
        var course = await _context.Courses
            .Include(c => c.Subject)
            .FirstOrDefaultAsync(c => c.Courseid == courseid);
        if (course == null)
            return NotFound($"Corso {courseid} non trovato.");

        // 3) Controlla che il corso appartenga al suo corso di laurea
        if (course.Subject.Degreecourseid != student.Degreecourseid)
            return BadRequest("Impossibile iscriversi: il corso non appartiene al tuo piano di studi.");

        // 4) Evita doppie iscrizioni (già presente, ma con messaggio più chiaro)
        if (await _context.Courseenrollments
            .AnyAsync(e => e.Studentid == id && e.Courseid == courseid))
        {
            return BadRequest("Sei già iscritto a questo corso.");
        }

        // 5) Crea e salva l’iscrizione
        var enrollment = new Courseenrollment
        {
            Studentid = id,
            Courseid = courseid,
            Enrollmentdate = DateTime.Now
        };
        _context.Courseenrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        // 6) Risposta con dettaglio del corso appena iscritto
        return Ok(new
        {
            enrollment.Enrollmentid,
            enrollment.Courseid,
            Subject = course.Subject.Name,
            course.Academicyear,
            course.Semester
        });
    }


    [HttpGet("{id}/degreecourse")]
    [Authorize(Policy = "StudenteOnly")]
    public async Task<IActionResult> GetCourseDegrees(int id)
    {
        var student = await _context.Students
            .Include(s => s.Degreecourse)
            .FirstOrDefaultAsync(s => s.Studentid == id);
        if (student == null)
            return NotFound();

        return Ok(student.Degreecourse);
    }

    // GET api/student/{id}/grade-analytics
    [HttpGet("{id}/grade-analytics")]
    [Authorize(Policy = "StudenteOnly")]
    public async Task<IActionResult> GetGradeAnalytics(int id)
    {
        var stats = await _context.Studentgrades
                    .Where(g => g.Studentid == id && g.Grade != null)
                    .GroupBy(g => g.Subject)
                    .Select(g => new {
                        Subject = g.Key,
                        Avg     = g.Average(x => x.Grade)
                    })
                    .ToListAsync();

        return Ok(stats);
    }


    [HttpPost]
    [Authorize(Policy = "RettoreOnly")]
    public async Task<IActionResult> CreateStudent([FromBody] Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetStudentById), new { id = student.Studentid }, student);
    }

    // Elimina uno studente
    [HttpDelete("{id}")]
    [Authorize(Policy = "RettoreOnly")]
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
    [Authorize(Policy = "StudenteOnly")]
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


    // 17. Annulla iscrizione a un corso
    [HttpDelete("{id}/unenrollcourse/{courseid}")]
    [Authorize(Policy = "StudenteOnly")]
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
    [Authorize(Policy = "StudenteOnly")]
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