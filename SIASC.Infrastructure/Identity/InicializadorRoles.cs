using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Identity;
using SIASC.Domain.Constants;

namespace SIASC.Infrastructure.Identity;

public static class InicializadorRoles
{
    public static async Task InicializarAsync(
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            RolesSistema.Administrador,
            RolesSistema.Operador
        };

        foreach (var nombreRol in roles)
        {
            if (await roleManager.RoleExistsAsync(nombreRol))
                continue;

            var resultado = await roleManager.CreateAsync(
                new IdentityRole(nombreRol));

            if (!resultado.Succeeded)
            {
                var errores = string.Join(
                    "; ",
                    resultado.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"No se pudo crear el rol {nombreRol}: {errores}");
            }
        }
    }
}
