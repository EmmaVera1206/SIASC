
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Clientes;

[Authorize]
public class DetalleModel : PageModel
{
    private readonly SIASCDbContext _context;

    public DetalleModel(SIASCDbContext context)
    {
        _context = context;
    }

    public ClienteDetalle? Cliente { get; set; }

    public List<TramiteListado> Tramites { get; set; } = new();

    public class ClienteDetalle
    {
        public int ClienteId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string? CorreoElectronico { get; set; }

        public string? RecomendadoPor { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public bool Activo { get; set; }
    }

    public class TramiteListado
    {
        public int TramiteId { get; set; }

        public string Servicio { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaCierre { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Cliente = await _context.Clientes
            .AsNoTracking()
            .Where(c => c.ClienteId == id)
            .Select(c => new ClienteDetalle
            {
                ClienteId = c.ClienteId,
                NombreCompleto = c.NombreCompleto,
                Telefono = c.Telefono,
                CorreoElectronico = c.CorreoElectronico,
                RecomendadoPor = c.RecomendadoPor,
                FechaRegistro = c.FechaRegistro,
                FechaActualizacion = c.FechaActualizacion,
                Activo = c.Activo
            })
            .FirstOrDefaultAsync();

        if (Cliente == null)
        {
            return NotFound();
        }

        Tramites = await _context.Tramites
            .AsNoTracking()
            .Where(t => t.ClienteId == id)
            .OrderByDescending(t => t.FechaInicio)
            .Select(t => new TramiteListado
            {
                TramiteId = t.TramiteId,
                Servicio = t.TipoServicio.Nombre,
                Estado = t.EstadoActual.Nombre,
                FechaInicio = t.FechaInicio,
                FechaCierre = t.FechaCierre
            })
            .ToListAsync();

        return Page();
    }
}

