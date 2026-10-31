
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Pendientes;

[Authorize]
public class IndexModel : PageModel
{
    private readonly SIASCDbContext _context;

    public IndexModel(SIASCDbContext context)
    {
        _context = context;
    }

    public List<PendienteListado> Pendientes { get; set; } = new();

    public int TotalAbiertos =>
        Pendientes.Count(p => p.Estado != "Completado");

    public int TotalCompletados =>
        Pendientes.Count(p => p.Estado == "Completado");

    public class PendienteListado
    {
        public int PendienteId { get; set; }
        public int? TramiteId { get; set; }
        public string? NombreCliente { get; set; }
        public string? NombreServicio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Prioridad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaFinalizacion { get; set; }

        public bool EstaVencido =>
            Estado != "Completado" &&
            FechaVencimiento.HasValue &&
            FechaVencimiento.Value < DateTime.Now;

        public bool PuedeEditar =>
            Estado == "Pendiente" || Estado == "EnProceso";
    }

    public async Task OnGetAsync()
    {
        Pendientes = await _context.Pendientes
            .AsNoTracking()
            .Select(p => new PendienteListado
            {
                PendienteId = p.PendienteId,
                TramiteId = p.TramiteId,
                NombreCliente = p.Tramite != null
                    ? p.Tramite.Cliente.NombreCompleto
                    : null,
                NombreServicio = p.Tramite != null
                    ? p.Tramite.TipoServicio.Nombre
                    : null,
                Titulo = p.Titulo,
                Descripcion = p.Descripcion,
                FechaVencimiento = p.FechaVencimiento,
                Prioridad = p.Prioridad,
                Estado = p.Estado,
                FechaCreacion = p.FechaCreacion,
                FechaFinalizacion = p.FechaFinalizacion
            })
            .ToListAsync();

        Pendientes = Pendientes
            .OrderBy(p => p.Estado == "Completado" ? 1 : 0)
            .ThenBy(p => p.FechaVencimiento ?? DateTime.MaxValue)
            .ThenByDescending(p => p.Prioridad == "Alta" ? 3 :
                                   p.Prioridad == "Media" ? 2 : 1)
            .ThenByDescending(p => p.PendienteId)
            .ToList();
    }
}
