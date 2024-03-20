using System;
using System.Collections.Generic;
using HaloDocDataAccess.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HaloDocDataAccess.DataContext;

public partial class HaloDocDbContext : DbContext
{
    public HaloDocDbContext()
    {
    }

    public HaloDocDbContext(DbContextOptions<HaloDocDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AdminRegion> AdminRegions { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

    public virtual DbSet<BlockRequest> BlockRequests { get; set; }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<CaseTag> CaseTags { get; set; }

    public virtual DbSet<Concierge> Concierges { get; set; }

    public virtual DbSet<EmailLog> EmailLogs { get; set; }

    public virtual DbSet<EncounterForm> EncounterForms { get; set; }

    public virtual DbSet<HealthProfessional> HealthProfessionals { get; set; }

    public virtual DbSet<HealthProfessionalType> HealthProfessionalTypes { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Physician> Physicians { get; set; }

    public virtual DbSet<PhysicianLocation> PhysicianLocations { get; set; }

    public virtual DbSet<PhysicianNotification> PhysicianNotifications { get; set; }

    public virtual DbSet<PhysicianRegion> PhysicianRegions { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestBusiness> RequestBusinesses { get; set; }

    public virtual DbSet<RequestClient> RequestClients { get; set; }

    public virtual DbSet<RequestClosed> RequestCloseds { get; set; }

    public virtual DbSet<RequestConcierge> RequestConcierges { get; set; }

    public virtual DbSet<RequestNote> RequestNotes { get; set; }

    public virtual DbSet<RequestStatusLog> RequestStatusLogs { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<RequestWiseFile> RequestWiseFiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleMenu> RoleMenus { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<ShiftDetail> ShiftDetails { get; set; }

    public virtual DbSet<ShiftDetailRegion> ShiftDetailRegions { get; set; }

    public virtual DbSet<Smslog> Smslogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=1234;Server=localhost;Port=5432;Database=HaloDoc_db;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("Admin_pkey");

            entity.Property(e => e.AdminId).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.AdminAspNetUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AspNetUserId");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AdminModifiedByNavigations).HasConstraintName("ModifiedBy");

            entity.HasOne(d => d.Region).WithMany(p => p.Admins).HasConstraintName("RegionId");
        });

        modelBuilder.Entity<AdminRegion>(entity =>
        {
            entity.HasKey(e => e.AdminRegionId).HasName("AdminRegion_pkey");

            entity.Property(e => e.AdminRegionId).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AdminId");

            entity.HasOne(d => d.Region).WithMany(p => p.AdminRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RegionId");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetRoles_pkey");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("AspNetUsers_pkey");
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("AspNetUserRoles_pkey");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AspNetUserRoles");
        });

        modelBuilder.Entity<BlockRequest>(entity =>
        {
            entity.HasKey(e => e.BlockRequestId).HasName("BlockRequests_pkey");

            entity.Property(e => e.BlockRequestId).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Request).WithMany(p => p.BlockRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestId");
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.BusinessId).HasName("Business_pkey");

            entity.Property(e => e.BusinessId).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.Businesses).HasConstraintName("ModifiedBy");
        });

        modelBuilder.Entity<CaseTag>(entity =>
        {
            entity.HasKey(e => e.CaseTagId).HasName("CaseTag_pkey");

            entity.Property(e => e.CaseTagId).HasIdentityOptions(null, null, null, null, true, null);
        });

        modelBuilder.Entity<Concierge>(entity =>
        {
            entity.HasKey(e => e.ConciergeId).HasName("Concierge_pkey");

            entity.Property(e => e.ConciergeId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RegionId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(e => e.EmailLogId).HasName("EmailLog_pkey");

            entity.HasOne(d => d.Admin).WithMany(p => p.EmailLogs).HasConstraintName("AdminId");

            entity.HasOne(d => d.Physician).WithMany(p => p.EmailLogs).HasConstraintName("PhysicianId");

            entity.HasOne(d => d.Request).WithMany(p => p.EmailLogs).HasConstraintName("RequestId");

            entity.HasOne(d => d.Role).WithMany(p => p.EmailLogs).HasConstraintName("RoleId");
        });

        modelBuilder.Entity<EncounterForm>(entity =>
        {
            entity.HasKey(e => e.EncounterFormId).HasName("EncounterForm_pkey");

            entity.Property(e => e.EncounterFormId).HasDefaultValueSql("1");

            entity.HasOne(d => d.Admin).WithMany(p => p.EncounterForms).HasConstraintName("AdminId");

            entity.HasOne(d => d.Physician).WithMany(p => p.EncounterForms).HasConstraintName("physicianid");

            entity.HasOne(d => d.Request).WithMany(p => p.EncounterForms).HasConstraintName("requestid");
        });

        modelBuilder.Entity<HealthProfessional>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("HealthProfessionals_pkey");

            entity.Property(e => e.VendorId).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.ProfessionNavigation).WithMany(p => p.HealthProfessionals).HasConstraintName("Profession");

            entity.HasOne(d => d.Region).WithMany(p => p.HealthProfessionals).HasConstraintName("RegionId");
        });

