using System;              // Para Guid y tipos base
using System.ComponentModel.DataAnnotations;  // Para validaciones (opcional)

namespace MiProyectoBackend.Models
{
    public class Rol
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }

    public ICollection<Persona> Personas { get; set; }  // Opcional para navegación inversa
}

}
