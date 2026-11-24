using System.Security.Claims;

namespace SistemaVacaciones.Web.Helpers
{
    public static class ClaimsHelper
    {
        public static int? GetEmpresaId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst("IdEmpresa");
            return claim != null && int.TryParse(claim.Value, out int id) ? id : null;
        }

        public static List<int> GetEmpresas(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == "EmpresaId")
                .Select(c => int.TryParse(c.Value, out int id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();
        }

        public static List<string> GetRoles(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }

        public static List<string> GetRolEmpresa(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == "RolEmpresa")
                .Select(c => c.Value)
                .ToList();
        }

        public static bool TieneAccesoARolYEmpresa(ClaimsPrincipal user, string rol, int idEmpresa)
        {
            var valor = $"{rol}:{idEmpresa}";
            return user.Claims.Any(c => c.Type == "RolEmpresa" && c.Value == valor);
        }

        public static bool TieneAccesoARolYSubEmpresa(ClaimsPrincipal user, string rol, int idEmpresamadre)
        {
            var valor = $"{rol}:{idEmpresamadre}";
            return user.Claims.Any(c => c.Type == "RolSubEmpresa" && c.Value == valor);
        }
    }
}
