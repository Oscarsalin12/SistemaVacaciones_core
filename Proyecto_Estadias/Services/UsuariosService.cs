using Proyecto_Estadias.Models;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace Proyecto_Estadias.Services
{
    public class UsuariosService
    {
        private readonly Vacaciones_ProyectoEntities db = new Vacaciones_ProyectoEntities();

        // 1. Obtener lista de usuarios con sus accesos
        public List<UsuariosVM> ObtenerTodosLosUsuarios()
        {
            var usuarios = db.Usuarios.ToList();
            var ListaVM = new List<UsuariosVM>();

            foreach (var u in usuarios)
            {
                // 1. Obtenemos los accesos crudos de la BD
                var accesosCrudos = db.UsuariosEmpresas
                    .Where(ue => ue.idUsuario == u.Id && ue.Roles != null && ue.SubEmpresas != null)
                    .Select(ur => new
                    {
                        NombreRol = ur.Roles.Nombre,
                        NombreSub = ur.SubEmpresas.Nombre,
                        Sucursal = ur.SubEmpresas.Sucursal
                    }).ToList();

                // 2. Los agrupamos en memoria para ELIMINAR CUALQUIER DUPLICADO visual
                var accesosUnicos = accesosCrudos
                    .GroupBy(x => new { x.NombreRol, x.NombreSub, x.Sucursal })
                    .Select(g => new UsuariosRolVM
                    {
                        RolesVM = new RolVM { Nombre = g.Key.NombreRol },
                        EmpresaVM = new EmpresaVM { Nombre = g.Key.NombreSub + " (" + g.Key.Sucursal + ")" }
                    }).ToList();

                var user = new UsuariosVM
                {
                    Id = u.Id,
                    Usuario = u.Usuario,
                    Contrasenia = u.Contrasenia,
                    UsuariosRolesVMs = accesosUnicos
                };
                ListaVM.Add(user);
            }
            return ListaVM;
        }
        public UsuariosVM ObtenerUsuariosPorId(int id)
        {
            var u = db.Usuarios.Find(id);
            if (u == null) return null;
            return new UsuariosVM
            {
                Id = u.Id,
                Usuario = u.Usuario,
                Contrasenia = u.Contrasenia,
                BitAdministrador = u.BitAdministrador
            };
        }

        public List<Empresas> ObtenerTodasLasEmpresas()
        {
            return db.Empresas.ToList();
        }

        public int CrearUsuario(string nombreUsuario, string password, bool esAdministrador)
        {
            var nuevoUsuario = new Usuarios
            {
                Usuario = nombreUsuario,
                Contrasenia = password,
                BitAdministrador = esAdministrador
            };
            db.Usuarios.Add(nuevoUsuario);
            db.SaveChanges();
            return nuevoUsuario.Id;
        }

        public void EditarUsuario(UsuariosVM model)
        {
            var u = db.Usuarios.Find(model.Id);
            if (u != null)
            {
                u.Usuario = model.Usuario;
                if (!string.IsNullOrEmpty(model.Contrasenia)){
                    u.Contrasenia = model.Contrasenia;
                }
                u.BitAdministrador = model.BitAdministrador;
                db.SaveChanges();
            }
        }

        public void EliminarUsuario(int id)
        {
            var u = db.Usuarios.Find(id);
            if (u != null)
            {
                // Primero eliminamos los accesos relacionados para evitar errores de clave foránea
                var accesosRelacionados = db.UsuariosEmpresas.Where(ue => ue.idUsuario == id).ToList();
                db.UsuariosEmpresas.RemoveRange(accesosRelacionados);
                db.Usuarios.Remove(u);
                db.SaveChanges();
            }
        }

        // Agrega esto a tu UsuariosService
        public List<SubEmpresas> ObtenerTodasLasSubEmpresas()
        {
            return db.SubEmpresas.ToList();
        }

    
public List<Roles> ObtenerTodosLosRoles()
        {
            return db.Roles.ToList();
        }

        public RolVM ObtenerRolPorId(int id)
        {
            var r = db.Roles.Find(id);
            if (r == null) return null;
            return new RolVM
            {
                Id = r.Id,
                Nombre = r.Nombre
            };
        }

        public int CrearRol(RolVM model)
        {
            var nuevoRol = new Roles
            {
                Nombre = model.Nombre,
             
            };

            db.Roles.Add(nuevoRol);
            db.SaveChanges();
            return nuevoRol.Id;
        }

        public void EditarRol(RolVM model)
        {
            var r = db.Roles.Find(model.Id);
            if (r != null)
            {
                r.Nombre = model.Nombre;
                db.SaveChanges();
            }
        }

        public void EliminarRol(int id)
        {
            var r = db.Roles.Find(id);
            if (r != null)
            {
                // Primero eliminamos los accesos relacionados para evitar errores de clave foránea
                var accesosRelacionados = db.UsuariosEmpresas.Where(ue => ue.idRol == id).ToList();
                db.UsuariosEmpresas.RemoveRange(accesosRelacionados);
                db.Roles.Remove(r);
                db.SaveChanges();
            }
        }


        public void AsignarRolYEmpresas(int idUsuario, int idRol, int idEmpresaMadre, List<int> idSubEmpresas)
        {
            var usuario = db.Usuarios.Find(idUsuario);
            bool esAdmin = usuario != null && usuario.BitAdministrador == true;

            if (esAdmin)
            {
                // Lógica de admin (se queda igual)
                var todosLosAccesos = db.UsuariosEmpresas.Where(ue => ue.idUsuario == idUsuario).ToList();
                db.UsuariosEmpresas.RemoveRange(todosLosAccesos);

                var todasLasSubs = db.SubEmpresas.ToList();
                foreach (var sub in todasLasSubs)
                {
                    db.UsuariosEmpresas.Add(new UsuariosEmpresas
                    {
                        idUsuario = idUsuario,
                        idRol = idRol,
                        idEmpresa = sub.IdEmpresa_madre,
                        idSubEmpresa = sub.Id
                    });
                }
            }
            else
            {
                var accesosPreviosEmpresa = db.UsuariosEmpresas
                    .Where(ue => ue.idUsuario == idUsuario && ue.idEmpresa == idEmpresaMadre)
                    .ToList();

                db.UsuariosEmpresas.RemoveRange(accesosPreviosEmpresa);

                if (idSubEmpresas != null && idSubEmpresas.Any())
                {
                    // NUEVO: Filtramos la lista para asegurar que los IDs sean únicos
                    var subEmpresasUnicas = idSubEmpresas.Distinct().ToList();

                    foreach (var idSub in subEmpresasUnicas)
                    {
                        db.UsuariosEmpresas.Add(new UsuariosEmpresas
                        {
                            idUsuario = idUsuario,
                            idRol = idRol,
                            idEmpresa = idEmpresaMadre,
                            idSubEmpresa = idSub
                        });
                    }
                }
            }

            db.SaveChanges();
        }

        public List<SubEmpresas> ObtenerEmpresasPermitidas(int idUsuario)
        {
            var usuario = db.Usuarios.Find(idUsuario);
            bool esAdmin = usuario != null && usuario.BitAdministrador == true;

            if (esAdmin)
            {
                return db.SubEmpresas.ToList();
            }
            else
            {
                return db.UsuariosEmpresas
                    .Where(ue => ue.idUsuario == idUsuario)
                    .Select(ue => ue.SubEmpresas)
                    .Distinct() // Añadimos esto para evitar que devuelva sucursales repetidas
                    .ToList();
            }
        }


    }
}