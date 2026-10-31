
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Pendientes;

[Authorize]
public class CrearModel : PageModel
{
    private readonly SIASCDbContext _context;

    public CrearModel(SIASCDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DatosEntrada Entrada { get; set; } = new();

    public List<SelectListItem> TramitesDisponibles { get; set; } = new();

    public class DatosEntrada
    {
        public int? TramiteId { get; set; }

        [Required(ErrorMessage = "Ingresa el título del pendiente.")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Máximo 2000 caracteres.")]
        public string? Descripcion { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? FechaVencimiento { get; set; }

        [Required(ErrorMessage = "Selecciona una prioridad.")]
        public string Prioridad { get; set; } = "Media";
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        await CargarTramitesAsync();

        if (id.HasValue)
        {
            if (!TramitesDisponibles.Any(t =>
                t.Value == id.Value.ToString()))
            {
                return NotFound();
            }

            Entrada.TramiteId = id.Value;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await CargarTramitesAsync();

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
            bool existeTramite = await _context.Tramites
                .AsNoTracking()
                .AnyAsync(t =>
                    t.TramiteId == Entrada.TramiteId.Value);

            if (!existeTramite)
            {
                ModelState.AddModelError(
                    "Entrada.TramiteId",
                    "El trámite seleccionado no existe.");
            }
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var pendiente = new Pendiente
        {
            TramiteId = Entrada.TramiteId,
            Titulo = Entrada.Titulo,
            Descripcion = string.IsNullOrWhiteSpace(Entrada.Descripcion)
                ? null
                : Entrada.Descripcion,
            FechaVencimiento = Entrada.FechaVencimiento,
            Prioridad = Entrada.Prioridad,
            Estado = "Pendiente",
            FechaCreacion = DateTime.UtcNow,
            FechaFinalizacion = null
        };

        _context.Pendientes.Add(pendiente);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            $"El pendiente #{pendiente.PendienteId} se registró correctamente.";

        return RedirectToPage("/Pendientes/Index");
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
