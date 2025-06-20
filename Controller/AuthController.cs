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

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Userroles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Passwordhash == request.Password); // Sostituisci con hash reale!

        if (user == null)
            return Unauthorized();

        var role = user.Userroles.FirstOrDefault()?.Role?.Rolename ?? "Utente";

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            role = role
        });
    }


    // DTO per i dati di registrazione
    public record RegisterStudentRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password,
        int    DegreeCourseId);

    [AllowAnonymous]
    [HttpPost("register-student")]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequest dto)
    {
        // 1) e-mail unica
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email già registrata.");

        // 2) crea User con password hashata
        var user = new User
        {
            Firstname    = dto.FirstName,
            Lastname     = dto.LastName,
            Email        = dto.Email,
            Passwordhash = BCrypt.Net.BCrypt.HashPassword(dto.Password) // ⇒  dotnet add package BCrypt.Net-Next
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();           // serve user.Userid

        // 3) trova/crea ruolo  "Studente"
        var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.Rolename == "Studente");
        if (studentRole is null)
        {
            studentRole = new Role { Rolename = "Studente" };
            _context.Roles.Add(studentRole);
            await _context.SaveChangesAsync();
        }
        _context.Userroles.Add(new Userrole { Userid = user.Userid, Roleid = studentRole.Roleid });

        // 4) crea record Student
        var student = new Student
        {
            Userid          = user.Userid,
            Degreecourseid  = dto.DegreeCourseId,
            Enrollmentdate   = DateOnly.FromDateTime(DateTime.UtcNow),
            Enrollmentnumber = Guid.NewGuid().ToString("N")[..8].ToUpper()
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // 5) genera token identico a quello del login
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, "Studente")
        };
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            role  = "Studente",
            studentId = student.Studentid,
            enrollmentNumber = student.Enrollmentnumber
        });
    }

    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }