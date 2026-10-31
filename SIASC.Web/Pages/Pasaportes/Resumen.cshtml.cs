using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Pasaportes;

[Authorize]
public class ResumenModel : PageModel
{
    private readonly SIASCDbContext _context;

    public ResumenModel(SIASCDbContext context)
    {
        _context = context;
    }

    public int TramiteId { get; private set; }
    public string NombreCliente { get; private set; } = string.Empty;
    public string NombreServicio { get; private set; } = string.Empty;
    public string EstadoTramite { get; private set; } = string.Empty;
    public string? Curp { get; private set; }
    public bool TieneRegistro { get; private set; }
    public bool CurpVerificada { get; private set; }
    public DateTime? FechaVerificacion { get; private set; }
    public string? Observaciones { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var tramite = await _context.Tramites
            .AsNoTracking()
            .Where(t => t.TramiteId == id &&
                (t.TipoServicio.Codigo == "PASAPORTE_PRIMERA_VEZ" ||
                 t.TipoServicio.Codigo == "PASAPORTE_RENOVACION"))
            .Select(t => new
            {
                t.TramiteId,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre,
                EstadoTramite = t.EstadoActual.Nombre
            })
            .FirstOrDefaultAsync();

        if (tramite is null)
            return NotFound();

        TramiteId = tramite.TramiteId;
        NombreCliente = tramite.NombreCliente;
        NombreServicio = tramite.NombreServicio;
        EstadoTramite = tramite.EstadoTramite;

        var datos = await _context.DatosPasaporte
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.TramiteId == id);

        if (datos is not null)
        {
            TieneRegistro = true;
            Curp = datos.CURP;
            CurpVerificada = datos.CURPVerificada;
            FechaVerificacion = datos.FechaVerificacion;
            Observaciones = datos.Observaciones;
        }

        return Page();
    }
}

