using System;
using System.Collections.Generic;
using System.Text;


namespace SIASC.Domain.Entities;

public class HistorialCambioPago
{
    public int CambioPagoId { get; set; }

    public int PagoId { get; set; }

    public string UsuarioId { get; set; } = string.Empty;

    public string TipoCambio { get; set; } = string.Empty;

    public string DatosAnteriores { get; set; } = string.Empty;

    public string? DatosNuevos { get; set; }

    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;

    public string Motivo { get; set; } = string.Empty;

    public Pago Pago { get; set; } = null!;
}
