namespace MiProyectoBackend.Models
{
    public class PersonaCreateDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }

    public class PersonaUpdateDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }  // opcional, puede ser null
        public string Rol { get; set; }
    }
}
