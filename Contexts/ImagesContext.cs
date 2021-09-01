using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiffingApi.Entities;
using DiffingApi.V1.Models;

namespace DiffingApi.Contexts
{
    public class ImageContext : DbContext
    {
        public DbSet<Base64Image> Images { get; set; }

        public ImageContext(DbContextOptions<ImageContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
