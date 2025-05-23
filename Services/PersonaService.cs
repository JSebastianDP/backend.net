using System;
using System.Collections.Generic;
using System.Linq;
using MiProyectoBackend.Data;
using MiProyectoBackend.Interfaces;
using MiProyectoBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MiProyectoBackend.Services
{
    public class PersonaService : IPersonaService
    {
        private readonly ApplicationDbContext _context;

        public PersonaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Persona GetById(Guid id)
        {
            return _context.Persona.Find(id);
        }

        public IEnumerable<Persona> GetAll()
        {
            return _context.Persona.ToList();
        }

        public Persona Create(Persona persona, string password)
        {
            if (_context.Persona.Any(x => x.Email == persona.Email))
                throw new Exception("Email already exists");

            persona.PasswordHash = HashPassword(password);

            _context.Persona.Add(persona);
            _context.SaveChanges();

            return persona;
        }

        public void Update(Guid id, Persona personaParam, string password = null)
        {
            var persona = _context.Persona.Find(id);
            if (persona == null) throw new Exception("Persona no encontrada");

            persona.Nombre = personaParam.Nombre;
            persona.Apellido = personaParam.Apellido;
            persona.Email = personaParam.Email;
            persona.Rol = personaParam.Rol;

            if (!string.IsNullOrEmpty(password))
            {
                persona.PasswordHash = HashPassword(password);
            }

            _context.Persona.Update(persona);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var persona = _context.Persona.Find(id);
            if (persona != null)
            {
                _context.Persona.Remove(persona);
                _context.SaveChanges();
            }
        }

        public Persona ValidateUser(string email, string password)
        {
            var persona = _context.Persona.SingleOrDefault(x => x.Email == email);

            if (persona == null) return null;

            if (VerifyPassword(password, persona.PasswordHash))
                return persona;

            return null;
        }

        // Hashing password simple (puedes usar librerías como BCrypt)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}
