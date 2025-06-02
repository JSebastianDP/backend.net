using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MiProyectoBackend.Data;
using MiProyectoBackend.Models;
using MiProyectoBackend.Helpers;
using Microsoft.EntityFrameworkCore; // para Include

namespace MiProyectoBackend.Services
{
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
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Persona
                .Include(p => p.Rol)
                .SingleOrDefault(u => u.Email == email);
            if (user == null)
                return null;

            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
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
                    new Claim(ClaimTypes.Role, user.Rol?.Nombre ?? "User"),
                    new Claim(ClaimTypes.Name, $"{user.Nombre} {user.Apellido}")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Persona Register(Persona user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña es requerida", nameof(password));

            // Verifica si el email ya existe
            if (_context.Persona.Any(x => x.Email == user.Email))
                throw new InvalidOperationException("El email ya está registrado");

            // Usa PasswordHelper para hashear
            user.PasswordHash = PasswordHelper.HashPassword(password);
            
            _context.Persona.Add(user);
            _context.SaveChanges();
            
            return user;
        }
    }
}