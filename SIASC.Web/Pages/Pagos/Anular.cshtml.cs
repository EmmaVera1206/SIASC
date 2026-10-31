
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;
using SIASC.Infrastructure.Identity;

namespace SIASC.Web.Pages.Pagos;

[Authorize]
public class AnularModel : PageModel
{
    private readonly SIASCDbContext _context;
    private readonly UserManager<UsuarioSistema> _userManager;

    public AnularModel(
        SIASCDbContext context,
        UserManager<UsuarioSistema> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public PagoDetalle? Pago { get; set; }

    [BindProperty]
    public DatosAnulacion Datos { get; set; } = new();

    public class DatosAnulacion
    {
        [Required(ErrorMessage = "Debes indicar el motivo de anulación.")]
        [StringLength(
            1000,
            MinimumLength = 10,
            ErrorMessage = "El motivo debe tener entre 10 y 1000 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }

    public class PagoDetalle
    {
        public int PagoId { get; set; }
        public int TramiteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string NombreServicio { get; set; } = string.Empty;
        public long MontoCentavos { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string TipoPago { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public bool Anulado { get; set; }

        public decimal Monto => MontoCentavos / 100m;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Pago = await ObtenerPagoAsync(id);

        if (Pago == null)
        {
            return NotFound();
        }

        if (Pago.Anulado)
        {
            TempData["MensajeError"] =
                "Este pago ya se encuentra anulado.";

            return RedirectToPage("/Pagos/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Pago = await ObtenerPagoAsync(id);

        if (Pago == null)
        {
            return NotFound();
        }

        if (Pago.Anulado)
        {
            TempData["MensajeError"] =
                "Este pago ya se encuentra anulado.";

            return RedirectToPage("/Pagos/Index");
        }

        Datos.Motivo = Datos.Motivo?.Trim() ?? string.Empty;

        if (Datos.Motivo.Length < 10 ||
            Datos.Motivo.Length > 1000)
        {
            ModelState.AddModelError(
                "Datos.Motivo",
                "El motivo debe tener entre 10 y 1000 caracteres.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var usuario = await _userManager.GetUserAsync(User);

        if (usuario == null || !usuario.Activo)
        {
            return Forbid();
        }

        await using var transaccion =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var pago = await _context.Pagos
                .FirstOrDefaultAsync(p => p.PagoId == id);

            if (pago == null)
            {
                return NotFound();
            }

            if (pago.Anulado)
            {
                TempData["MensajeError"] =
                    "Este pago ya fue anulado por otro proceso.";

                return RedirectToPage("/Pagos/Index");
            }

            var datosAnteriores = JsonSerializer.Serialize(new
            {
                pago.PagoId,
                pago.TramiteId,
                pago.MontoCentavos,
                pago.FechaPago,
                pago.MetodoPago,
                pago.TipoPago,
                pago.Referencia,
                pago.Observaciones,
                pago.FechaRegistro,
                pago.Anulado,
                pago.FechaAnulacion,
                pago.MotivoAnulacion
            });

            var fechaAnulacion = DateTime.UtcNow;

            pago.Anulado = true;
            pago.FechaAnulacion = fechaAnulacion;
            pago.MotivoAnulacion = Datos.Motivo;

            var datosNuevos = JsonSerializer.Serialize(new
            {
                pago.PagoId,
                pago.Anulado,
                pago.FechaAnulacion,
                pago.MotivoAnulacion
            });

            var historial = new HistorialCambioPago
            {
                PagoId = pago.PagoId,
                UsuarioId = usuario.Id,
                TipoCambio = "Anulacion",
                DatosAnteriores = datosAnteriores,
                DatosNuevos = datosNuevos,
                FechaCambio = fechaAnulacion,
                Motivo = Datos.Motivo
            };

            _context.HistorialCambiosPagos.Add(historial);

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            TempData["MensajeExito"] =
                $"El pago #{pago.PagoId} fue anulado correctamente.";

            return RedirectToPage(
                "/Tramites/Detalle",
                new { id = pago.TramiteId });
        }
        catch (DbUpdateException)
        {
            await transaccion.RollbackAsync();

            ModelState.AddModelError(
                string.Empty,
                "No fue posible anular el pago. Inténtalo nuevamente.");

            return Page();
        }
    }

    private async Task<PagoDetalle?> ObtenerPagoAsync(int id)
    {
        return await _context.Pagos
            .AsNoTracking()
            .Where(p => p.PagoId == id)
            .Select(p => new PagoDetalle
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
                Anulado = p.Anulado
            })
            .FirstOrDefaultAsync();
    }
}
