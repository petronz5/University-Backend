namespace Backend_University.Controller;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend_University.Data;
using Backend_University.Models;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UniversityDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(UniversityDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1) recupera utente
        var user = await _context.Users
            .Include(u => u.Userroles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // 2) verifica password
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash))
            return Unauthorized();

        // 3) ruolo principale
        var role = user.Userroles.FirstOrDefault()?.Role?.Rolename ?? "Utente";

        // 4) genera JWT
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, role)
        };
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        // ➊ se è Studente, recupera lo StudentId
        int? studentId = null;
        if (role == "Studente")
        {
            var stud = await _context.Students
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.Userid == user.Userid);
            studentId = stud?.Studentid;
        }

        // 5) ritorna il payload con studentId se esiste
        return Ok(new
        {
            token     = new JwtSecurityTokenHandler().WriteToken(token),
            role,
            studentId
        });
    }


    // DTO per i dati di registrazione
    public record RegisterStudentRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        int    DegreeCourseId);

    //  ▸  AuthController.cs  ▸  dentro la classe AuthController
    [AllowAnonymous]
    [HttpPost("register-student")]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequest dto)
    {
        // 1) e-mail unica ---------------------------------------------------------
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email già registrata.");

        // 2) crea User con password hashata --------------------------------------
        var user = new User
        {
            Firstname    = dto.FirstName,
            Lastname     = dto.LastName,
            Email        = dto.Email,
            Passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();           // ⇒ user.Userid valorizzato

        // 3) Associa il ruolo “Studente” -----------------------------------------
        var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Rolename == "Studente");
        if (studentRole is null)
        {
            studentRole = new Role { Rolename = "Studente" };
            _context.Roles.Add(studentRole);
            await _context.SaveChangesAsync();
        }
        _context.Userroles.Add(new Userrole { Userid = user.Userid, Roleid = studentRole.Roleid });

        // 4) Crea il record Student ----------------------------------------------
        var student = new Student
        {
            Userid           = user.Userid,
            Degreecourseid   = dto.DegreeCourseId,
            Enrollmentdate   = DateOnly.FromDateTime(DateTime.UtcNow),
            Enrollmentnumber = Guid.NewGuid().ToString("N")[..8].ToUpper()
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();           // ⇒ student.Studentid valorizzato

        // 5) ──────────────────────────────────────────────────────────────────────
        //    ISCRIZIONE AUTOMATICA A TUTTI I CORSI DEL CdL
        //    (evita doppioni grazie al vincolo UNIQUE consigliato)
        var courseIds = await _context.Courses
                        .Where(c => c.Subject.Degreecourseid == dto.DegreeCourseId)
                        .Select(c => c.Courseid)
                        .ToListAsync();

        var now   = DateTime.UtcNow;
        var batch = courseIds.Select(cid => new Courseenrollment {
                        Studentid      = student.Studentid,
                        Courseid       = cid,
                        Enrollmentdate = now
                    });
        _context.Courseenrollments.AddRange(batch);
        await _context.SaveChangesAsync();
        //  ────────────────────────────────────────────────────────────────────────

        // 6) Genera JWT identico al login ----------------------------------------
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, "Studente")
        };
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer   : _config["Jwt:Issuer"],
            audience : _config["Jwt:Audience"],
            claims   : claims,
            expires  : DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return Ok(new {
            token            = new JwtSecurityTokenHandler().WriteToken(token),
            role             = "Studente",
            studentId        = student.Studentid,
            enrollmentNumber = student.Enrollmentnumber
        });
    }

    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }