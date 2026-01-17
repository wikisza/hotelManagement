using Microsoft.AspNetCore.Authorization;

namespace hotelASP.Authorization
{
    /// <summary>
    /// Atrybut do dekorowania kontrolerów/akcji wymaganiem konkretnego uprawnienia
    /// </summary>
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permissionCode)
        {
            Policy = $"Permission.{permissionCode}";
        }
    }
}