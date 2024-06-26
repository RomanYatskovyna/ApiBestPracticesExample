﻿// <auto-generated />
using System;
using ApiBestPracticesExample.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApiBestPracticesExample.Infrastructure.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ApiBestPracticesExample.Domain.Entities.RefreshToken", b =>
                {
                    b.Property<string>("UserEmail")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("UserEmail")
                        .HasName("PK_NewTable");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("ApiBestPracticesExample.Domain.Entities.Role", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Name");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("ApiBestPracticesExample.Domain.Entities.User", b =>
                {
                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Email");

                    b.HasIndex(new[] { "RoleName" }, "IX_Users_RoleName");

                    b.HasIndex(new[] { "PhoneNumber" }, "Index_Users_PhoneNumber")
                        .IsUnique()
                        .HasFilter("([PhoneNumber] IS NOT NULL)");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ApiBestPracticesExample.Domain.Entities.RefreshToken", b =>
                {
                    b.HasOne("ApiBestPracticesExample.Domain.Entities.User", "UserEmailNavigation")
                        .WithOne("RefreshToken")
                        .HasForeignKey("ApiBestPracticesExample.Domain.Entities.RefreshToken", "UserEmail")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_NewTable_Users");

                    b.Navigation("UserEmailNavigation");
                });

            modelBuilder.Entity("ApiBestPracticesExample.Domain.Entities.User", b =>
                {
                    b.HasOne("ApiBestPracticesExample.Domain.Entities.Role", "RoleNameNavigation")
                        .WithMany("Users")
                        .HasForeignKey("RoleName")
                        .IsRequired()
                        .HasConstraintName("FK_Users_Roles");

                    b.Navigation("RoleNameNavigation");
                });

            modelBuilder.Entity("ApiBestPracticesExample.Domain.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("ApiBestPracticesExample.Domain.Entities.User", b =>
                {
                    b.Navigation("RefreshToken");
                });
#pragma warning restore 612, 618
        }
    }
}
