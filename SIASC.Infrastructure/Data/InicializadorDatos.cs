
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Data;

public static class InicializadorDatos
{
    public static async Task InicializarAsync(
        SIASCDbContext context)
    {
        await InicializarTiposServicioAsync(context);
        await InicializarEstadosAsync(context);
        await InicializarEstadosPorServicioAsync(context);
        await InicializarTransicionesAsync(context);
    }

    private static async Task InicializarTiposServicioAsync(
        SIASCDbContext context)
    {
        var servicios = new[]
        {
            new TipoServicio
            {
                Codigo = "VISA_AMERICANA",
                Nombre = "Trámite de visa americana",
                Descripcion = "Gestión y seguimiento de solicitudes de visa americana.",
                Activo = true
            },
            new TipoServicio
            {
                Codigo = "PASAPORTE_PRIMERA_VEZ",
                Nombre = "Pasaporte mexicano por primera vez",
                Descripcion = "Gestión de solicitudes de pasaporte por primera vez.",
                Activo = true
            },
            new TipoServicio
            {
                Codigo = "PASAPORTE_RENOVACION",
                Nombre = "Renovación de pasaporte mexicano",
                Descripcion = "Gestión de renovaciones de pasaporte mexicano.",
                Activo = true
            }
        };

        foreach (var servicio in servicios)
        {
            if (!await context.TiposServicio.AnyAsync(
                s => s.Codigo == servicio.Codigo))
            {
                context.TiposServicio.Add(servicio);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task InicializarEstadosAsync(
        SIASCDbContext context)
    {
        var estados = new[]
        {
            new EstadoTramite
            {
                Codigo = "REGISTRADO",
                Nombre = "Registrado",
                OrdenVisual = 1,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "DOCUMENTACION_PENDIENTE",
                Nombre = "Documentación pendiente",
                OrdenVisual = 2,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "DOCUMENTACION_COMPLETA",
                Nombre = "Documentación completa",
                OrdenVisual = 3,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "FORMULARIO_EN_PROCESO",
                Nombre = "Formulario en proceso",
                OrdenVisual = 4,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "FORMULARIO_COMPLETO",
                Nombre = "Formulario completo",
                OrdenVisual = 5,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "CITA_PENDIENTE",
                Nombre = "Cita pendiente",
                OrdenVisual = 6,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "CITA_CONFIRMADA",
                Nombre = "Cita confirmada",
                OrdenVisual = 7,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "EN_PROCESO",
                Nombre = "En proceso",
                OrdenVisual = 8,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "FINALIZADO",
                Nombre = "Finalizado",
                OrdenVisual = 9,
                EsFinal = true,
                Activo = true
            },
            new EstadoTramite
            {
                Codigo = "CANCELADO",
                Nombre = "Cancelado",
                OrdenVisual = 10,
                EsFinal = true,
                Activo = true
            }
        };

        foreach (var estado in estados)
        {
            if (!await context.EstadosTramite.AnyAsync(
                e => e.Codigo == estado.Codigo))
            {
                context.EstadosTramite.Add(estado);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task InicializarEstadosPorServicioAsync(
        SIASCDbContext context)
    {
        string[] estadosVisa =
        {
            "REGISTRADO",
            "DOCUMENTACION_PENDIENTE",
            "DOCUMENTACION_COMPLETA",
            "FORMULARIO_EN_PROCESO",
            "FORMULARIO_COMPLETO",
            "CITA_PENDIENTE",
            "CITA_CONFIRMADA",
            "EN_PROCESO",
            "FINALIZADO",
            "CANCELADO"
        };

        string[] estadosPasaporte =
        {
            "REGISTRADO",
            "DOCUMENTACION_PENDIENTE",
            "DOCUMENTACION_COMPLETA",
            "CITA_PENDIENTE",
            "CITA_CONFIRMADA",
            "EN_PROCESO",
            "FINALIZADO",
            "CANCELADO"
        };

        await AsociarEstadosAsync(
            context, "VISA_AMERICANA", estadosVisa);

        await AsociarEstadosAsync(
            context, "PASAPORTE_PRIMERA_VEZ", estadosPasaporte);

        await AsociarEstadosAsync(
            context, "PASAPORTE_RENOVACION", estadosPasaporte);
    }

    private static async Task AsociarEstadosAsync(
        SIASCDbContext context,
        string codigoServicio,
        string[] codigosEstados)
    {
        var servicio = await context.TiposServicio
            .SingleAsync(s => s.Codigo == codigoServicio);

        foreach (var codigoEstado in codigosEstados)
        {
            var estado = await context.EstadosTramite
                .SingleAsync(e => e.Codigo == codigoEstado);

            bool existe = await context.EstadosPorServicio
                .AnyAsync(e =>
                    e.TipoServicioId == servicio.TipoServicioId &&
                    e.EstadoId == estado.EstadoId);

            if (!existe)
            {
                context.EstadosPorServicio.Add(
                    new EstadoPorServicio
                    {
                        TipoServicioId = servicio.TipoServicioId,
                        EstadoId = estado.EstadoId
                    });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task InicializarTransicionesAsync(
        SIASCDbContext context)
    {
        var flujoVisa = new[]
        {
            ("REGISTRADO", "DOCUMENTACION_PENDIENTE"),
            ("DOCUMENTACION_PENDIENTE", "DOCUMENTACION_COMPLETA"),
            ("DOCUMENTACION_COMPLETA", "FORMULARIO_EN_PROCESO"),
            ("FORMULARIO_EN_PROCESO", "FORMULARIO_COMPLETO"),
            ("FORMULARIO_COMPLETO", "CITA_PENDIENTE"),
            ("CITA_PENDIENTE", "CITA_CONFIRMADA"),
            ("CITA_CONFIRMADA", "EN_PROCESO"),
            ("EN_PROCESO", "FINALIZADO")
        };

        var flujoPasaporte = new[]
        {
            ("REGISTRADO", "DOCUMENTACION_PENDIENTE"),
            ("DOCUMENTACION_PENDIENTE", "DOCUMENTACION_COMPLETA"),
            ("DOCUMENTACION_COMPLETA", "CITA_PENDIENTE"),
            ("CITA_PENDIENTE", "CITA_CONFIRMADA"),
            ("CITA_CONFIRMADA", "EN_PROCESO"),
            ("EN_PROCESO", "FINALIZADO")
        };

        await AgregarTransicionesAsync(
            context, "VISA_AMERICANA", flujoVisa);

        await AgregarTransicionesAsync(
            context, "PASAPORTE_PRIMERA_VEZ", flujoPasaporte);

        await AgregarTransicionesAsync(
            context, "PASAPORTE_RENOVACION", flujoPasaporte);

        var servicios = await context.TiposServicio
            .Where(s => s.Activo)
            .ToListAsync();

        var estadosCancelables = await context.EstadosTramite
            .Where(e => !e.EsFinal)
            .ToListAsync();

        var cancelado = await context.EstadosTramite
            .SingleAsync(e => e.Codigo == "CANCELADO");

        foreach (var servicio in servicios)
        {
            var estadosPermitidos = await context.EstadosPorServicio
                .Where(e => e.TipoServicioId == servicio.TipoServicioId)
                .Select(e => e.EstadoId)
                .ToListAsync();

            foreach (var estado in estadosCancelables)
            {
                if (!estadosPermitidos.Contains(estado.EstadoId))
                    continue;

                await AgregarTransicionAsync(
                    context,
                    servicio.TipoServicioId,
                    estado.EstadoId,
                    cancelado.EstadoId);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task AgregarTransicionesAsync(
        SIASCDbContext context,
        string codigoServicio,
        (string Origen, string Destino)[] transiciones)
    {
        var servicio = await context.TiposServicio
            .SingleAsync(s => s.Codigo == codigoServicio);

        foreach (var (origen, destino) in transiciones)
        {
            var estadoOrigen = await context.EstadosTramite
                .SingleAsync(e => e.Codigo == origen);

            var estadoDestino = await context.EstadosTramite
                .SingleAsync(e => e.Codigo == destino);

            await AgregarTransicionAsync(
                context,
                servicio.TipoServicioId,
                estadoOrigen.EstadoId,
                estadoDestino.EstadoId);
        }

        await context.SaveChangesAsync();
    }

    private static async Task AgregarTransicionAsync(
        SIASCDbContext context,
        int tipoServicioId,
        int estadoOrigenId,
        int estadoDestinoId)
    {
        bool existe = await context.TransicionesEstado
            .AnyAsync(t =>
                t.TipoServicioId == tipoServicioId &&
                t.EstadoOrigenId == estadoOrigenId &&
                t.EstadoDestinoId == estadoDestinoId);

        if (!existe)
        {
            context.TransicionesEstado.Add(
                new TransicionEstado
                {
                    TipoServicioId = tipoServicioId,
                    EstadoOrigenId = estadoOrigenId,
                    EstadoDestinoId = estadoDestinoId,
                    Activo = true
                });

            await context.SaveChangesAsync();
        }
    }
}
