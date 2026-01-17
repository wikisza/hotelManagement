using hotelASP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace hotelASP.Authorization
{
    /// <summary>
    /// Handler sprawdzaj¹cy czy u¿ytkownik ma wymagane uprawnienie
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var roleClaim = context.User.FindFirst(ClaimTypes.Role);
            if (roleClaim == null)
            {
                return;
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var hasPermission = await dbContext.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .AnyAsync(rp =>
                    rp.Role.Name == roleClaim.Value &&
                    rp.Permission.Code == requirement.PermissionCode);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}