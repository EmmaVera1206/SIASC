using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class Cliente
{
    public int ClienteId { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string? CorreoElectronico { get; set; }

    public string? RecomendadoPor { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

    public bool Activo { get; set; } = true;

    public ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
