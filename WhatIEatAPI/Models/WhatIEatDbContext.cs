using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WhatIEatAPI.Models
{
    public class WhatIEatDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public WhatIEatDbContext()
        { }
        
        public WhatIEatDbContext(DbContextOptions options) : base(options)
        { }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("data source=.;initial catalog = WhatIEat;integrated security = true");
           // optionsBuilder.UseSqlServer("data source=.;initial catalog = northwind;integrated security = true");
        }
    }
}
