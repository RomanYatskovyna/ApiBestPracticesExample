﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using ApiBestPracticesExample.Domain.Entities;
using ApiBestPracticesExample.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable disable

namespace ApiBestPracticesExample.Infrastructure.Database.Configurations
{
    public partial class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.HasKey(e => e.Email);

            entity.HasIndex(e => e.RoleName, "IX_Users_RoleName");

            entity.HasIndex(e => e.PhoneNumber, "Index_Users_PhoneNumber")
                .IsUnique()
                .HasFilter("([PhoneNumber] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasOne(d => d.RoleNameNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<User> entity);
    }
}
