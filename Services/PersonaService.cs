using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MiProyectoBackend.Data;
using MiProyectoBackend.Interfaces;
using MiProyectoBackend.Models;
using MiProyectoBackend.Helpers;

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
            return _context.Persona
                .Include(p => p.Rol)
                .SingleOrDefault(p => p.Id == id);
        }

        public IEnumerable<Persona> GetAll()
        {
            return _context.Persona
                .Include(p => p.Rol)
                .ToList();
        }

        public Persona Create(Persona persona, string password)
        {
            if (persona == null)
                throw new ArgumentNullException(nameof(persona));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña es requerida", nameof(password));

            if (_context.Persona.Any(x => x.Email == persona.Email))
                throw new InvalidOperationException("El email ya existe");

            // Validar que el RolId existe en la tabla Rol
            if (!_context.Rol.Any(r => r.Id == persona.RolId))
                throw new InvalidOperationException("El RolId no es válido");

            // Usa PasswordHelper para hashear
            persona.PasswordHash = PasswordHelper.HashPassword(password);

            _context.Persona.Add(persona);
            _context.SaveChanges();

            return persona;
        }

        public void Update(Guid id, Persona personaParam, string password = null)
        {
            var persona = _context.Persona.Find(id);
            if (persona == null)
                throw new KeyNotFoundException("Persona no encontrada");

            if (_context.Persona.Any(x => x.Email == personaParam.Email && x.Id != id))
                throw new InvalidOperationException("El email ya está en uso por otra persona");

            // Validar que el RolId es válido antes de actualizar
            if (!_context.Rol.Any(r => r.Id == personaParam.RolId))
                throw new InvalidOperationException("El RolId no es válido");

            persona.Nombre = personaParam.Nombre;
            persona.Apellido = personaParam.Apellido;
            persona.Email = personaParam.Email;
            persona.RolId = personaParam.RolId; // Cambiado para usar RolId

            if (!string.IsNullOrEmpty(password))
            {
                persona.PasswordHash = PasswordHelper.HashPassword(password);
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
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var persona = _context.Persona
                .Include(p => p.Rol)
                .SingleOrDefault(x => x.Email == email);

            if (persona == null)
                return null;

            if (PasswordHelper.VerifyPassword(password, persona.PasswordHash))
                return persona;

            return null;
        }
    }
}
