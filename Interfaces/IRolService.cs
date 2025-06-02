using System;
using System.Collections.Generic;
using MiProyectoBackend.Models;

namespace MiProyectoBackend.Interfaces
{
    public interface IRolService
    {
    Rol GetById(Guid id);
    IEnumerable<Rol> GetAll();
    Rol Create(Rol rol);
    void Update(Guid id, Rol rol);
    void Delete(Guid id);
    }
}
