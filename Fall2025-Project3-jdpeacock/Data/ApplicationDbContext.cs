using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fall2025_Project3_jdpeacock.Models;

namespace Fall2025_Project3_jdpeacock.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Fall2025_Project3_jdpeacock.Models.Movie> Movie { get; set; } = default!;
        public DbSet<Fall2025_Project3_jdpeacock.Models.Actor> Actor { get; set; } = default!;
        public DbSet<Fall2025_Project3_jdpeacock.Models.ActorMovie> ActorMovie { get; set; } = default!;

        // When an actor or movie is removed, remove any associated actor/movie relationships from the ActorMovie table
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // delete row when an actor is removed
            modelBuilder.Entity<ActorMovie>()
                .HasOne(am => am.Actor)
                .WithMany()
                .HasForeignKey(am => am.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            // delete row when a Movie is removed
            modelBuilder.Entity<ActorMovie>()
                .HasOne(am => am.Movie)
                .WithMany()
                .HasForeignKey(am => am.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}