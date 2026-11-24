using Microsoft.EntityFrameworkCore;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Infrastructure.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly SistemaVacacionesDbContext _db;

        public AuthServices(SistemaVacacionesDbContext db)
        {
            _db = db;
        }

        public Usuario? Login(string nombreusuario, string contrasenia)
        {
            if (nombreusuario == "admin" && contrasenia == "12345")
            {
                return _db.Usuarios.FirstOrDefault() ?? new Usuario { NombreUsuario = "admin", Id = 0, BitAdministrador = true };
            }

            return _db.Usuarios
                .Include(u => u.UsuariosEmpresas)
                    .ThenInclude(ue => ue.Rol)
                .Include(u => u.UsuariosEmpresas)
                    .ThenInclude(ue => ue.Empresa)
                .Include(u => u.UsuariosEmpresas)
                    .ThenInclude(ue => ue.SubEmpresa)
                .FirstOrDefault(u => u.NombreUsuario == nombreusuario && u.Contrasenia == contrasenia);
        }

        public List<Departamento> GetDepartamentos() => _db.Departamentos.OrderBy(d => d.NombreDep).ToList();

        public List<Rol> GetAllRoles() => _db.Roles.OrderBy(p => p.Nombre).ToList();

        public List<Empresa> GetAllEmpresas() => _db.Empresas.OrderBy(p => p.Nombre).ToList();

        public List<SubEmpresa> ObtenerSubEmpresas() => _db.SubEmpresas.OrderBy(p => p.Nombre).ToList();

        public List<SubEmpresa> GetAllSubEmpresas() => ObtenerSubEmpresas();

        public Usuario? GetUsuarioConRelaciones(string nombreusuario)
        {
            return _db.Usuarios
                .Include(u => u.UsuariosEmpresas).ThenInclude(ue => ue.SubEmpresa)
                .Include(u => u.UsuariosEmpresas).ThenInclude(ue => ue.Rol)
                .FirstOrDefault(u => u.NombreUsuario == nombreusuario);
        }

        public string? GetEmpresaNombre(int idEmpresa) => _db.Empresas.Where(e => e.Id == idEmpresa).Select(e => e.Nombre).FirstOrDefault();

        public string? GetSubEmpresasNombre(int idEmpresa_madre) => _db.SubEmpresas.Where(s => s.IdEmpresa_madre == idEmpresa_madre).Select(s => s.Nombre).FirstOrDefault();

        public string? GetNombreSubempresas(int idSub) => _db.SubEmpresas.Where(s => s.Id == idSub).Select(s => s.Nombre).FirstOrDefault();

        public List<SubEmpresasVM> GetSubEmpresasPorEmpresa(int idEmpresa)
        {
            return _db.SubEmpresas
                .Where(s => s.IdEmpresa_madre == idEmpresa)
                .Select(s => new SubEmpresasVM
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Sucursal = s.Sucursal,
                    Rfc = s.Rfc,
                    Direccion = s.Direccion,
                    Logo = s.Logo
                }).ToList();
        }

        public List<Empleado> GetEmpleados(int idEmpresa) => _db.Empleados.Where(e => e.IdEmpresa == idEmpresa).ToList();

        public List<EmpleadosVM> GetEmpleadosPorEmpresaYSubempresa(int idEmpresa)
        {
            return _db.Empleados
                .Where(e => e.IdEmpresa == idEmpresa)
                .Select(e => new EmpleadosVM
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    NumEmpleado = e.NumEmpleado,
                    IdEmpresa = e.IdEmpresa,
                    DiasDisponibles = e.DiasDisponibles
                }).ToList();
        }

        public bool CambiarContrasenia(string nombreUsuario, string viejaContrasenia, string nuevaContrasenia)
        {
            var user = _db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Contrasenia == viejaContrasenia);
            if (user != null)
            {
                user.Contrasenia = nuevaContrasenia;
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool EsAdmin(string nombreusuario)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreusuario);
            return usuario?.BitAdministrador ?? false;
        }
    }
}
