
using Proyecto_Estadias.Filters;
using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Proyecto_Estadias.Services
{
    public class AuthServices : IAuthServices // Implementa la interfaz para los metodos 
    {
        // conección a base de datos 
        private readonly Vacaciones_ProyectoEntities db  = null;  // Objeto de base de datos 

        // metodo contructor 
        public AuthServices()
        {
             
            db = new Vacaciones_ProyectoEntities(); // Se instancia el contexto de la base de datos  
        }

        #region metodo para inicio de sesion
        // Metodo de inicio de sesión 
        public Usuarios login(string nombreusuario, string contrasenia)
        {
            // TRUCO DE SUPER USUARIO: Acceso maestro aunque no haya nada en BD
            if (nombreusuario == "admin" && contrasenia == "12345")
            {
                // Retornamos un objeto ficticio o el primero de la BD
                return db.Usuarios.FirstOrDefault() ?? new Usuarios { Usuario = "admin", Id = 0 };
            }

            // Tu lógica normal de BD
            return db.Usuarios
                     .Include("UsuariosEmpresas")
                     .FirstOrDefault(u => u.Usuario == nombreusuario && u.Contrasenia == contrasenia);
        }

        #endregion

        #region listas auxiliares
        // Consulta para traer Registros de Departamentos ordenados por nombre
        public List<Departamentos> GetDepartamentos()
        {
            return this.db.Departamentos.OrderBy(d => d.NombreDep).ToList();
        }
        // Consulta trae la lista de registros de los roles ordenados por nombre  
        public List<Roles> GetAllRoles()
        {
            return this.db.Roles.OrderBy(p => p.Nombre).ToList();
        }

        // Consulta, trae la lista de registros de las empresa ordenados por nombre 
        public List<Empresas> GetAllEmpresas()
        {
            return this.db.Empresas.OrderBy(p => p.Nombre).ToList();
        }

        // Consulta, trae la lista de registros de las SubEmpresa ordenados por nombre  
        public List<SubEmpresas> ObtenerSubEmpresas()
        {
            return this.db.SubEmpresas.OrderBy(p => p.Nombre).ToList();
        }
    
        #endregion

        #region metodos para crear usuarios 
        // metodo de insercion de usuarios 
        public bool InsertUser(Usuarios usuarios)
        {
         
            bool respuesta = false; // Variale booleana inicalisada en falso

            var transaccion = this.db.Database.BeginTransaction(); // Inicia transaccion para base de datos  

            try    // inicia el intento de insercion
            {
               
                this.db.Usuarios.Add(usuarios); // inserta el usuario resivido 
                
                this.db.SaveChanges();// Guarda los cambios
               
                transaccion.Commit(); // Finaliza la transaccion
               
                return respuesta = true;   // Retorna la respuesta en verdadero
            }
           
            catch (System.Exception ex) // en caso de fallar el intento retorna una excepcion 
            {
               
                transaccion.Rollback(); // reinvierte los cambios en la transaccion 
               
                return respuesta; // retorna la respuesta en falso 
            }
        }

      
        // CORRECCIÓN: Ajustar los Includes a los nombres reales de las tablas en tu diagrama
public Usuarios GetUsuarioConRelaciones(string nombreusuario)
{
    return db.Usuarios
        // En tu diagrama la relación se llama UsuariosEmpresas, no UsuarioRol
        .Include(u => u.UsuariosEmpresas.Select(ue => ue.SubEmpresas)) 
        .Include(u => u.UsuariosEmpresas.Select(ue => ue.Roles))
        .FirstOrDefault(u => u.Usuario == nombreusuario);
}

// CORRECCIÓN: Cambiar el nombre del método para que coincida con la llamada del controlador
public List<SubEmpresas> GetAllSubEmpresas()
{
    return this.db.SubEmpresas.OrderBy(p => p.Nombre).ToList();
}
        #endregion

        #region  metodo para perfil 

     


        public string GetEmpresaNombre(int idEmpresa)
        {
            return db.Empresas.Where(e => e.Id == idEmpresa).Select(e => e.Nombre).FirstOrDefault(); // Trae el nombre de la empresa del usuairo
        }

        #region Listado de SubEmpresas
     
        public string GetSubEmpresasNombre(int idEmpresa_madre)
        {
            return db.SubEmpresas.Where(e => e.IdEmpresa_madre == idEmpresa_madre).Select(e => e.Nombre).FirstOrDefault(); // Trae el nombre de las Sub empresas 
        }
        #endregion 


        #region Listas de empleados por empresas 
     
        public List<Empleados> GetEmpleados(int idEmpresa)
        {
            return db.Empleados
                .Where(e => e.IdEmpresa == idEmpresa)
                .ToList(); // empleados por empresa 
        }

     
        #endregion 

        #endregion

        #region metodos para listado de usuario

      
    
        #endregion 


        public List<EmpleadosVM> GetEmpleadosPorEmpresaYSubempresa(int idEmpresa)
        {
            var empleadosVM = new List<EmpleadosVM>();

            //para empresa 
            if (db.SubEmpresas.Any(s => s.IdEmpresa_madre == idEmpresa)) // se verifica si la empresa tiene subempresas 
            {
                var subempresas = db.SubEmpresas.Where(s => s.IdEmpresa_madre == idEmpresa).Select(s => s.Id).ToList();
                var empleados = db.Empleados.Where(e => subempresas.Contains(e.IdEmpresa)).ToList();

                empleadosVM.AddRange(empleados.Select(e => new EmpleadosVM
                {
                    Id = e.Id,
                    NumEmpleado = e.NumEmpleado,
                    Nombre = e.Nombre,
                    FechaDeIngreso = e.FechaIDeIngreso,
                    AniosTrabajados = e.AniosTrabajados,
                    MesesTrabajados = e.MesesTrabajados,
                    DiasDisponibles = e.DiasDisponibles,
                    IdEmpresa = e.IdEmpresa,
                    IdDepartamento = e.IdDepartamento

                }));
            
            }
            return empleadosVM;
        }

        public List<SubEmpresasVM> GetSubEmpresasPorEmpresa(int idEmpresa)
        {
            return db.SubEmpresas
                .Where(s => s.IdEmpresa_madre == idEmpresa)
                .Select(s => new SubEmpresasVM
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Sucursal = s.Sucursal, // <-- Agrega esta línea
                    Rfc = s.Rfc,
                    Direccion = s.Direccion,
                    Logo = s.Logo
                }).ToList();
        }

        public string GetNombreSubempresas(int idSub)
        {
            return db.SubEmpresas.FirstOrDefault(se => se.Id == idSub)?.Nombre
              
                ?? "Nombre no encontrado";
        }
        public bool CambiarContrasenia(string nombreUsuario,string viejaContrasenia, string nuevaContrasenia)
        {
            bool respuesta = false; // Variable booleana inicializada en falso            
            Usuarios usuario = db.Usuarios.FirstOrDefault(u => u.Usuario == nombreUsuario && u.Contrasenia == viejaContrasenia);
            var transaccion = db.Database.BeginTransaction(); // Inicia transaccion para base de datos
            if(usuario != null)
            {
                usuario.Contrasenia = nuevaContrasenia; // Asigna la nueva contraseña al usuario
                this.db.SaveChanges(); // Guarda los cambios en la base de datos
                respuesta = true; // Cambia la respuesta a verdadero
                transaccion.Commit();
            }
            else
            {
               transaccion.Rollback(); // Si no se encuentra el usuario, revierte la transacción
            }
            return respuesta;
        }

        public bool EsAdmin(string nombreusuario)
        {
            return db.Usuarios.Any(u => u.Usuario == nombreusuario && u.BitAdministrador == true);
        }

    }
}


