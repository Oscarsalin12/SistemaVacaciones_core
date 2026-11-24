using SistemaVacaciones.Core.Entities;

namespace SistemaVacaciones.Core.Interfaces
{
    public interface IEmpleadosServices
    {
        void ActualizarEmpleado(Empleado empleado);
        void ActualizarVacacionesYPermisos();
        IQueryable<Empleado> FiltrarEmpleados(int idSub, string? estado, string? busqueda, int? idDepartamento);
        Empleado? GetEmpleadoPorId(int id);
        void InsertarEmpleados(Empleado emp);
        Empleado? LoginEmpleados(string usuario, int contrasena);
        Empleado? ObtenerEmpleadoPorId(int id);
    }
}
