using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WhatIEatAPI.Models
{
    public class NorthwindDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public NorthwindDbContext()
        { }
        
        public NorthwindDbContext(DbContextOptions options) : base(options)
        { }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("data source=.;initial catalog = northwind;integrated security = true");
            //optionsBuilder.UseSqlServer("Server =.; initial catalog = Northwind; User Id = Admin; Password = Mongodb.gps1");
        }
    }
}
