using Proyecto_Estadias.Infraestructura;
using Proyecto_Estadias.Models;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.Services
{
    public class MotivoServices : IMotivoServices
    {
        private readonly Vacaciones_ProyectoEntities db = null;
        public MotivoServices()
        {
            db = new Vacaciones_ProyectoEntities();
        }

        public List<MotivosAusenciaVM> Listar()
        {
            return db.MotivosAusencia.Select(x => new MotivosAusenciaVM
            {
                Id = x.Id,
                NombreMotivo = x.NombreMotivo,
                BitDescuentoDias = x.BitDescuentoDias,
                BitDescuentoHoras = x.BitDescuentoHoras
            }).ToList();
        }

        public void Agregar(MotivosAusenciaVM model)
        {
            var motivo = new MotivosAusencia
            {
                NombreMotivo = model.NombreMotivo,
                BitDescuentoDias = model.BitDescuentoDias,
                BitDescuentoHoras = model.BitDescuentoHoras
            };
            db.MotivosAusencia.Add(motivo);
            db.SaveChanges();
        }

        public void Editar(MotivosAusenciaVM model)
        {
            var motivo = db.MotivosAusencia.Find(model.Id);
            if (motivo != null)
            {
                motivo.NombreMotivo = model.NombreMotivo;
                motivo.BitDescuentoDias = model.BitDescuentoDias;
                motivo.BitDescuentoHoras = model.BitDescuentoHoras;
                db.SaveChanges();
            }
        }
        public void Eliminar(int id)
        {
            var motivo = db.MotivosAusencia.Find(id);
            if (motivo != null)
            {
                db.MotivosAusencia.Remove(motivo);
                db.SaveChanges();
            }
        }

        public MotivosAusenciaVM ObtenerPorId(int id)
        {
            var motivo = db.MotivosAusencia.Find(id);
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

