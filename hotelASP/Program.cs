using hotelASP.Authorization;
using hotelASP.Data;
using hotelASP.Interfaces;
using hotelASP.Interfaces.IRoomService;
using hotelASP.Services;
using hotelASP.Services.RoomService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

// ===== KONFIGURACJA AUTORYZACJI OPARTEJ NA UPRAWNIENIACH =====
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    // Rejestracja policy dla ka¿dego kodu uprawnienia
    var permissionCodeFields = typeof(PermissionCodes).GetFields(
        System.Reflection.BindingFlags.Public | 
        System.Reflection.BindingFlags.Static | 
        System.Reflection.BindingFlags.FlattenHierarchy
    );

    foreach (var field in permissionCodeFields)
    {
        if (field.IsLiteral && !field.IsInitOnly)
        {
            var permissionCode = field.GetValue(null)?.ToString();
            if (!string.IsNullOrEmpty(permissionCode))
            {
                options.AddPolicy(
                    $"Permission.{permissionCode}",
                    policy => policy.Requirements.Add(new PermissionRequirement(permissionCode))
                );
            }
        }
    }
});
// =============================================================

string _GetConnStringName = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(_GetConnStringName, ServerVersion.AutoDetect(_GetConnStringName)));

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IBillService, BillsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
