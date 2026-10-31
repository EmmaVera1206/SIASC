
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SIASC.Infrastructure.Data;

namespace SIASC.Web.Pages.Tramites;

[Authorize]
public class IndexModel : PageModel
{
    private readonly SIASCDbContext _context;

    public IndexModel(SIASCDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Busqueda { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? TipoServicioId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? EstadoId { get; set; }

    public List<TramiteListado> Tramites { get; set; } = new();

    public List<OpcionFiltro> Servicios { get; set; } = new();

    public List<OpcionFiltro> Estados { get; set; } = new();

    public int TotalTramites { get; set; }

    public int TramitesEnProceso { get; set; }

    public int TramitesFinalizados { get; set; }

    public class OpcionFiltro
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }

    public class TramiteListado
    {
        public int TramiteId { get; set; }

        public int ClienteId { get; set; }

        public string NombreCliente { get; set; } = string.Empty;

        public string NombreServicio { get; set; } = string.Empty;

        public string NombreEstado { get; set; } = string.Empty;

        public string CodigoEstado { get; set; } = string.Empty;

        public bool EsEstadoFinal { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaCierre { get; set; }

        public long CostoTotalCentavos { get; set; }

        public decimal CostoTotal =>
            CostoTotalCentavos / 100m;
    }

    public async Task OnGetAsync()
    {
        Servicios = await _context.TiposServicio
            .AsNoTracking()
            .Where(s => s.Activo)
            .OrderBy(s => s.Nombre)
            .Select(s => new OpcionFiltro
            {
                Id = s.TipoServicioId,
                Nombre = s.Nombre
            })
            .ToListAsync();

        Estados = await _context.EstadosTramite
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.OrdenVisual)
            .Select(e => new OpcionFiltro
            {
                Id = e.EstadoId,
                Nombre = e.Nombre
            })
            .ToListAsync();

        TotalTramites = await _context.Tramites
            .AsNoTracking()
            .CountAsync();

        TramitesEnProceso = await _context.Tramites
            .AsNoTracking()
            .CountAsync(t => !t.EstadoActual.EsFinal);

        TramitesFinalizados = await _context.Tramites
            .AsNoTracking()
            .CountAsync(t =>
                t.EstadoActual.Codigo == "FINALIZADO");

        var consulta = _context.Tramites
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Busqueda))
        {
            var texto = Busqueda.Trim();

            consulta = consulta.Where(t =>
                t.Cliente.NombreCompleto.Contains(texto) ||
                t.Cliente.Telefono.Contains(texto));
        }

        if (TipoServicioId.HasValue)
        {
            consulta = consulta.Where(t =>
                t.TipoServicioId == TipoServicioId.Value);
        }

        if (EstadoId.HasValue)
        {
            consulta = consulta.Where(t =>
                t.EstadoActualId == EstadoId.Value);
        }

        Tramites = await consulta
            .OrderByDescending(t => t.FechaInicio)
            .ThenByDescending(t => t.TramiteId)
            .Select(t => new TramiteListado
            {
                TramiteId = t.TramiteId,
                ClienteId = t.ClienteId,
                NombreCliente = t.Cliente.NombreCompleto,
                NombreServicio = t.TipoServicio.Nombre,
                NombreEstado = t.EstadoActual.Nombre,
                CodigoEstado = t.EstadoActual.Codigo,
                EsEstadoFinal = t.EstadoActual.EsFinal,
                FechaInicio = t.FechaInicio,
                FechaCierre = t.FechaCierre,
                CostoTotalCentavos = t.CostoTotalCentavos
            })
            .ToListAsync();
    }
}
