
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;
using SIASC.Infrastructure.Services;

namespace SIASC.Web.Pages.Documentos;

[Authorize]
public class IndexModel : PageModel
{
    private readonly SIASCDbContext _context;
    private readonly AlmacenamientoDocumentosService _almacenamiento;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        SIASCDbContext context,
        AlmacenamientoDocumentosService almacenamiento,
        ILogger<IndexModel> logger)
    {
        _context = context;
        _almacenamiento = almacenamiento;
        _logger = logger;
    }

    public int TramiteId { get; private set; }

    public string NombreCliente { get; private set; } = string.Empty;

    public string NombreServicio { get; private set; } = string.Empty;

    public List<DocumentoListado> Documentos { get; private set; } = new();

    public class DocumentoListado
    {
        public int DocumentoId { get; set; }

        public string TipoDocumento { get; set; } = string.Empty;

        public string NombreOriginal { get; set; } = string.Empty;

        public string TipoMime { get; set; } = string.Empty;

        public long TamanoBytes { get; set; }

        public DateTime FechaCarga { get; set; }

        public string? Observaciones { get; set; }

        public string TamanoLegible
        {
            get
            {
                if (TamanoBytes < 1024)
                {
                    return $"{TamanoBytes} B";
                }

                if (TamanoBytes < 1024 * 1024)
                {
                    return $"{TamanoBytes / 1024d:N1} KB";
                }

                return $"{TamanoBytes / (1024d * 1024d):N2} MB";
            }
        }
    }

    public async Task<IActionResult> OnGetAsync(int id)
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
            return NotFound();
        }

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;
        NombreServicio = tramite.NombreServicio;

        Documentos = await _context.Documentos
            .AsNoTracking()
            .Where(d => d.TramiteId == id && d.Activo)
            .OrderByDescending(d => d.FechaCarga)
            .ThenByDescending(d => d.DocumentoId)
            .Select(d => new DocumentoListado
            {
                DocumentoId = d.DocumentoId,
                TipoDocumento = d.TipoDocumento,
                NombreOriginal = d.NombreOriginal,
                TipoMime = d.TipoMime,
                TamanoBytes = d.TamanoBytes,
                FechaCarga = d.FechaCarga,
                Observaciones = d.Observaciones
            })
            .ToListAsync();

        return Page();
    }

    public Task<IActionResult> OnGetVerAsync(
        int id,
        int documentoId,
        CancellationToken cancellationToken)
    {
        return ObtenerArchivoAsync(
            id,
            documentoId,
            descargar: false,
            cancellationToken);
    }

    public Task<IActionResult> OnGetDescargarAsync(
        int id,
        int documentoId,
        CancellationToken cancellationToken)
    {
        return ObtenerArchivoAsync(
            id,
            documentoId,
            descargar: true,
            cancellationToken);
    }

    public async Task<IActionResult> OnPostDesactivarAsync(
        int id,
        int documentoId,
        CancellationToken cancellationToken)
    {
        var tramiteExiste = await _context.Tramites
            .AsNoTracking()
            .AnyAsync(
                t => t.TramiteId == id,
                cancellationToken);

        if (!tramiteExiste)
        {
            return NotFound();
        }

        var documento = await _context.Documentos
            .FirstOrDefaultAsync(
                d => d.DocumentoId == documentoId &&
                     d.TramiteId == id &&
                     d.Activo,
                cancellationToken);

        if (documento is null)
        {
            return NotFound();
        }

        documento.Activo = false;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Error al desactivar documento {DocumentoId} del trámite {TramiteId}.",
                documentoId,
                id);

            TempData["MensajeError"] =
                "No fue posible desactivar el documento. Inténtalo nuevamente.";

            return RedirectToPage(
                "/Documentos/Index",
                new { id });
        }

        TempData["MensajeExito"] =
            "El documento se desactivó correctamente. " +
            "Su registro y archivo físico se conservaron.";

        return RedirectToPage(
            "/Documentos/Index",
            new { id });
    }

    private async Task<IActionResult> ObtenerArchivoAsync(
        int tramiteId,
        int documentoId,
        bool descargar,
        CancellationToken cancellationToken)
    {
        var documento = await _context.Documentos
            .AsNoTracking()
            .Where(d =>
                d.DocumentoId == documentoId &&
                d.TramiteId == tramiteId &&
                d.Activo)
            .Select(d => new
            {
                d.NombreOriginal,
                d.NombreAlmacenado,
                d.TipoMime
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (documento is null)
        {
            return NotFound();
        }

        if (documento.TipoMime is not
            ("application/pdf" or "image/jpeg" or "image/png"))
        {
            return BadRequest(
                "El tipo de archivo no está permitido.");
        }

        Stream contenido;

        try
        {
            contenido = await _almacenamiento.AbrirLecturaAsync(
                documento.NombreAlmacenado,
                cancellationToken);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Archivo físico no encontrado para documento {DocumentoId}.",
                documentoId);

            return NotFound(
                "El archivo no está disponible.");
        }
        catch (DirectoryNotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Directorio no encontrado para documento {DocumentoId}.",
                documentoId);

            return NotFound(
                "El archivo no está disponible.");
        }
        catch (IOException ex)
        {
            _logger.LogError(
                ex,
                "Error al leer documento {DocumentoId}.",
                documentoId);

            return StatusCode(
                500,
                "No fue posible recuperar el documento.");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(
                ex,
                "Acceso denegado al archivo del documento {DocumentoId}.",
                documentoId);

            return StatusCode(
                500,
                "No fue posible recuperar el documento.");
        }

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers["Cache-Control"] = "private, no-store";
        Response.Headers["Pragma"] = "no-cache";

        if (descargar)
        {
            return File(
                contenido,
                documento.TipoMime,
                documento.NombreOriginal);
        }

        Response.Headers["Content-Security-Policy"] =
            "sandbox; default-src 'none'; style-src 'unsafe-inline'";

        Response.Headers["Content-Disposition"] = "inline";

        return File(
            contenido,
            documento.TipoMime);
    }
}
