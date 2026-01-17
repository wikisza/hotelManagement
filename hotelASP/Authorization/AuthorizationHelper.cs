using hotelASP.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace hotelASP.Authorization
{
    /// <summary>
    /// Helper do sprawdzania uprawnieñ w widokach
    /// </summary>
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Sprawdza czy u¿ytkownik ma dane uprawnienie (sprawdza w bazie danych)
        /// </summary>
        public static bool UserHasPermission(ClaimsPrincipal user, string permissionCode, ApplicationDbContext context)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            if (roleClaim == null) return false;

            var hasPermission = context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Any(rp => rp.Role.Name == roleClaim.Value && rp.Permission.Code == permissionCode);

            return hasPermission;
        }

        /// <summary>
        /// Sprawdza czy u¿ytkownik ma któr¹œ z podanych ról
        /// </summary>
        public static bool UserHasRole(ClaimsPrincipal user, params string[] roleNames)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            if (roleClaim == null) return false;

            return roleNames.Contains(roleClaim.Value);
        }

        /// <summary>
        /// Pobiera nazwê roli u¿ytkownika
        /// </summary>
        public static string? GetUserRole(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}