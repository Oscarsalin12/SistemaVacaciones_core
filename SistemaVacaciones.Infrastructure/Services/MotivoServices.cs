using SistemaVacaciones.Core.Entities;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Core.ViewModels;
using SistemaVacaciones.Infrastructure.Data;

namespace SistemaVacaciones.Infrastructure.Services
{
    public class MotivoServices : IMotivoServices
    {
        private readonly SistemaVacacionesDbContext _db;

        public MotivoServices(SistemaVacacionesDbContext db)
        {
            _db = db;
        }

        public List<MotivosAusenciaVM> Listar()
        {
            return _db.MotivosAusencia.Select(x => new MotivosAusenciaVM
            {
                Id = x.Id,
                NombreMotivo = x.NombreMotivo,
                BitDescuentoDias = x.BitDescuentoDias,
                BitDescuentoHoras = x.BitDescuentoHoras
            }).ToList();
        }

        public void Agregar(MotivosAusenciaVM model)
        {
            var motivo = new MotivoAusencia
            {
                NombreMotivo = model.NombreMotivo,
                BitDescuentoDias = model.BitDescuentoDias,
                BitDescuentoHoras = model.BitDescuentoHoras
            };
            _db.MotivosAusencia.Add(motivo);
            _db.SaveChanges();
        }

        public void Editar(MotivosAusenciaVM model)
        {
            var motivo = _db.MotivosAusencia.Find(model.Id);
            if (motivo != null)
            {
                motivo.NombreMotivo = model.NombreMotivo;
                motivo.BitDescuentoDias = model.BitDescuentoDias;
                motivo.BitDescuentoHoras = model.BitDescuentoHoras;
                _db.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var motivo = _db.MotivosAusencia.Find(id);
            if (motivo != null)
            {
                _db.MotivosAusencia.Remove(motivo);
                _db.SaveChanges();
            }
        }

        public MotivosAusenciaVM? ObtenerPorId(int id)
        {
            var motivo = _db.MotivosAusencia.Find(id);
            if (motivo != null)
            {
                return new MotivosAusenciaVM
                {
                    Id = motivo.Id,
                    NombreMotivo = motivo.NombreMotivo,
                    BitDescuentoDias = motivo.BitDescuentoDias,
                    BitDescuentoHoras = motivo.BitDescuentoHoras
                };
            }
            return null;
        }
    }
}
