using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MiProyectoBackend.Data;
using MiProyectoBackend.Models;
using BCrypt.Net; // Importa esta librería para hash

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthService(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public Persona Authenticate(string email, string password)
    {
        var user = _context.Persona.SingleOrDefault(u => u.Email == email);
        if (user == null)
            return null;

        // Verifica si la contraseña coincide con el hash guardado
        bool verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!verified)
            return null;

        return user;
    }

    public string GenerateJwtToken(Persona user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Persona Register(Persona user, string password)
    {
        // Hashea la contraseña antes de guardar
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        _context.Persona.Add(user);
        _context.SaveChanges();
        return user;
    }
}
