using Microsoft.EntityFrameworkCore;
using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Infrastructure.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly SistemaVacacionesDbContext _db;

        public UsuariosService(SistemaVacacionesDbContext db)
        {
            _db = db;
        }

        public List<Usuario> ListarUsuarios()
        {
            return _db.Usuarios
                .Include(u => u.UsuariosEmpresas)
                    .ThenInclude(ue => ue.Rol)
                .Include(u => u.UsuariosEmpresas)
                    .ThenInclude(ue => ue.Empresa)
                .Include(u => u.UsuariosEmpresas)
                    .ThenInclude(ue => ue.SubEmpresa)
                .ToList();
        }

        public Usuario? ObtenerUsuarioPorId(int id)
        {
            return _db.Usuarios
                .Include(u => u.UsuariosEmpresas)
                .FirstOrDefault(u => u.Id == id);
        }

        public int CrearUsuario(UsuariosVM model)
        {
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = model.Usuario,
                Contrasenia = model.Contrasenia,
                BitAdministrador = model.BitAdministrador
            };

            _db.Usuarios.Add(nuevoUsuario);
            _db.SaveChanges();
            return nuevoUsuario.Id;
        }

        public void EditarUsuario(UsuariosVM model)
        {
            var u = _db.Usuarios.Find(model.Id);
            if (u != null)
            {
                u.NombreUsuario = model.Usuario;
                u.BitAdministrador = model.BitAdministrador;
                if (!string.IsNullOrWhiteSpace(model.Contrasenia))
                {
                    u.Contrasenia = model.Contrasenia;
                }
                _db.SaveChanges();
            }
        }

        public void EliminarUsuario(int id)
        {
            var u = _db.Usuarios.Find(id);
            if (u != null)
            {
                var accesosRelacionados = _db.UsuariosEmpresas.Where(ue => ue.IdUsuario == id).ToList();
                _db.UsuariosEmpresas.RemoveRange(accesosRelacionados);
                _db.Usuarios.Remove(u);
                _db.SaveChanges();
            }
        }

        public List<SubEmpresa> ObtenerTodasLasSubEmpresas() => _db.SubEmpresas.ToList();

        public List<Rol> ObtenerTodosLosRoles() => _db.Roles.ToList();

        public RolVM? ObtenerRolPorId(int id)
        {
            var r = _db.Roles.Find(id);
            if (r == null) return null;
            return new RolVM
            {
                Id = r.Id,
                Nombre = r.Nombre
            };
        }

        public int CrearRol(RolVM model)
        {
            var nuevoRol = new Rol
            {
                Nombre = model.Nombre
            };
            _db.Roles.Add(nuevoRol);
            _db.SaveChanges();
            return nuevoRol.Id;
        }

        public void EditarRol(RolVM model)
        {
            var r = _db.Roles.Find(model.Id);
            if (r != null)
            {
                r.Nombre = model.Nombre;
                _db.SaveChanges();
            }
        }

        public void EliminarRol(int id)
        {
            var r = _db.Roles.Find(id);
            if (r != null)
            {
                var accesosRelacionados = _db.UsuariosEmpresas.Where(ue => ue.IdRol == id).ToList();
                _db.UsuariosEmpresas.RemoveRange(accesosRelacionados);
                _db.Roles.Remove(r);
                _db.SaveChanges();
            }
        }

        public void AsignarRolYEmpresas(int idUsuario, int idRol, int idEmpresaMadre, List<int> idSubEmpresas)
        {
            var usuario = _db.Usuarios.Find(idUsuario);
            bool esAdmin = usuario?.BitAdministrador ?? false;

            if (esAdmin)
            {
                var todosLosAccesos = _db.UsuariosEmpresas.Where(ue => ue.IdUsuario == idUsuario).ToList();
                _db.UsuariosEmpresas.RemoveRange(todosLosAccesos);

                var todasLasSubs = _db.SubEmpresas.ToList();
                foreach (var sub in todasLasSubs)
                {
                    _db.UsuariosEmpresas.Add(new UsuarioEmpresa
                    {
                        IdUsuario = idUsuario,
                        IdRol = idRol,
                        IdEmpresa = sub.IdEmpresa_madre,
                        IdSubEmpresa = sub.Id
                    });
                }
            }
            else
            {
                var accesosPreviosEmpresa = _db.UsuariosEmpresas
                    .Where(ue => ue.IdUsuario == idUsuario && ue.IdEmpresa == idEmpresaMadre)
                    .ToList();

                _db.UsuariosEmpresas.RemoveRange(accesosPreviosEmpresa);

                if (idSubEmpresas != null && idSubEmpresas.Any())
                {
                    var subEmpresasUnicas = idSubEmpresas.Distinct().ToList();

                    foreach (var idSub in subEmpresasUnicas)
                    {
                        _db.UsuariosEmpresas.Add(new UsuarioEmpresa
                        {
                            IdUsuario = idUsuario,
                            IdRol = idRol,
                            IdEmpresa = idEmpresaMadre,
                            IdSubEmpresa = idSub
                        });
                    }
                }
            }

            _db.SaveChanges();
        }

        public List<SubEmpresa> ObtenerEmpresasPermitidas(int idUsuario)
        {
            var usuario = _db.Usuarios.Find(idUsuario);
            bool esAdmin = usuario?.BitAdministrador ?? false;

            if (esAdmin)
            {
                return _db.SubEmpresas.ToList();
            }
            else
            {
                return _db.UsuariosEmpresas
                    .Where(ue => ue.IdUsuario == idUsuario && ue.SubEmpresa != null)
                    .Select(ue => ue.SubEmpresa!)
                    .Distinct()
                    .ToList();
            }
        }
    }
}
