
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;
using SIASC.Infrastructure.Identity;

namespace SIASC.Web.Pages.Tramites;

[Authorize]
public class CambiarEstadoModel : PageModel
{
    private readonly SIASCDbContext _context;
    private readonly UserManager<UsuarioSistema> _userManager;

    public CambiarEstadoModel(
        SIASCDbContext context,
        UserManager<UsuarioSistema> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int TramiteId { get; set; }

    public string NombreCliente { get; set; } = string.Empty;

    public string NombreServicio { get; set; } = string.Empty;

    public string EstadoActual { get; set; } = string.Empty;

    public List<SelectListItem> EstadosDisponibles { get; set; } = new();

    [BindProperty]
    public DatosCambio Input { get; set; } = new();

    public class DatosCambio
    {
        [Required(ErrorMessage = "Selecciona el nuevo estado.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Selecciona un estado válido.")]
        public int? EstadoNuevoId { get; set; }

        [StringLength(1000,
            ErrorMessage = "El motivo no puede superar los 1000 caracteres.")]
        public string? MotivoCambio { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var existe = await CargarInformacionAsync(id);

        if (!existe)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var existe = await CargarInformacionAsync(id);

        if (!existe)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var usuarioId = _userManager.GetUserId(User);

        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return Challenge();
        }

        await using var transaccion =
            await _context.Database.BeginTransactionAsync();

        var tramite = await _context.Tramites
            .Include(t => t.EstadoActual)
            .FirstOrDefaultAsync(t => t.TramiteId == id);

        if (tramite == null)
        {
            return NotFound();
        }

        if (tramite.EstadoActual.EsFinal)
        {
            ModelState.AddModelError(
                string.Empty,
                "Este trámite ya se encuentra cerrado.");

            return Page();
        }

        int nuevoEstadoId = Input.EstadoNuevoId!.Value;

        var transicion = await _context.TransicionesEstado
            .Include(t => t.EstadoDestino)
            .FirstOrDefaultAsync(t =>
                t.TipoServicioId == tramite.TipoServicioId &&
                t.EstadoOrigenId == tramite.EstadoActualId &&
                t.EstadoDestinoId == nuevoEstadoId &&
                t.Activo &&
                t.EstadoDestino.Activo);

        if (transicion == null)
        {
            ModelState.AddModelError(
                "Input.EstadoNuevoId",
                "La transición seleccionada no está permitida.");

            return Page();
        }

        bool perteneceAlServicio =
            await _context.EstadosPorServicio.AnyAsync(e =>
                e.TipoServicioId == tramite.TipoServicioId &&
                e.EstadoId == nuevoEstadoId);

        if (!perteneceAlServicio)
        {
            ModelState.AddModelError(
                "Input.EstadoNuevoId",
                "El estado no corresponde a este servicio.");

            return Page();
        }

        string? motivo = string.IsNullOrWhiteSpace(Input.MotivoCambio)
            ? null
            : Input.MotivoCambio.Trim();

        bool esCancelacion =
            transicion.EstadoDestino.Codigo == "CANCELADO";

        if (esCancelacion && string.IsNullOrWhiteSpace(motivo))
        {
            ModelState.AddModelError(
                "Input.MotivoCambio",
                "Debes indicar el motivo de cancelación.");

            return Page();
        }

        var fechaActual = DateTime.UtcNow;
        var estadoAnteriorId = tramite.EstadoActualId;

        tramite.EstadoActualId = nuevoEstadoId;
        tramite.FechaUltimaActualizacion = fechaActual;

        if (transicion.EstadoDestino.EsFinal)
        {
            tramite.FechaCierre = fechaActual;
        }

        if (esCancelacion)
        {
            tramite.MotivoCancelacion = motivo;
        }

        _context.HistorialEstados.Add(new HistorialEstado
        {
            TramiteId = tramite.TramiteId,
            EstadoAnteriorId = estadoAnteriorId,
            EstadoNuevoId = nuevoEstadoId,
            FechaCambio = fechaActual,
            UsuarioId = usuarioId,
            MotivoCambio = motivo
        });

        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        TempData["MensajeExito"] =
            "El estado del trámite se actualizó correctamente.";

        return RedirectToPage(
            "/Tramites/Detalle",
            new { id = tramite.TramiteId });
    }

    private async Task<bool> CargarInformacionAsync(int id)
    {
        var tramite = await _context.Tramites
            .AsNoTracking()
            .Where(t => t.TramiteId == id)
            .Select(t => new
            {
                t.TramiteId,
                t.TipoServicioId,
                t.EstadoActualId,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre,
                EstadoActual = t.EstadoActual.Nombre,
                EstadoFinal = t.EstadoActual.EsFinal
            })
            .FirstOrDefaultAsync();

        if (tramite == null)
        {
            return false;
        }

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;
        NombreServicio = tramite.NombreServicio;
        EstadoActual = tramite.EstadoActual;

        if (tramite.EstadoFinal)
        {
            return true;
        }

        EstadosDisponibles = await _context.TransicionesEstado
            .AsNoTracking()
            .Where(t =>
                t.TipoServicioId == tramite.TipoServicioId &&
                t.EstadoOrigenId == tramite.EstadoActualId &&
                t.Activo &&
                t.EstadoDestino.Activo &&
                _context.EstadosPorServicio.Any(e =>
                    e.TipoServicioId == tramite.TipoServicioId &&
                    e.EstadoId == t.EstadoDestinoId))
            .OrderBy(t => t.EstadoDestino.OrdenVisual)
            .Select(t => new SelectListItem
            {
                Value = t.EstadoDestinoId.ToString(),
                Text = t.EstadoDestino.Nombre
            })
            .ToListAsync();

        return true;
    }
}
