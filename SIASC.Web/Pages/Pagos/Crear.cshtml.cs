
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Pagos;

[Authorize]
public class CrearModel : PageModel
{
    private readonly SIASCDbContext _context;

    public CrearModel(SIASCDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DatosPago Datos { get; set; } = new();

    public List<OpcionTramite> Tramites { get; set; } = new();

    public string? NombreTramite { get; set; }

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

    public class DatosPago
    {
        [Required(ErrorMessage = "Selecciona un trámite.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Selecciona un trámite válido.")]
        public int? TramiteId { get; set; }

        [Required(ErrorMessage = "Ingresa el importe.")]
        [Range(typeof(decimal), "0.01", "999999999999.99",
            ErrorMessage = "El importe debe ser mayor que cero.")]
        public decimal? Monto { get; set; }

        [Required(ErrorMessage = "Selecciona la fecha del pago.")]
        [DataType(DataType.Date)]
        public DateTime? FechaPago { get; set; }

        [Required(ErrorMessage = "Selecciona el método de pago.")]
        public string MetodoPago { get; set; } = "Efectivo";

        [Required(ErrorMessage = "Selecciona el tipo de pago.")]
        public string TipoPago { get; set; } = "Abono";

        [StringLength(150,
            ErrorMessage = "La referencia no puede superar 150 caracteres.")]
        public string? Referencia { get; set; }

        [StringLength(1000,
            ErrorMessage = "Las observaciones no pueden superar 1000 caracteres.")]
        public string? Observaciones { get; set; }
    }

    public class OpcionTramite
    {
        public int TramiteId { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        public string NombreServicio { get; set; } = string.Empty;

        public string Descripcion =>
            $"#{TramiteId} - {NombreCliente} - {NombreServicio}";
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        Datos.FechaPago = DateTime.Today;

        await CargarTramitesAsync();

        if (id.HasValue)
        {
            if (!Tramites.Any(t => t.TramiteId == id.Value))
            {
                return NotFound();
            }

            Datos.TramiteId = id.Value;

            await CargarResumenAsync(id.Value);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await CargarTramitesAsync();

        if (Datos.TramiteId.HasValue)
        {
            await CargarResumenAsync(Datos.TramiteId.Value);
        }

        var metodosPermitidos = new[]
        {
            "Efectivo",
            "Transferencia",
            "Tarjeta",
            "Otro"
        };

        var tiposPermitidos = new[]
        {
            "Anticipo",
            "Abono",
            "Liquidacion"
        };

        if (!metodosPermitidos.Contains(Datos.MetodoPago))
        {
            ModelState.AddModelError(
                "Datos.MetodoPago",
                "Selecciona un método de pago válido.");
        }

        if (!tiposPermitidos.Contains(Datos.TipoPago))
        {
            ModelState.AddModelError(
                "Datos.TipoPago",
                "Selecciona un tipo de pago válido.");
        }

        if (Datos.FechaPago.HasValue &&
            Datos.FechaPago.Value.Date > DateTime.Today)
        {
            ModelState.AddModelError(
                "Datos.FechaPago",
                "La fecha del pago no puede ser futura.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var tramiteId = Datos.TramiteId!.Value;

        var tramite = await _context.Tramites
            .FirstOrDefaultAsync(t => t.TramiteId == tramiteId);

        if (tramite == null)
        {
            ModelState.AddModelError(
                "Datos.TramiteId",
                "El trámite seleccionado no existe.");

            return Page();
        }

        long montoCentavos;

        try
        {
            montoCentavos = checked(
                (long)(Datos.Monto!.Value * 100m));
        }
        catch (OverflowException)
        {
            ModelState.AddModelError(
                "Datos.Monto",
                "El importe ingresado es demasiado grande.");

            return Page();
        }

        if (Datos.Monto!.Value * 100m != montoCentavos)
        {
            ModelState.AddModelError(
                "Datos.Monto",
                "El importe solo puede tener dos decimales.");

            return Page();
        }

        if (montoCentavos <= 0)
        {
            ModelState.AddModelError(
                "Datos.Monto",
                "El importe debe ser mayor que cero.");

            return Page();
        }

        var totalPagado = await _context.Pagos
            .Where(p =>
                p.TramiteId == tramiteId &&
                !p.Anulado)
            .SumAsync(p => (long?)p.MontoCentavos) ?? 0L;

        var saldo = tramite.CostoTotalCentavos - totalPagado;

        if (saldo <= 0)
        {
            ModelState.AddModelError(
                "Datos.Monto",
                "Este trámite no tiene saldo pendiente.");

            return Page();
        }

        if (montoCentavos > saldo)
        {
            ModelState.AddModelError(
                "Datos.Monto",
                $"El importe supera el saldo pendiente de {saldo / 100m:C2}.");

            return Page();
        }

        var pago = new Pago
        {
            TramiteId = tramiteId,
            MontoCentavos = montoCentavos,
            FechaPago = Datos.FechaPago!.Value.Date,
            MetodoPago = Datos.MetodoPago,
            TipoPago = Datos.TipoPago,
            Referencia = string.IsNullOrWhiteSpace(Datos.Referencia)
                ? null
                : Datos.Referencia.Trim(),
            Observaciones = string.IsNullOrWhiteSpace(Datos.Observaciones)
                ? null
                : Datos.Observaciones.Trim(),
            FechaRegistro = DateTime.UtcNow,
            Anulado = false
        };

        _context.Pagos.Add(pago);

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            $"Pago de {montoCentavos / 100m:C2} registrado correctamente.";

        return RedirectToPage(
            "/Tramites/Detalle",
            new { id = tramiteId });
    }

    private async Task CargarTramitesAsync()
    {
        Tramites = await _context.Tramites
            .AsNoTracking()
            .OrderByDescending(t => t.TramiteId)
            .Select(t => new OpcionTramite
            {
                TramiteId = t.TramiteId,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre
            })
            .ToListAsync();
    }

    private async Task CargarResumenAsync(int tramiteId)
    {
        var tramite = await _context.Tramites
            .AsNoTracking()
            .Where(t => t.TramiteId == tramiteId)
            .Select(t => new
            {
                t.CostoTotalCentavos,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre
            })
            .FirstOrDefaultAsync();

        if (tramite == null)
        {
            return;
        }

        NombreTramite =
            $"{tramite.NombreCliente} - {tramite.NombreServicio}";

        CostoTotalCentavos = tramite.CostoTotalCentavos;

        TotalPagadoCentavos = await _context.Pagos
            .AsNoTracking()
            .Where(p =>
                p.TramiteId == tramiteId &&
                !p.Anulado)
            .SumAsync(p => (long?)p.MontoCentavos) ?? 0L;
    }
}
