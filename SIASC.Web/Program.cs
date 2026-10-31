
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;
using SIASC.Infrastructure.Identity;
using SIASC.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Error");
    options.Conventions.AllowAnonymousToPage(
        "/Cuenta/IniciarSesion");
    options.Conventions.AllowAnonymousToPage(
        "/Cuenta/AccesoDenegado");
});

var databasePath = Path.Combine(
    builder.Environment.ContentRootPath,
    "App_Data",
    "siasc.db");

Directory.CreateDirectory(
    Path.GetDirectoryName(databasePath)!);

builder.Services.AddDbContext<SIASCDbContext>(options =>
{
    options.UseSqlite($"Data Source={databasePath}");
});

var directorioDocumentos = Path.Combine(
    builder.Environment.ContentRootPath,
    "App_Data",
    "Documentos");

Directory.CreateDirectory(directorioDocumentos);

builder.Services.AddSingleton(
    new AlmacenamientoDocumentosService(
        directorioDocumentos));

builder.Services
    .AddIdentity<UsuarioSistema, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        options.Lockout.MaxFailedAccessAttempts = 5;

        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<SIASCDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/IniciarSesion";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;

    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<SIASCDbContext>();

    await InicializadorDatos.InicializarAsync(context);

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    await InicializadorRoles.InicializarAsync(roleManager);

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<UsuarioSistema>>();

    await InicializadorAdministrador.InicializarAsync(userManager);
}

app.Run();
