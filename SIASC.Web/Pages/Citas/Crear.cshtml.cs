
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Citas;

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
        [Required(ErrorMessage = "Selecciona un trámite.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un trámite válido.")]
        public int? TramiteId { get; set; }

        [Required(ErrorMessage = "Ingresa el tipo de cita.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string TipoCita { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona el estado de la cita.")]
        public string EstadoCita { get; set; } = "Tentativa";

        [DataType(DataType.DateTime)]
        public DateTime? FechaTentativa { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? FechaConfirmada { get; set; }

        [StringLength(300, ErrorMessage = "Máximo 300 caracteres.")]
        public string? Lugar { get; set; }

        [StringLength(2000, ErrorMessage = "Máximo 2000 caracteres.")]
        public string? Observaciones { get; set; }
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

        Entrada.TipoCita = Entrada.TipoCita?.Trim() ?? string.Empty;
        Entrada.Lugar = Entrada.Lugar?.Trim();
        Entrada.Observaciones = Entrada.Observaciones?.Trim();

        if (Entrada.EstadoCita != "Tentativa" &&
            Entrada.EstadoCita != "Confirmada")
        {
            ModelState.AddModelError(
                "Entrada.EstadoCita",
                "Selecciona un estado válido.");
        }

        if (Entrada.EstadoCita == "Tentativa" &&
            !Entrada.FechaTentativa.HasValue)
        {
            ModelState.AddModelError(
                "Entrada.FechaTentativa",
                "La fecha tentativa es obligatoria.");
        }

        if (Entrada.EstadoCita == "Confirmada" &&
            !Entrada.FechaConfirmada.HasValue)
        {
            ModelState.AddModelError(
                "Entrada.FechaConfirmada",
                "La fecha confirmada es obligatoria.");
        }

        if (Entrada.TramiteId.HasValue &&
            Entrada.TramiteId.Value > 0)
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

        var ahora = DateTime.UtcNow;

        var cita = new Cita
        {
            TramiteId = Entrada.TramiteId!.Value,
            TipoCita = Entrada.TipoCita,
            EstadoCita = Entrada.EstadoCita,
            FechaTentativa = Entrada.FechaTentativa,
            FechaConfirmada = Entrada.FechaConfirmada,
            Lugar = string.IsNullOrWhiteSpace(Entrada.Lugar)
                ? null
                : Entrada.Lugar,
            Observaciones =
                string.IsNullOrWhiteSpace(Entrada.Observaciones)
                    ? null
                    : Entrada.Observaciones,
            FechaRegistro = ahora,
            FechaActualizacion = ahora
        };

        _context.Citas.Add(cita);
        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            $"La cita #{cita.CitaId} se registró correctamente.";

        return RedirectToPage("/Citas/Index");
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
