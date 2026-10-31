
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Citas;

[Authorize]
public class EditarModel : PageModel
{
    private readonly SIASCDbContext _context;

    public EditarModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int CitaId { get; set; }
    public int TramiteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string NombreServicio { get; set; } = string.Empty;
    public string EstadoActual { get; set; } = string.Empty;

    public bool PuedeEditar =>
        EstadoActual == "Tentativa" ||
        EstadoActual == "Confirmada";

    [BindProperty]
    public DatosEntrada Entrada { get; set; } = new();

    public class DatosEntrada
    {
        [Required(ErrorMessage = "Ingresa el tipo de cita.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        public string TipoCita { get; set; } = string.Empty;

        public DateTime? FechaTentativa { get; set; }

        public DateTime? FechaConfirmada { get; set; }

        [StringLength(300, ErrorMessage = "Máximo 300 caracteres.")]
        public string? Lugar { get; set; }

        [StringLength(2000, ErrorMessage = "Máximo 2000 caracteres.")]
        public string? Observaciones { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var cita = await _context.Citas
            .AsNoTracking()
            .Where(c => c.CitaId == id)
            .Select(c => new
            {
                c.CitaId,
                c.TramiteId,
                c.TipoCita,
                c.EstadoCita,
                c.FechaTentativa,
                c.FechaConfirmada,
                c.Lugar,
                c.Observaciones,
                NombreCliente = c.Tramite.Cliente.NombreCompleto,
                NombreServicio = c.Tramite.TipoServicio.Nombre
            })
            .FirstOrDefaultAsync();

        if (cita == null)
            return NotFound();

        CitaId = cita.CitaId;
        TramiteId = cita.TramiteId;
        NombreCliente = cita.NombreCliente;
        NombreServicio = cita.NombreServicio;
        EstadoActual = cita.EstadoCita;

        Entrada = new DatosEntrada
        {
            TipoCita = cita.TipoCita,
            FechaTentativa = cita.FechaTentativa,
            FechaConfirmada = cita.FechaConfirmada,
            Lugar = cita.Lugar,
            Observaciones = cita.Observaciones
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var cita = await _context.Citas
            .Include(c => c.Tramite)
                .ThenInclude(t => t.Cliente)
            .Include(c => c.Tramite)
                .ThenInclude(t => t.TipoServicio)
            .FirstOrDefaultAsync(c => c.CitaId == id);

        if (cita == null)
            return NotFound();

        CitaId = cita.CitaId;
        TramiteId = cita.TramiteId;
        NombreCliente = cita.Tramite.Cliente.NombreCompleto;
        NombreServicio = cita.Tramite.TipoServicio.Nombre;
        EstadoActual = cita.EstadoCita;

        if (!PuedeEditar)
        {
            TempData["MensajeError"] =
                "Una cita realizada o cancelada no puede modificarse.";

            return RedirectToPage("/Citas/Editar", new { id });
        }

        Entrada.TipoCita = Entrada.TipoCita?.Trim() ?? string.Empty;
        Entrada.Lugar = Entrada.Lugar?.Trim();
        Entrada.Observaciones = Entrada.Observaciones?.Trim();

        if (string.IsNullOrWhiteSpace(Entrada.TipoCita))
        {
            ModelState.AddModelError(
                "Entrada.TipoCita",
                "Ingresa el tipo de cita.");
        }

        if (EstadoActual == "Tentativa" &&
            !Entrada.FechaTentativa.HasValue)
        {
            ModelState.AddModelError(
                "Entrada.FechaTentativa",
                "La fecha tentativa es obligatoria.");
        }

        if (EstadoActual == "Confirmada" &&
            !Entrada.FechaConfirmada.HasValue)
        {
            ModelState.AddModelError(
                "Entrada.FechaConfirmada",
                "La fecha confirmada es obligatoria.");
        }

        if (!ModelState.IsValid)
            return Page();

        cita.TipoCita = Entrada.TipoCita;
        cita.FechaTentativa = Entrada.FechaTentativa;
        cita.FechaConfirmada = Entrada.FechaConfirmada;
        cita.Lugar = string.IsNullOrWhiteSpace(Entrada.Lugar)
            ? null
            : Entrada.Lugar;
        cita.Observaciones =
            string.IsNullOrWhiteSpace(Entrada.Observaciones)
                ? null
                : Entrada.Observaciones;
        cita.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            $"La cita #{id} se actualizó correctamente.";

        return RedirectToPage("/Citas/Editar", new { id });
    }

    public async Task<IActionResult> OnPostCambiarEstadoAsync(
        int id,
        string nuevoEstado)
    {
        var cita = await _context.Citas
            .FirstOrDefaultAsync(c => c.CitaId == id);

        if (cita == null)
            return NotFound();

        bool transicionValida =
            (cita.EstadoCita == "Tentativa" &&
                (nuevoEstado == "Confirmada" ||
                 nuevoEstado == "Cancelada")) ||
            (cita.EstadoCita == "Confirmada" &&
                (nuevoEstado == "Realizada" ||
                 nuevoEstado == "Cancelada"));

        if (!transicionValida)
        {
            TempData["MensajeError"] =
                "El cambio de estado solicitado no está permitido.";

            return RedirectToPage("/Citas/Editar", new { id });
        }

        if (nuevoEstado == "Confirmada" &&
            !cita.FechaConfirmada.HasValue)
        {
            TempData["MensajeError"] =
                "Debes guardar una fecha confirmada antes de confirmar la cita.";

            return RedirectToPage("/Citas/Editar", new { id });
        }

        if (nuevoEstado == "Realizada" &&
            !cita.FechaConfirmada.HasValue)
        {
            TempData["MensajeError"] =
                "No se puede realizar una cita sin fecha confirmada.";

            return RedirectToPage("/Citas/Editar", new { id });
        }

        cita.EstadoCita = nuevoEstado;
        cita.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        TempData["MensajeExito"] =
            $"La cita #{id} cambió al estado {nuevoEstado}.";

        return RedirectToPage("/Citas/Editar", new { id });
    }
}

