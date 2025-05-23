using Microsoft.EntityFrameworkCore;
using MiProyectoBackend.Models;  

namespace MiProyectoBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Aquí defines tus tablas como DbSet:
        public DbSet<Persona> Persona  { get; set; }
    }
}
