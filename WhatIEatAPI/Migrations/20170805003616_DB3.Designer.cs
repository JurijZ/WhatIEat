using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using WhatIEatAPI.Models;

namespace WhatIEatAPI.Migrations
{
    [DbContext(typeof(WhatIEatDbContext))]
    [Migration("20170805003616_DB3")]
    partial class DB3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WhatIEatAPI.Models.Ingredient", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<short>("IngredientDangerLevel");

                    b.Property<string>("IngredientDescription")
                        .HasMaxLength(200);

                    b.Property<string>("IngredientName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<short>("IngredientPopularity");

                    b.HasKey("ID");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("WhatIEatAPI.Models.Product", b =>
                {
                    b.Property<int>("ProductID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ProductID");

                    b.ToTable("Products");
                });
        }
    }
}
