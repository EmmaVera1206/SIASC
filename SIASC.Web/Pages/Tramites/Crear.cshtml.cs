
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
public class CrearModel : PageModel
{
    private readonly SIASCDbContext _context;
    private readonly UserManager<UsuarioSistema> _userManager;

    public CrearModel(
        SIASCDbContext context,
        UserManager<UsuarioSistema> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public DatosTramite Input { get; set; } = new();

    public List<SelectListItem> Clientes { get; set; } = new();

    public List<SelectListItem> Servicios { get; set; } = new();

    public class DatosTramite
    {
        [Required(ErrorMessage = "Selecciona un cliente.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Selecciona un cliente válido.")]
        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        [Required(ErrorMessage = "Selecciona un servicio.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Selecciona un servicio válido.")]
        [Display(Name = "Tipo de servicio")]
        public int? TipoServicioId { get; set; }

        [Required(ErrorMessage = "Ingresa el costo total.")]
        [Range(typeof(decimal), "0", "999999999.99",
            ErrorMessage = "Ingresa un costo válido.")]
        [Display(Name = "Costo total (MXN)")]
        public decimal? CostoTotal { get; set; }

        [StringLength(2000,
            ErrorMessage = "Las observaciones no pueden superar los 2000 caracteres.")]
        [Display(Name = "Observaciones generales")]
        public string? ObservacionesGenerales { get; set; }
    }

    public async Task OnGetAsync(int? clienteId)
    {
        await CargarOpcionesAsync();

        if (clienteId.HasValue)
        {
            Input.ClienteId = clienteId.Value;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync();
            return Page();
        }

        var clienteId = Input.ClienteId!.Value;
        var tipoServicioId = Input.TipoServicioId!.Value;
        var costo = Input.CostoTotal!.Value;

        var clienteValido = await _context.Clientes
            .AnyAsync(c => c.ClienteId == clienteId && c.Activo);

        if (!clienteValido)
        {
            ModelState.AddModelError(
                "Input.ClienteId",
                "El cliente seleccionado no existe o está inactivo.");
        }

        var servicioValido = await _context.TiposServicio
            .AnyAsync(s => s.TipoServicioId == tipoServicioId && s.Activo);

        if (!servicioValido)
        {
            ModelState.AddModelError(
                "Input.TipoServicioId",
                "El servicio seleccionado no está disponible.");
        }

        var estadoInicial = await _context.EstadosTramite
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Codigo == "REGISTRADO");

        if (estadoInicial == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se encontró el estado inicial REGISTRADO.");
        }

        if (decimal.Round(costo, 2) != costo)
        {
            ModelState.AddModelError(
                "Input.CostoTotal",
                "El costo debe tener como máximo dos decimales.");
        }

        var usuarioId = _userManager.GetUserId(User);

        if (string.IsNullOrWhiteSpace(usuarioId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync();
            return Page();
        }

        var fechaActual = DateTime.UtcNow;

        var tramite = new Tramite
        {
            ClienteId = clienteId,
            TipoServicioId = tipoServicioId,
            EstadoActualId = estadoInicial!.EstadoId,
            FechaInicio = fechaActual,
            FechaUltimaActualizacion = fechaActual,
            CostoTotalCentavos = decimal.ToInt64(costo * 100m),
            ObservacionesGenerales =
                string.IsNullOrWhiteSpace(Input.ObservacionesGenerales)
                    ? null
                    : Input.ObservacionesGenerales.Trim()
        };

        await using var transaccion =
            await _context.Database.BeginTransactionAsync();

        _context.Tramites.Add(tramite);
        await _context.SaveChangesAsync();

        var historial = new HistorialEstado
        {
            TramiteId = tramite.TramiteId,
            EstadoAnteriorId = null,
            EstadoNuevoId = estadoInicial.EstadoId,
            FechaCambio = fechaActual,
            UsuarioId = usuarioId,
            MotivoCambio = "Registro inicial del trámite."
        };

        _context.HistorialEstados.Add(historial);
        await _context.SaveChangesAsync();

        await transaccion.CommitAsync();

        TempData["MensajeExito"] =
            $"El trámite #{tramite.TramiteId} se registró correctamente.";

        return RedirectToPage("/Tramites/Index");
    }

    private async Task CargarOpcionesAsync()
    {
        Clientes = await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.NombreCompleto)
            .Select(c => new SelectListItem
            {
                Value = c.ClienteId.ToString(),
                Text = c.NombreCompleto + " — " + c.Telefono
            })
            .ToListAsync();

        Servicios = await _context.TiposServicio
            .AsNoTracking()
            .Where(s => s.Activo)
            .OrderBy(s => s.Nombre)
            .Select(s => new SelectListItem
            {
                Value = s.TipoServicioId.ToString(),
                Text = s.Nombre
            })
            .ToListAsync();
    }
}
