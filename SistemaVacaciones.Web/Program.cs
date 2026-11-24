using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SistemaVacaciones.Core.Interfaces;
using SistemaVacaciones.Infrastructure.Data;
using SistemaVacaciones.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Context (Entity Framework Core Code-First)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SistemaVacacionesDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("SistemaVacaciones.Infrastructure")));

// Register Application Services
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IDepartamentosServices, DepartamentoServices>();
builder.Services.AddScoped<IEmpleadosServices, EmpleadoServices>();
builder.Services.AddScoped<IEmpresasService, EmpresasService>();
builder.Services.AddScoped<IMotivoServices, MotivoServices>();
builder.Services.AddScoped<IPermisosServices, PermisosService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();

// Session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/LogOut";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
