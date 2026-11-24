    using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.Services
{
    public class DepartamentoServices : IDepartamentosServices
    {
        private readonly Vacaciones_ProyectoEntities db = null;

        public DepartamentoServices()
        {
            this.db = new Vacaciones_ProyectoEntities();
        }
        public List<DepartamentosVM> Listar()
        {
            return this.db.Departamentos
                .OrderBy(d => d.NombreDep)
                .Select(d => new DepartamentosVM
                {
                    Id = d.Id,
                    ClaveDep = d.ClaveDep,
                    NombreDep = d.NombreDep,
                    NombreEmcargado = d.NombreEmcargado
                }).ToList();

        }
        public DepartamentosVM ObtenerPorId(int id)
        {
            var dep = this.db.Departamentos.Find(id);
            if (dep != null)
            {
                return new DepartamentosVM
                {
                    Id = dep.Id,
                    ClaveDep = dep.ClaveDep,
                    NombreDep = dep.NombreDep,
                    NombreEmcargado = dep.NombreEmcargado
                };
            }
            return null;

        }


        public void Agregar(DepartamentosVM departamento)
        {
            var dep = new Departamentos
            {
                ClaveDep = departamento.ClaveDep,
                NombreDep = departamento.NombreDep,
                NombreEmcargado = departamento.NombreEmcargado
            };
            this.db.Departamentos.Add(dep);
            this.db.SaveChanges();
        }

        public void Editar(DepartamentosVM DepNuevo)
        {
            var dep = db.Departamentos.Find(DepNuevo.Id);
            if (dep != null)
            {
                dep.NombreDep = DepNuevo.NombreDep;
                dep.ClaveDep = DepNuevo.ClaveDep;
                dep.NombreEmcargado = DepNuevo.NombreEmcargado;
                db.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var dep = db.Departamentos.Find(id);
            if (dep != null)
            {
                db.Departamentos.Remove(dep);
                db.SaveChanges();
            }
        }
    }
}