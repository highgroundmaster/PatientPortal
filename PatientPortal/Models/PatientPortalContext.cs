using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PatientPortal.Models
{
    public partial class PatientPortalContext : DbContext
    {
         public IConfigurationRoot Configuration { get; }
      
        public PatientPortalContext()
        {
        }

        public PatientPortalContext(DbContextOptions<PatientPortalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Donor> Donors { get; set; } = null!;
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public virtual DbSet<Swap> Swaps { get; set; } = null!;
        public virtual DbSet<Userinfo> Userinfos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning         optionsBuilder.UseMySql(Configuration.GetConnectionString("Default"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.29-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8mb3");

            modelBuilder.Entity<Donor>(entity =>
            {
                entity.ToTable("donor");

                entity.HasIndex(e => e.DonorId, "DonorId_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.FamilyPatientId, "FamilyPatient_idx");

                entity.Property(e => e.BloodType).HasColumnType("enum('A','B','AB','O')");

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.PastHistory).HasMaxLength(300);

                entity.Property(e => e.PatientRelation).HasColumnType("enum('father','mother','brother','sister','husband','wife','son','daughter','others')");

                entity.Property(e => e.Sex).HasColumnType("enum('male','female','others')");

                entity.Property(e => e.State).HasMaxLength(200);

                entity.HasOne(d => d.FamilyPatient)
                    .WithMany(p => p.Donors)
                    .HasForeignKey(d => d.FamilyPatientId)
                    .HasConstraintName("FamilyPatient");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("patient");

                entity.HasIndex(e => e.PatientId, "PatientId_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.PatientUserId, "PatientUserId_idx");

                entity.Property(e => e.BloodType).HasColumnType("enum('A','B','AB','O')");

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.PastHistory).HasMaxLength(300);

                entity.Property(e => e.Reports).HasColumnType("blob");

                entity.Property(e => e.Sex).HasColumnType("enum('male','female','others')");

                entity.Property(e => e.State).HasMaxLength(200);

                entity.HasOne(d => d.PatientUser)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.PatientUserId)
                    .HasConstraintName("PatientUserId");
            });

            modelBuilder.Entity<Swap>(entity =>
            {
                entity.ToTable("swap");

                entity.HasIndex(e => e.DonorId, "DonorId_idx");

                entity.HasIndex(e => e.PatientId, "PatientId_idx");

                entity.HasIndex(e => e.SwapId, "SwapId_UNIQUE")
                    .IsUnique();

                entity.HasOne(d => d.Donor)
                    .WithMany(p => p.Swaps)
                    .HasForeignKey(d => d.DonorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DonorId");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Swaps)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PatientId");
            });

            modelBuilder.Entity<Userinfo>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                entity.ToTable("userinfo");

                entity.HasIndex(e => e.UserId, "UserId_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserName, "UserName_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.EmailId).HasMaxLength(256);

                entity.Property(e => e.Password).HasMaxLength(256);

                entity.Property(e => e.PhoneNumber).HasMaxLength(45);

                entity.Property(e => e.Role).HasColumnType("enum('SuperAdmin','Admin','User')");

                entity.Property(e => e.UserName).HasMaxLength(200);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
