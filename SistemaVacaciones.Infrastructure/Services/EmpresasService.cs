using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Infrastructure.Services
{
    public class EmpresasService : IEmpresasService
    {
        private readonly SistemaVacacionesDbContext _db;

        public EmpresasService(SistemaVacacionesDbContext db)
        {
            _db = db;
        }

        public void InsertarEmpresa(Empresa empresa, byte[]? logoBytes)
        {
            if (logoBytes != null && logoBytes.Length > 0)
            {
                empresa.Logo = logoBytes;
            }
            _db.Empresas.Add(empresa);
            _db.SaveChanges();
        }

        public void InsertarSubEmpresas(SubEmpresa subEmpresa, byte[]? logoBytes)
        {
            if (logoBytes != null && logoBytes.Length > 0)
            {
                subEmpresa.Logo = logoBytes;
            }
            _db.SubEmpresas.Add(subEmpresa);
            _db.SaveChanges();
        }
    }
}
