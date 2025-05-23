using System;
using System.Collections.Generic;
using MiProyectoBackend.Models;

namespace MiProyectoBackend.Interfaces
{
    public interface IPersonaService
    {
        Persona GetById(Guid id);
        IEnumerable<Persona> GetAll();
        Persona Create(Persona persona, string password);
        void Update(Guid id, Persona persona, string password = null);
        void Delete(Guid id);
        Persona ValidateUser(string email, string password);
    }
}
