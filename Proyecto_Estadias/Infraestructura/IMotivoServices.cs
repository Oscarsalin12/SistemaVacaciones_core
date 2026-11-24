using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.Infraestructura
{
    public interface IMotivoServices
    {
        void Agregar(MotivosAusenciaVM model);
        void Editar(MotivosAusenciaVM model);
        void Eliminar(int id);
        List<MotivosAusenciaVM> Listar();
        MotivosAusenciaVM ObtenerPorId(int id);
    }
}