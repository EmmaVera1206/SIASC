using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class EmpleoAnteriorConfiguration
    : IEntityTypeConfiguration<EmpleoAnterior>
{
    public void Configure(EntityTypeBuilder<EmpleoAnterior> builder)
    {
        builder.ToTable("EmpleosAnteriores");

        builder.HasKey(e => e.EmpleoAnteriorId);

        builder.Property(e => e.NombreEmpresa)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Direccion).HasMaxLength(500);
        builder.Property(e => e.Telefono).HasMaxLength(30);
        builder.Property(e => e.Puesto).HasMaxLength(150);
        builder.Property(e => e.NombreJefeInmediato).HasMaxLength(200);

        builder.HasOne(e => e.FormularioVisa)
            .WithMany(f => f.EmpleosAnteriores)
            .HasForeignKey(e => e.FormularioVisaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
