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
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

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

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(32)
                        .IsUnicode(false)
                        .HasColumnType("varchar(32)");

                    b.Property<DateTime?>("RefreshTokenExpiration")
                        .HasColumnType("datetime2");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Email")
                        .HasName("PK_NewTable");

                    b.HasIndex("RoleName");

                    b.ToTable("Users");
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
#pragma warning restore 612, 618
        }
    }
}
