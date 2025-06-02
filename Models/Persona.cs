using System;              // Para Guid y tipos base
using System.ComponentModel.DataAnnotations;  // Para validaciones (opcional)

namespace MiProyectoBackend.Models
{
    public class Persona
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public Guid RolId { get; set; }  // Foreign Key
        public Rol Rol { get; set; }     // Navigation property


    }
}
