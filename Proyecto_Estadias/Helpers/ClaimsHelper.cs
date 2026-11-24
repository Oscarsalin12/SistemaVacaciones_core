using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Proyecto_Estadias.Helpers
{
    public class ClaimsHelper
    {
        public static int? GetEmpresaId(ClaimsIdentity identity)
        {
            var claim = identity.Claims.FirstOrDefault(c => c.Type == "IdEmpresa");
            return claim != null ? int.Parse(claim.Value) : (int?)null;
        }

        public static List<int> GetEmpresas(ClaimsIdentity identity)
        {
            return identity.Claims
                .Where(c => c.Type == "EmpresaId")
                .Select(c => int.Parse(c.Value))
                .Distinct()
                .ToList();
        }

        public static List<string> GetRoles(ClaimsIdentity identity)
        {
            return identity.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }

        public static List<string> GetRolEmpresa(ClaimsIdentity identity)
        {
            return identity.Claims
                .Where(c => c.Type == "RolEmpresa")
                .Select(c => c.Value)
                .ToList();
        }
        public static bool TieneAccesoARolYEmpresa(ClaimsIdentity identity, string rol, int idEmpresa)
            {
            var valor = $"{rol}:{idEmpresa}";
            return identity.Claims.Any(c => c.Type == "RolEmpresa" && c.Value == valor);
        }

        public static bool TieneAccesoARolYSubEmpresa(ClaimsIdentity identity, string rol, int idEmpresamadre)
        {
            var valor = $"{rol}:{idEmpresamadre}";
            return identity.Claims.Any(c => c.Type == "RolSubEmpresa" && c.Value == valor);
        }

    }
}