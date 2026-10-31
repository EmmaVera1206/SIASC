
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
public class SaldosModel : PageModel
{
    private readonly SIASCDbContext _context;

    public SaldosModel(SIASCDbContext context)
    {
        _context = context;
    }

    public List<SaldoTramite> Saldos { get; set; } = new();

    public int TotalTramites => Saldos.Count;

    public int TotalLiquidados =>
        Saldos.Count(s => s.EstadoPago == "Liquidado");

    public int TotalParciales =>
        Saldos.Count(s => s.EstadoPago == "Parcial");

    public int TotalSinPagos =>
        Saldos.Count(s => s.EstadoPago == "Sin pagos");

    public long TotalCostosCentavos =>
        Saldos.Sum(s => s.CostoTotalCentavos);

    public long TotalPagadoCentavos =>
        Saldos.Sum(s => s.TotalPagadoCentavos);

    public long TotalPendienteCentavos =>
        Saldos.Sum(s => s.SaldoPendienteCentavos);

    public decimal TotalCostos =>
        TotalCostosCentavos / 100m;

    public decimal TotalPagado =>
        TotalPagadoCentavos / 100m;

    public decimal TotalPendiente =>
        TotalPendienteCentavos / 100m;

    public class SaldoTramite
    {
        public int TramiteId { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        public string NombreServicio { get; set; } = string.Empty;

        public string EstadoTramite { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public long CostoTotalCentavos { get; set; }

        public long TotalPagadoCentavos { get; set; }

        public long SaldoPendienteCentavos =>
            Math.Max(0, CostoTotalCentavos - TotalPagadoCentavos);

        public decimal CostoTotal =>
            CostoTotalCentavos / 100m;

        public decimal TotalPagado =>
            TotalPagadoCentavos / 100m;

        public decimal SaldoPendiente =>
            SaldoPendienteCentavos / 100m;

        public string EstadoPago
        {
            get
            {
                if (SaldoPendienteCentavos == 0)
                {
                    return "Liquidado";
                }

                if (TotalPagadoCentavos > 0)
                {
                    return "Parcial";
                }

                return "Sin pagos";
            }
        }

        public decimal PorcentajePagado
        {
            get
            {
                if (CostoTotalCentavos <= 0)
                {
                    return 100m;
                }

                var porcentaje =
                    TotalPagadoCentavos * 100m / CostoTotalCentavos;

                return Math.Min(100m, porcentaje);
            }
        }
    }

    public async Task OnGetAsync()
    {
        var tramites = await _context.Tramites
            .AsNoTracking()
            .Select(t => new
            {
                t.TramiteId,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre,
                EstadoTramite = t.EstadoActual.Nombre,
                t.FechaInicio,
                t.CostoTotalCentavos
            })
            .ToListAsync();

        var pagosPorTramite = await _context.Pagos
            .AsNoTracking()
            .Where(p => !p.Anulado)
            .GroupBy(p => p.TramiteId)
            .Select(g => new
            {
                TramiteId = g.Key,
                TotalPagadoCentavos = g.Sum(p => p.MontoCentavos)
            })
            .ToDictionaryAsync(
                p => p.TramiteId,
                p => p.TotalPagadoCentavos);

        Saldos = tramites
            .Select(t => new SaldoTramite
            {
                TramiteId = t.TramiteId,
                NombreCliente = t.NombreCliente,
                NombreServicio = t.NombreServicio,
                EstadoTramite = t.EstadoTramite,
                FechaInicio = t.FechaInicio,
                CostoTotalCentavos = t.CostoTotalCentavos,
                TotalPagadoCentavos =
                    pagosPorTramite.TryGetValue(
                        t.TramiteId,
                        out var totalPagado)
                        ? totalPagado
                        : 0L
            })
            .OrderByDescending(s => s.SaldoPendienteCentavos)
            .ThenByDescending(s => s.TramiteId)
            .ToList();
    }
}
