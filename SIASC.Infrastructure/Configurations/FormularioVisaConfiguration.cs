using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class FormularioVisaConfiguration
    : IEntityTypeConfiguration<FormularioVisa>
{
    public void Configure(EntityTypeBuilder<FormularioVisa> builder)
    {
        builder.ToTable("FormulariosVisa");

        builder.HasKey(f => f.FormularioVisaId);

        builder.HasOne(f => f.Tramite)
            .WithOne()
            .HasForeignKey<FormularioVisa>(f => f.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => f.TramiteId)
            .IsUnique();

        builder.Property(f => f.NombreSegunPasaporte)
            .HasMaxLength(200);

        builder.Property(f => f.Sexo).HasMaxLength(30);
        builder.Property(f => f.EstadoCivil).HasMaxLength(50);
        builder.Property(f => f.Nacionalidad).HasMaxLength(100);
        builder.Property(f => f.OtraNacionalidad).HasMaxLength(100);
        builder.Property(f => f.PaisResidencia).HasMaxLength(100);

        builder.Property(f => f.MotivoViaje).HasMaxLength(1000);
        builder.Property(f => f.CiudadDestino).HasMaxLength(150);
        builder.Property(f => f.Hospedaje).HasMaxLength(500);
        builder.Property(f => f.EmpresaVisita).HasMaxLength(200);
        builder.Property(f => f.DireccionEmpresaVisita).HasMaxLength(500);
        builder.Property(f => f.TelefonoEmpresaVisita).HasMaxLength(30);
        builder.Property(f => f.CorreoEmpresaVisita).HasMaxLength(200);

        builder.Property(f => f.ResponsableGastos).HasMaxLength(100);
        builder.Property(f => f.NombrePatrocinador).HasMaxLength(200);
        builder.Property(f => f.ParentescoPatrocinador).HasMaxLength(100);
        builder.Property(f => f.TelefonoPatrocinador).HasMaxLength(30);

        builder.Property(f => f.DetalleNegativa).HasMaxLength(2000);
        builder.Property(f => f.DetalleCancelacionVisa).HasMaxLength(2000);
        builder.Property(f => f.DetalleExtravíoVisa).HasMaxLength(2000);

        builder.Property(f => f.DomicilioParticular).HasMaxLength(500);
        builder.Property(f => f.TelefonoParticular).HasMaxLength(30);
        builder.Property(f => f.TelefonoAdicional).HasMaxLength(30);
        builder.Property(f => f.CorreoPersonal).HasMaxLength(200);

        builder.Property(f => f.NombrePadre).HasMaxLength(200);
        builder.Property(f => f.NombreMadre).HasMaxLength(200);

        builder.Property(f => f.NombreConyuge).HasMaxLength(200);
        builder.Property(f => f.LugarNacimientoConyuge).HasMaxLength(200);
        builder.Property(f => f.MotivoDivorcio).HasMaxLength(2000);

        builder.Property(f => f.Ocupacion).HasMaxLength(150);
        builder.Property(f => f.NombreEmpresaActual).HasMaxLength(200);
        builder.Property(f => f.DireccionEmpresaActual).HasMaxLength(500);
        builder.Property(f => f.TelefonoEmpresaActual).HasMaxLength(30);
        builder.Property(f => f.PuestoActual).HasMaxLength(150);

        builder.Property(f => f.NombreInstitucion).HasMaxLength(200);
        builder.Property(f => f.DireccionInstitucion).HasMaxLength(500);
        builder.Property(f => f.TelefonoInstitucion).HasMaxLength(30);
        builder.Property(f => f.CarreraCurso).HasMaxLength(200);

        builder.Property(f => f.EstadoCaptura)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Borrador");

        builder.HasIndex(f => f.EstadoCaptura);
    }
}

