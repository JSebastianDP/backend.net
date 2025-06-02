using System;
using System.Collections.Generic;
using System.Linq;
using MiProyectoBackend.Data;
using MiProyectoBackend.Interfaces;
using MiProyectoBackend.Models;

namespace MiProyectoBackend.Services
{
    public class RolService : IRolService
    {
        private readonly ApplicationDbContext _context;

        public RolService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Rol GetById(Guid id)
        {
            return _context.Rol.Find(id);
        }

        public IEnumerable<Rol> GetAll()
        {
            return _context.Rol.ToList();
        }

        public Rol Create(Rol rol)
        {
            if (rol == null)
                throw new ArgumentNullException(nameof(rol));

            if (string.IsNullOrEmpty(rol.Nombre))
                throw new ArgumentException("El nombre del rol es requerido");

            if (_context.Rol.Any(r => r.Nombre == rol.Nombre))
                throw new InvalidOperationException("Ya existe un rol con ese nombre");

            rol.Id = Guid.NewGuid();
            _context.Rol.Add(rol);
            _context.SaveChanges();

            return rol;
        }

        public void Update(Guid id, Rol rolParam)
        {
            var rol = _context.Rol.Find(id);
            if (rol == null)
                throw new KeyNotFoundException("Rol no encontrado");

            if (_context.Rol.Any(r => r.Nombre == rolParam.Nombre && r.Id != id))
                throw new InvalidOperationException("El nombre del rol ya está en uso");

            rol.Nombre = rolParam.Nombre;

            _context.Rol.Update(rol);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var rol = _context.Rol.Find(id);
            if (rol != null)
            {
                _context.Rol.Remove(rol);
                _context.SaveChanges();
            }
        }
    }
}
