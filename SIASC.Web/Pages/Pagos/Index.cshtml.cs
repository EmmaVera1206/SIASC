
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Pagos;

[Authorize]
public class IndexModel : PageModel
{
    private readonly SIASCDbContext _context;

    public IndexModel(SIASCDbContext context)
    {
        _context = context;
    }

    public List<PagoResumen> Pagos { get; set; } = new();

    public int TotalRegistros => Pagos.Count;

    public int TotalVigentes =>
        Pagos.Count(p => !p.Anulado);

    public int TotalAnulados =>
        Pagos.Count(p => p.Anulado);

    public long TotalRecibidoCentavos =>
        Pagos
            .Where(p => !p.Anulado)
            .Sum(p => p.MontoCentavos);

    public decimal TotalRecibido =>
        TotalRecibidoCentavos / 100m;

    public class PagoResumen
    {
        public int PagoId { get; set; }

        public int TramiteId { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        public string NombreServicio { get; set; } = string.Empty;

        public long MontoCentavos { get; set; }

        public decimal Monto => MontoCentavos / 100m;

        public DateTime FechaPago { get; set; }

        public string MetodoPago { get; set; } = string.Empty;

        public string TipoPago { get; set; } = string.Empty;

        public string? Referencia { get; set; }

        public string? Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Anulado { get; set; }

        public DateTime? FechaAnulacion { get; set; }

        public string? MotivoAnulacion { get; set; }
    }

    public async Task OnGetAsync()
    {
        Pagos = await _context.Pagos
            .AsNoTracking()
            .Select(p => new PagoResumen
            {
                PagoId = p.PagoId,
                TramiteId = p.TramiteId,
                NombreCliente = p.Tramite.Cliente.NombreCompleto,
                NombreServicio = p.Tramite.TipoServicio.Nombre,
                MontoCentavos = p.MontoCentavos,
                FechaPago = p.FechaPago,
                MetodoPago = p.MetodoPago,
                TipoPago = p.TipoPago,
                Referencia = p.Referencia,
                Observaciones = p.Observaciones,
                FechaRegistro = p.FechaRegistro,
                Anulado = p.Anulado,
                FechaAnulacion = p.FechaAnulacion,
                MotivoAnulacion = p.MotivoAnulacion
            })
            .OrderByDescending(p => p.FechaPago)
            .ThenByDescending(p => p.PagoId)
            .ToListAsync();
    }
}

