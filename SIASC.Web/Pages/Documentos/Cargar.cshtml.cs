
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Data;
using SIASC.Infrastructure.Services;

namespace SIASC.Web.Pages.Documentos;

[Authorize]
public class CargarModel : PageModel
{
    private readonly SIASCDbContext _context;
    private readonly AlmacenamientoDocumentosService _almacenamiento;
    private readonly ILogger<CargarModel> _logger;

    public CargarModel(
        SIASCDbContext context,
        AlmacenamientoDocumentosService almacenamiento,
        ILogger<CargarModel> logger)
    {
        _context = context;
        _almacenamiento = almacenamiento;
        _logger = logger;
    }

    public int TramiteId { get; private set; }

    public string NombreCliente { get; private set; } = string.Empty;

    public string NombreServicio { get; private set; } = string.Empty;

    [BindProperty]
    public DatosEntrada Input { get; set; } = new();

    public class DatosEntrada
    {
        [Required(ErrorMessage = "Indica el tipo de documento.")]
        [StringLength(
            100,
            ErrorMessage = "El tipo de documento no puede superar 100 caracteres.")]
        [Display(Name = "Tipo de documento")]
        public string TipoDocumento { get; set; } = string.Empty;

        [StringLength(
            1000,
            ErrorMessage = "Las observaciones no pueden superar 1000 caracteres.")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "Selecciona un archivo.")]
        [Display(Name = "Archivo")]
        public IFormFile? Archivo { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        int id,
        CancellationToken cancellationToken)
    {
        if (!await CargarTramiteAsync(id))
        {
            return NotFound();
        }

        Input.TipoDocumento =
            Input.TipoDocumento?.Trim() ?? string.Empty;

        Input.Observaciones =
            string.IsNullOrWhiteSpace(Input.Observaciones)
                ? null
                : Input.Observaciones.Trim();

        ModelState.Clear();

        if (!TryValidateModel(Input, nameof(Input)))
        {
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Input.TipoDocumento))
        {
            ModelState.AddModelError(
                "Input.TipoDocumento",
                "Indica el tipo de documento.");

            return Page();
        }

        var archivo = Input.Archivo!;

        if (archivo.Length <= 0)
        {
            ModelState.AddModelError(
                "Input.Archivo",
                "No se permiten archivos vacíos.");

            return Page();
        }

        if (archivo.Length >
            AlmacenamientoDocumentosService.TamanoMaximoBytes)
        {
            ModelState.AddModelError(
                "Input.Archivo",
                "El archivo supera el límite de 10 MB.");

            return Page();
        }

        var nombreOriginal = Path.GetFileName(
            archivo.FileName.Replace('\\', '/'));

        if (string.IsNullOrWhiteSpace(nombreOriginal))
        {
            ModelState.AddModelError(
                "Input.Archivo",
                "El nombre del archivo no es válido.");

            return Page();
        }

        if (nombreOriginal.Length > 255)
        {
            ModelState.AddModelError(
                "Input.Archivo",
                "El nombre del archivo no puede superar 255 caracteres.");

            return Page();
        }

        var extension = Path.GetExtension(nombreOriginal)
            .ToLowerInvariant();

        if (extension is not
            (".pdf" or ".jpg" or ".jpeg" or ".png"))
        {
            ModelState.AddModelError(
                "Input.Archivo",
                "Solo se permiten archivos PDF, JPG, JPEG y PNG.");

            return Page();
        }

        ResultadoAlmacenamiento resultado;

        try
        {
            await using var contenido = archivo.OpenReadStream();

            resultado = await _almacenamiento.GuardarAsync(
                contenido,
                nombreOriginal,
                cancellationToken);
        }
        catch (InvalidDataException ex)
        {
            ModelState.AddModelError(
                "Input.Archivo",
                ex.Message);

            return Page();
        }
        catch (IOException ex)
        {
            _logger.LogError(
                ex,
                "Error de almacenamiento para el trámite {TramiteId}.",
                id);

            ModelState.AddModelError(
                string.Empty,
                "No fue posible almacenar el archivo. Inténtalo nuevamente.");

            return Page();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(
                ex,
                "Acceso denegado al almacenamiento del trámite {TramiteId}.",
                id);

            ModelState.AddModelError(
                string.Empty,
                "No fue posible acceder al almacenamiento de documentos.");

            return Page();
        }

        var documento = new Documento
        {
            TramiteId = id,
            TipoDocumento = Input.TipoDocumento,
            NombreOriginal = nombreOriginal,
            NombreAlmacenado = resultado.NombreAlmacenado,
            TipoMime = resultado.TipoMime,
            TamanoBytes = resultado.TamanoBytes,
            HashSHA256 = resultado.HashSHA256,
            FechaCarga = DateTime.UtcNow,
            Observaciones = Input.Observaciones,
            Activo = true
        };

        try
        {
            _context.Documentos.Add(documento);

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (
            ex is DbUpdateException ||
            ex is OperationCanceledException)
        {
            _logger.LogError(
                ex,
                "No se pudo registrar el documento del trámite {TramiteId}.",
                id);

            try
            {
                await _almacenamiento.EliminarAsync(
                    resultado.NombreAlmacenado,
                    CancellationToken.None);
            }
            catch (Exception errorLimpieza) when (
                errorLimpieza is IOException ||
                errorLimpieza is UnauthorizedAccessException)
            {
                _logger.LogError(
                    errorLimpieza,
                    "No se pudo limpiar el archivo {NombreAlmacenado}.",
                    resultado.NombreAlmacenado);
            }

            if (ex is OperationCanceledException)
            {
                throw;
            }

            ModelState.AddModelError(
                string.Empty,
                "No fue posible registrar el documento. Inténtalo nuevamente.");

            return Page();
        }

        TempData["MensajeExito"] =
            "El documento se cargó correctamente.";

        return RedirectToPage(
            "/Documentos/Cargar",
            new { id });
    }

    private async Task<bool> CargarTramiteAsync(int id)
    {
        var tramite = await _context.Tramites
            .AsNoTracking()
            .Where(t => t.TramiteId == id)
            .Select(t => new
            {
                t.TramiteId,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre
            })
            .FirstOrDefaultAsync();

        if (tramite is null)
        {
            return false;
        }

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;
        NombreServicio = tramite.NombreServicio;

        return true;
    }
}
