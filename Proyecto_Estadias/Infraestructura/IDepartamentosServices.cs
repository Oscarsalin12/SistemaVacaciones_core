using Proyecto_Estadias.Models;
using Proyecto_Estadias.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_Estadias.Infraestructura
{

    public interface IDepartamentosServices
    {
        void Agregar(DepartamentosVM departamento);
        void Editar(DepartamentosVM DepNuevo);
        void Eliminar(int id);
        List<DepartamentosVM> Listar();
        DepartamentosVM ObtenerPorId(int id);
    }
}