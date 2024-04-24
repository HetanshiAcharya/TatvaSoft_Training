using System;
using System.Collections.Generic;
using AssignmentEntity.DataModels;
using Microsoft.EntityFrameworkCore;

namespace AssignmentEntity.DataContext;

public partial class AssignmentDbContext : DbContext
{
    public AssignmentDbContext()
    {
    }

    public AssignmentDbContext(DbContextOptions<AssignmentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Domain> Domains { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=1234;Server=localhost;Port=5432;Database=AssignmentDb;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Domain_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Project_pkey");

            entity.Property(e => e.Id).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.DomainNavigation).WithMany(p => p.Projects).HasConstraintName("DomainId _Fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
