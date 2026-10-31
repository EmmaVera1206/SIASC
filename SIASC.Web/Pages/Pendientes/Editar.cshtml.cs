
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Pendientes;

[Authorize]
public class EditarModel : PageModel
{
    private readonly SIASCDbContext _context;

    public EditarModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int PendienteId { get; set; }
    public string EstadoActual { get; set; } = string.Empty;
    public DateTime? FechaFinalizacion { get; set; }
    public List<SelectListItem> TramitesDisponibles { get; set; } = new();

    public bool PuedeEditar =>
        EstadoActual == "Pendiente" || EstadoActual == "EnProceso";

    [BindProperty]
    public DatosEntrada Entrada { get; set; } = new();

    public class DatosEntrada
    {
        public int? TramiteId { get; set; }

        [Required(ErrorMessage = "Ingresa el título.")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Máximo 2000 caracteres.")]
        public string? Descripcion { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        [Required(ErrorMessage = "Selecciona una prioridad.")]
        public string Prioridad { get; set; } = "Media";
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var pendiente = await _context.Pendientes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PendienteId == id);

        if (pendiente == null)
            return NotFound();

        PendienteId = pendiente.PendienteId;
        EstadoActual = pendiente.Estado;
        FechaFinalizacion = pendiente.FechaFinalizacion;

        Entrada = new DatosEntrada
        {
            TramiteId = pendiente.TramiteId,
            Titulo = pendiente.Titulo,
            Descripcion = pendiente.Descripcion,
            FechaVencimiento = pendiente.FechaVencimiento,
            Prioridad = pendiente.Prioridad
        };

        await CargarTramitesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var pendiente = await _context.Pendientes
            .FirstOrDefaultAsync(p => p.PendienteId == id);

        if (pendiente == null)
            return NotFound();

        PendienteId = pendiente.PendienteId;
        EstadoActual = pendiente.Estado;
        FechaFinalizacion = pendiente.FechaFinalizacion;

        await CargarTramitesAsync();

        if (!PuedeEditar)
        {
            TempData["MensajeError"] =
                "No se puede modificar una actividad completada.";

            return RedirectToPage("/Pendientes/Editar", new { id });
        }

        Entrada.Titulo = Entrada.Titulo?.Trim() ?? string.Empty;
        Entrada.Descripcion = Entrada.Descripcion?.Trim();

        if (string.IsNullOrWhiteSpace(Entrada.Titulo))
        {
            ModelState.AddModelError(
                "Entrada.Titulo",
                "El título no puede estar vacío.");
        }

        if (Entrada.Prioridad != "Baja" &&
            Entrada.Prioridad != "Media" &&
            Entrada.Prioridad != "Alta")
        {
            ModelState.AddModelError(
                "Entrada.Prioridad",
                "Selecciona una prioridad válida.");
        }

        if (Entrada.TramiteId.HasValue)
        {
            bool existe = await _context.Tramites
                .AsNoTracking()
                .AnyAsync(t => t.TramiteId == Entrada.TramiteId.Value);

            if (!existe)
            {
                ModelState.AddModelError(
                    "Entrada.TramiteId",
                    "El trámite seleccionado no existe.");
            }
        }

        if (!ModelState.IsValid)
            return Page();

        pendiente.TramiteId = Entrada.TramiteId;
        pendiente.Titulo = Entrada.Titulo;
        pendiente.Descripcion =
            string.IsNullOrWhiteSpace(Entrada.Descripcion)
                ? null
                : Entrada.Descripcion;
        pendiente.FechaVencimiento = Entrada.FechaVencimiento;
        pendiente.Prioridad = Entrada.Prioridad;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            $"El pendiente #{id} se actualizó correctamente.";

        return RedirectToPage("/Pendientes/Editar", new { id });
    }

    public async Task<IActionResult> OnPostCambiarEstadoAsync(
        int id,
        string nuevoEstado)
    {
        var pendiente = await _context.Pendientes
            .FirstOrDefaultAsync(p => p.PendienteId == id);

        if (pendiente == null)
            return NotFound();

        bool permitido =
            (pendiente.Estado == "Pendiente" &&
                (nuevoEstado == "EnProceso" ||
                 nuevoEstado == "Completado")) ||
            (pendiente.Estado == "EnProceso" &&
                nuevoEstado == "Completado");

        if (!permitido)
        {
            TempData["MensajeError"] =
                "El cambio de estado solicitado no está permitido.";

            return RedirectToPage("/Pendientes/Editar", new { id });
        }

        pendiente.Estado = nuevoEstado;

        if (nuevoEstado == "Completado")
        {
            pendiente.FechaFinalizacion = DateTime.UtcNow;
        }
        else
        {
            pendiente.FechaFinalizacion = null;
        }

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            $"El pendiente #{id} cambió al estado " +
            (nuevoEstado == "EnProceso" ? "En proceso" : "Completado") +
            ".";

        return RedirectToPage("/Pendientes/Editar", new { id });
    }

    private async Task CargarTramitesAsync()
    {
        var tramites = await _context.Tramites
            .AsNoTracking()
            .OrderByDescending(t => t.TramiteId)
            .Select(t => new
            {
                t.TramiteId,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre
            })
            .ToListAsync();

        TramitesDisponibles = tramites
            .Select(t => new SelectListItem
            {
                Value = t.TramiteId.ToString(),
                Text = $"#{t.TramiteId} - {t.NombreCliente} - {t.NombreServicio}"
            })
            .ToList();
    }
}
