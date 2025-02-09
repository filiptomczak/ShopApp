﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyShop.Data;

#nullable disable

namespace MyShop.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241228152600_AddedImageUrlPropertyToProductModel")]
    partial class AddedImageUrlPropertyToProductModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MyShop.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Fantasy"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "History"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "Action"
                        });
                });

            modelBuilder.Entity("MyShop.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ListPrice")
                        .HasColumnType("float");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<double>("Price100")
                        .HasColumnType("float");

                    b.Property<double>("Price50")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Author = "J.R.R. Tolkien",
                            CategoryId = 1,
                            Description = "Lorem Ipsum",
                            ISBN = "1111PPPP",
                            ImageUrl = "",
                            ListPrice = 30.0,
                            Price = 27.0,
                            Price100 = 22.0,
                            Price50 = 25.0,
                            Title = "Lord Of The Rings"
                        },
                        new
                        {
                            Id = 2,
                            Author = "J.K. Rowling",
                            CategoryId = 1,
                            Description = "Lorem Ipsum",
                            ISBN = "2P2P2P2P",
                            ImageUrl = "",
                            ListPrice = 20.0,
                            Price = 17.0,
                            Price100 = 12.0,
                            Price50 = 15.0,
                            Title = "Harry Potter"
                        },
                        new
                        {
                            Id = 3,
                            Author = "John Steinbeck",
                            CategoryId = 1,
                            Description = "Lorem Ipsum",
                            ISBN = "11PP11PP",
                            ImageUrl = "",
                            ListPrice = 33.0,
                            Price = 30.0,
                            Price100 = 25.0,
                            Price50 = 29.0,
                            Title = "The Grapes Of Wrath"
                        });
                });

            modelBuilder.Entity("MyShop.Models.Product", b =>
                {
                    b.HasOne("MyShop.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