        modelBuilder.Entity<HealthProfessionalType>(entity =>
        {
            entity.HasKey(e => e.HealthProfessionalId).HasName("HealthProfessionalType_pkey");

            entity.Property(e => e.HealthProfessionalId).HasIdentityOptions(null, null, null, null, true, null);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("Menu_pkey");

            entity.Property(e => e.MenuId).HasIdentityOptions(null, null, null, null, true, null);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("OrderDetails_pkey");

            entity.Property(e => e.Id).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Request).WithMany(p => p.OrderDetails).HasConstraintName("RequestId");

            entity.HasOne(d => d.Vendor).WithMany(p => p.OrderDetails).HasConstraintName("VendorId");
        });

        modelBuilder.Entity<Physician>(entity =>
        {
            entity.HasKey(e => e.PhysicianId).HasName("Physician_pkey");

            entity.Property(e => e.PhysicianId).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.Physicians).HasConstraintName("AspNetUserId");

            entity.HasOne(d => d.Region).WithMany(p => p.Physicians).HasConstraintName("RegionId");
        });

        modelBuilder.Entity<PhysicianLocation>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PhysicianLocation_pkey");

            entity.Property(e => e.PhysicianId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianLocations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianId");
        });

        modelBuilder.Entity<PhysicianNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PhysicianNotification_pkey");

            entity.Property(e => e.PhysicianId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianNotifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianId");
        });

        modelBuilder.Entity<PhysicianRegion>(entity =>
        {
            entity.HasKey(e => e.PhysicianRegionId).HasName("PhysicianRegion_pkey");

            entity.Property(e => e.PhysicianRegionId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.PhysicianId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RegionId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Physician).WithMany(p => p.PhysicianRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianId");

            entity.HasOne(d => d.Region).WithMany(p => p.PhysicianRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RegionId");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("Region_pkey");

            entity.Property(e => e.RegionId).HasIdentityOptions(null, null, null, null, true, null);
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("Request_pkey");

            entity.Property(e => e.RequestId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestTypeId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Physician).WithMany(p => p.Requests).HasConstraintName("PhysicianId");

            entity.HasOne(d => d.RequestType).WithMany(p => p.Requests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestTypeId");

            entity.HasOne(d => d.User).WithMany(p => p.Requests).HasConstraintName("UserId");
        });

        modelBuilder.Entity<RequestBusiness>(entity =>
        {
            entity.HasKey(e => e.RequestBusinessId).HasName("RequestBusiness_pkey");

            entity.Property(e => e.RequestBusinessId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.BusinessId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Business).WithMany(p => p.RequestBusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BusinessId");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestBusinesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestId");
        });

        modelBuilder.Entity<RequestClient>(entity =>
        {
            entity.HasKey(e => e.RequestClientId).HasName("RequestClient_pkey");

            entity.Property(e => e.RequestClientId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Region).WithMany(p => p.RequestClients).HasConstraintName("RegionId");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestClients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestId");
        });

        modelBuilder.Entity<RequestClosed>(entity =>
        {
            entity.HasKey(e => e.RequestClosedId).HasName("RequestClosed_pkey");

            entity.Property(e => e.RequestClosedId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Request).WithMany(p => p.RequestCloseds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestId");

            entity.HasOne(d => d.RequestStatusLog).WithMany(p => p.RequestCloseds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestStatusLogId");
        });

        modelBuilder.Entity<RequestConcierge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RequestConcierge_pkey");

            entity.Property(e => e.Id).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.ConciergeId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Concierge).WithMany(p => p.RequestConcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ConciergeId");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestConcierges)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestId");
        });

        modelBuilder.Entity<RequestNote>(entity =>
        {
            entity.HasKey(e => e.RequestNotesId).HasName("RequestNotes_pkey");

            entity.Property(e => e.RequestNotesId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Request).WithMany(p => p.RequestNotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestId");
        });

        modelBuilder.Entity<RequestStatusLog>(entity =>
        {
            entity.HasKey(e => e.RequestStatusLogId).HasName("RequestStatusLog_pkey");

            entity.Property(e => e.RequestStatusLogId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.AdminId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.PhysicianId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.TransToPhysicianId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Request).WithMany(p => p.RequestStatusLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestId");
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.HasKey(e => e.RequestTypeId).HasName("RequestType_pkey");

            entity.Property(e => e.RequestTypeId).HasIdentityOptions(null, null, null, null, true, null);
        });

        modelBuilder.Entity<RequestWiseFile>(entity =>
        {
            entity.HasKey(e => e.RequestWiseFileId).HasName("RequestWiseFile_pkey");

            entity.Property(e => e.RequestWiseFileId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.AdminId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.PhysicianId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("Role_pkey");

            entity.Property(e => e.RoleId).HasIdentityOptions(null, null, null, null, true, null);
        });

        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.HasKey(e => e.RoleMenuId).HasName("RoleMenu_pkey");

            entity.Property(e => e.RoleMenuId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.MenuId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MenuId");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RoleId");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("Shift_pkey");

            entity.Property(e => e.ShiftId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.PhysicianId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CreatedBy");

            entity.HasOne(d => d.Physician).WithMany(p => p.Shifts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianId");
        });

        modelBuilder.Entity<ShiftDetail>(entity =>
        {
            entity.HasKey(e => e.ShiftDetailId).HasName("ShiftDetail_pkey");

            entity.Property(e => e.ShiftDetailId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RegionId).ValueGeneratedOnAdd();
            entity.Property(e => e.ShiftId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ShiftDetails).HasConstraintName("ModifiedBy");

            entity.HasOne(d => d.Shift).WithMany(p => p.ShiftDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ShiftId");
        });

        modelBuilder.Entity<ShiftDetailRegion>(entity =>
        {
            entity.HasKey(e => e.ShiftDetailRegionId).HasName("ShiftDetailRegion_pkey");

            entity.Property(e => e.ShiftDetailRegionId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RegionId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.ShiftDetailId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Region).WithMany(p => p.ShiftDetailRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RegionId");

            entity.HasOne(d => d.ShiftDetail).WithMany(p => p.ShiftDetailRegions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ShiftDetailId");
        });

        modelBuilder.Entity<Smslog>(entity =>
        {
            entity.HasKey(e => e.SmslogId).HasName("SMSLog_pkey");

            entity.Property(e => e.SmslogId).HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.AdminId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.PhysicianId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RequestId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);
            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.Admin).WithMany(p => p.Smslogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AdminId");

            entity.HasOne(d => d.Physician).WithMany(p => p.Smslogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PhysicianId");

            entity.HasOne(d => d.Request).WithMany(p => p.Smslogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RequestId");

            entity.HasOne(d => d.Role).WithMany(p => p.Smslogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RoleId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pkey");

            entity.Property(e => e.UserId).HasIdentityOptions(null, null, null, null, true, null);

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AspNetUser");

            entity.HasOne(d => d.Region).WithMany(p => p.Users).HasConstraintName("Region");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
