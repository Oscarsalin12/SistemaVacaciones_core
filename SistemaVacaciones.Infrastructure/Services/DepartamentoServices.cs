using Microsoft.EntityFrameworkCore;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Infrastructure.Services
{
    public class DepartamentoServices : IDepartamentosServices
    {
        private readonly SistemaVacacionesDbContext _db;

        public DepartamentoServices(SistemaVacacionesDbContext db)
        {
            _db = db;
        }

        public List<DepartamentosVM> Listar()
        {
            return _db.Departamentos.Select(x => new DepartamentosVM
            {
                Id = x.Id,
                ClaveDep = x.ClaveDep,
                NombreDep = x.NombreDep,
                NombreEmcargado = x.NombreEmcargado
            }).ToList();
        }

        public DepartamentosVM? ObtenerPorId(int id)
        {
            var dep = _db.Departamentos.Find(id);
            if (dep == null) return null;
            return new DepartamentosVM
            {
                Id = dep.Id,
                ClaveDep = dep.ClaveDep,
                NombreDep = dep.NombreDep,
                NombreEmcargado = dep.NombreEmcargado
            };
        }

        public void Agregar(DepartamentosVM departamento)
        {
            var dep = new Departamento
            {
                ClaveDep = departamento.ClaveDep,
                NombreDep = departamento.NombreDep,
                NombreEmcargado = departamento.NombreEmcargado
            };
            _db.Departamentos.Add(dep);
            _db.SaveChanges();
        }

        public void Editar(DepartamentosVM depNuevo)
        {
            var dep = _db.Departamentos.Find(depNuevo.Id);
            if (dep != null)
            {
                dep.NombreDep = depNuevo.NombreDep;
                dep.ClaveDep = depNuevo.ClaveDep;
                dep.NombreEmcargado = depNuevo.NombreEmcargado;
                _db.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var dep = _db.Departamentos.Find(id);
            if (dep != null)
            {
                _db.Departamentos.Remove(dep);
                _db.SaveChanges();
            }
        }
    }
}
