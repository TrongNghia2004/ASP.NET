﻿using luongtrongnghia.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace luongtrongnghia.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }
    }

}
