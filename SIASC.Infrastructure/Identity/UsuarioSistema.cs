using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.AspNetCore.Identity;

namespace SIASC.Infrastructure.Identity;

public class UsuarioSistema : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}

