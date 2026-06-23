using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LinePerformanceDashboard.DemoModels;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblOperatorDetail> TblOperatorDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=103.9.134.216;Database=QCO;User Id=sa;Password=TKL@007#;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblOperatorDetail>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("TBL_OPERATOR_DETAIL");

            entity.Property(e => e.Oid).HasColumnName("OID");
            entity.Property(e => e.AvgCycle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AVG_CYCLE");
            entity.Property(e => e.CapacityHr)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CAPACITY_HR");
            entity.Property(e => e.LineNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LINE_NO");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.ProcessName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PROCESS_NAME");
            entity.Property(e => e.ProdDate)
                .HasColumnType("datetime")
                .HasColumnName("PROD_DATE");
            entity.Property(e => e.Remark)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REMARK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
