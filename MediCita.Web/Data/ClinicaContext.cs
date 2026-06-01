using System;
using System.Collections.Generic;
using ClinPiura.Web.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ClinPiura.Web.Data;

public partial class ClinicaContext : DbContext
{
    public ClinicaContext(DbContextOptions<ClinicaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbCarritoTemporal> TbCarritoTemporals { get; set; }

    public virtual DbSet<TbCita> TbCitas { get; set; }

    public virtual DbSet<TbCitaNota> TbCitaNotas { get; set; }

    public virtual DbSet<TbDetalleVentum> TbDetalleVenta { get; set; }

    public virtual DbSet<TbEspecialidade> TbEspecialidades { get; set; }

    public virtual DbSet<TbHorariosMedico> TbHorariosMedicos { get; set; }

    public virtual DbSet<TbHorariosPlantilla> TbHorariosPlantillas { get; set; }

    public virtual DbSet<TbMedicamento> TbMedicamentos { get; set; }

    public virtual DbSet<TbMedico> TbMedicos { get; set; }

    public virtual DbSet<TbRole> TbRoles { get; set; }

    public virtual DbSet<TbUsuario> TbUsuarios { get; set; }

    public virtual DbSet<TbVenta> TbVentas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbCarritoTemporal>(entity =>
        {
            entity.HasKey(e => e.IdCarrito).HasName("PK__tb_Carri__8B4A618C8F97A640");

            entity.ToTable("tb_CarritoTemporal");

            entity.Property(e => e.Cantidad).HasDefaultValue(1);
            entity.Property(e => e.FechaAgregado)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdMedicamentoNavigation).WithMany(p => p.TbCarritoTemporals)
                .HasForeignKey(d => d.IdMedicamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carrito_Medicamento");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.TbCarritoTemporals)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carrito_Usuario");
        });

        modelBuilder.Entity<TbCita>(entity =>
        {
            entity.HasKey(e => e.IdCita).HasName("PK__tb_Citas__394B02022A96C8F0");

            entity.ToTable("tb_Citas");

            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("P")
                .IsFixedLength();
            entity.Property(e => e.FechaCita).HasColumnType("datetime");
            entity.Property(e => e.MontoPagar).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.TbCita)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Medico");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.TbCita)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Paciente");
        });

        modelBuilder.Entity<TbCitaNota>(entity =>
        {
            entity.HasKey(e => e.IdNota).HasName("PK__tb_CitaN__4B2ACFF2E313852D");

            entity.ToTable("tb_CitaNotas");

            entity.Property(e => e.FechaNota)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nota).HasMaxLength(500);

            entity.HasOne(d => d.IdCitaNavigation).WithMany(p => p.TbCitaNota)
                .HasForeignKey(d => d.IdCita)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CitaNotas_Citas");
        });

        modelBuilder.Entity<TbDetalleVentum>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__tb_Detal__E43646A5AC303AD0");

            entity.ToTable("tb_DetalleVenta");

            entity.Property(e => e.ImagenUrl).HasMaxLength(500);
            entity.Property(e => e.NombreMedicamento).HasMaxLength(150);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Promocion).HasMaxLength(300);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdMedicamentoNavigation).WithMany(p => p.TbDetalleVenta)
                .HasForeignKey(d => d.IdMedicamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleVenta_Medicamento");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.TbDetalleVenta)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK_DetalleVenta_Venta");
        });

        modelBuilder.Entity<TbEspecialidade>(entity =>
        {
            entity.HasKey(e => e.IdEspecialidad).HasName("PK__tb_Espec__693FA0AF8DF16B8F");

            entity.ToTable("tb_Especialidades");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.NombreEspec)
                .HasMaxLength(80)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbHorariosMedico>(entity =>
        {
            entity.HasKey(e => e.IdHorario).HasName("PK__tb_Horar__1539229B1CBF57C3");

            entity.ToTable("tb_HorariosMedico");

            entity.Property(e => e.Disponible).HasDefaultValue(true);

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.TbHorariosMedicos)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Horarios_Medico");
        });

        modelBuilder.Entity<TbHorariosPlantilla>(entity =>
        {
            entity.HasKey(e => e.IdHorarioPlantilla).HasName("PK__tb_Horar__57D9A353C512F8BE");

            entity.ToTable("tb_HorariosPlantilla");
        });

        modelBuilder.Entity<TbMedicamento>(entity =>
        {
            entity.HasKey(e => e.IdMedicamento).HasName("PK__tb_Medic__AC96376EE5884489");

            entity.ToTable("tb_Medicamentos");

            entity.Property(e => e.Categoria).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(1000);
            entity.Property(e => e.ImagenUrl).HasMaxLength(500);
            entity.Property(e => e.Laboratorio)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Promocion).HasMaxLength(300);
        });

        modelBuilder.Entity<TbMedico>(entity =>
        {
            entity.HasKey(e => e.IdMedico).HasName("PK__tb_Medic__C326E65282C1F957");

            entity.ToTable("tb_Medicos");

            entity.HasIndex(e => e.IdUsuario, "UQ__tb_Medic__5B65BF96CA7C391F").IsUnique();

            entity.Property(e => e.Cmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CMP");
            entity.Property(e => e.DuracionMinutos).HasDefaultValue(30);
            entity.Property(e => e.ImagenUrl).HasMaxLength(500);
            entity.Property(e => e.PrecioConsulta)
                .HasDefaultValue(80.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Rne)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("RNE");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TipoServicio)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEspecialidadNavigation).WithMany(p => p.TbMedicos)
                .HasForeignKey(d => d.IdEspecialidad)
                .HasConstraintName("FK_Medicos_Especialidad");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.TbMedico)
                .HasForeignKey<TbMedico>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medicos_Usuario");
        });

        modelBuilder.Entity<TbRole>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__tb_Roles__2A49584C37383F8A");

            entity.ToTable("tb_Roles");

            entity.HasIndex(e => e.NombreRol, "UQ__tb_Roles__4F0B537F7641BA2C").IsUnique();

            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TbUsuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__tb_Usuar__5B65BF976963DAA3");

            entity.ToTable("tb_Usuarios");

            entity.HasIndex(e => e.Correo, "UQ__tb_Usuar__60695A19B29EFFD3").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Clave).HasMaxLength(200);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Dni)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("DNI");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.TbUsuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Rol");
        });

        modelBuilder.Entity<TbVenta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__tb_Venta__BC1240BD98DD9330");

            entity.ToTable("tb_Ventas");

            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Igv)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("IGV");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("EFECTIVO");
            entity.Property(e => e.SubTotal).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.TotalFinal).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.TbVenta)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_Paciente");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
