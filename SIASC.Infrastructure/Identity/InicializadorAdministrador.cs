
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Constants;

namespace SIASC.Infrastructure.Identity;

public static class InicializadorAdministrador
{
    public static async Task InicializarAsync(
        UserManager<UsuarioSistema> userManager)
    {
        bool existeAdministrador = await userManager
            .GetUsersInRoleAsync(RolesSistema.Administrador)
            .ContinueWith(t => t.Result.Any());

        if (existeAdministrador)
        {
            return;
        }

        string? correo = Environment.GetEnvironmentVariable(
            "SIASC_ADMIN_EMAIL");

        string? contrasena = Environment.GetEnvironmentVariable(
            "SIASC_ADMIN_PASSWORD");

        if (string.IsNullOrWhiteSpace(correo) ||
            string.IsNullOrWhiteSpace(contrasena))
        {
            throw new InvalidOperationException(
                "No existe un administrador. Configura temporalmente " +
                "SIASC_ADMIN_EMAIL y SIASC_ADMIN_PASSWORD para crear uno.");
        }

        correo = correo.Trim();

        var usuarioExistente = await userManager.FindByEmailAsync(correo);

        if (usuarioExistente != null)
        {
            throw new InvalidOperationException(
                "El correo proporcionado ya pertenece a otro usuario.");
        }

        var administrador = new UsuarioSistema
        {
            UserName = correo,
            Email = correo,
            EmailConfirmed = true,
            NombreCompleto = "Administrador SIASC",
            Activo = true,
            FechaRegistro = DateTime.UtcNow
        };

        var resultado = await userManager.CreateAsync(
            administrador,
            contrasena);

        if (!resultado.Succeeded)
        {
            string errores = string.Join(
                "; ",
                resultado.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"No se pudo crear el administrador: {errores}");
        }

        var resultadoRol = await userManager.AddToRoleAsync(
            administrador,
            RolesSistema.Administrador);

        if (!resultadoRol.Succeeded)
        {
            string errores = string.Join(
                "; ",
                resultadoRol.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"No se pudo asignar el rol Administrador: {errores}");
        }
    }
}
