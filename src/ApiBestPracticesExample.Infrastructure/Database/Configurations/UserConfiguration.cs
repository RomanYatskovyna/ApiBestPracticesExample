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
            entity.HasIndex(e => e.RoleName, "IX_Users_RoleName");

            entity.HasIndex(e => e.Email, "Index_Users_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "Index_Users_PhoneNumber").IsUnique();

            entity.HasIndex(e => e.UserName, "Index_Users_UserName").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.PasswordHash).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RefreshToken)
            .HasMaxLength(32)
            .IsUnicode(false);
            entity.Property(e => e.RoleName).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(200);

            entity.HasOne(d => d.RoleNameNavigation).WithMany(p => p.Users)
            .HasForeignKey(d => d.RoleName)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_Roles");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<User> entity);
    }
}
