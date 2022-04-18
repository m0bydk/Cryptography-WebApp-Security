using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;

    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Domain.Caesar> Caesar { get; set; }

        public DbSet<Domain.Diffie> Diffie { get; set; }
        
        public DbSet<RSA> RSAs { get; set; }
    }
