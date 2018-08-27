﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using microblogApi.Models;

namespace microblogApi.Migrations
{
    [DbContext(typeof(MicropostContext))]
    partial class MicropostContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932");

            modelBuilder.Entity("microblogApi.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
