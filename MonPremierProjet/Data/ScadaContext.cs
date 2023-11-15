// Fichier ScadaContext.cs
using Microsoft.EntityFrameworkCore;
using MonPremierProjet.Models;

namespace MonPremierProjet.Data
{
    public partial class ScadaContext : DbContext

    {
        public ScadaContext() { }


        public ScadaContext(DbContextOptions<ScadaContext> options)
            : base(options) { }

        public virtual DbSet<AjustementEnergie> AjustementEnergies { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseNpgsql("Host=localhost:5432;Database=postgres;Username=postgres ; Password=1234");
            //}

        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<AjustementEnergie>(entity =>
            {
                entity.ToTable("ajustementenergie");
                entity.HasKey(t => t.Id);
            });


            base.OnModelCreating(modelBuilder);
        }


    }
}
