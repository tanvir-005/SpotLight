using Microsoft.EntityFrameworkCore;
using SpotLight.Models;

namespace SpotLight.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRequest> EventRequests { get; set; }
        public DbSet<EventParticipation> EventParticipations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed admin data
            modelBuilder.Entity<Admin>().HasData(
                new Admin { Id = 1, Username = "admin", Password = "admin" }
            );

            // Configure Event relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure EventParticipation relationships
            modelBuilder.Entity<EventParticipation>()
                .HasOne(ep => ep.Event)
                .WithMany(e => e.Participations)
                .HasForeignKey(ep => ep.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventParticipation>()
                .HasOne(ep => ep.Participant)
                .WithMany(p => p.Participations)
                .HasForeignKey(ep => ep.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure EventRequest relationships
            modelBuilder.Entity<EventRequest>()
                .HasOne(er => er.Organizer)
                .WithMany(o => o.EventRequests)
                .HasForeignKey(er => er.OrganizerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure required fields
            modelBuilder.Entity<Event>()
                .Property(e => e.Title)
                .IsRequired();

            modelBuilder.Entity<Organizer>()
                .Property(o => o.InstitutionName)
                .IsRequired();

            modelBuilder.Entity<Organizer>()
                .Property(o => o.Email)
                .IsRequired();

            modelBuilder.Entity<Organizer>()
                .Property(o => o.Contact)
                .IsRequired();

            modelBuilder.Entity<Organizer>()
                .Property(o => o.Address)
                .IsRequired();

            modelBuilder.Entity<Organizer>()
                .Property(o => o.Password)
                .IsRequired();

            modelBuilder.Entity<Participant>()
                .Property(p => p.Name)
                .IsRequired();

            modelBuilder.Entity<Participant>()
                .Property(p => p.Email)
                .IsRequired();

            modelBuilder.Entity<Participant>()
                .Property(p => p.Password)
                .IsRequired();

            modelBuilder.Entity<Participant>()
                .Property(p => p.Contact)
                .IsRequired();

            modelBuilder.Entity<EventRequest>()
                .Property(er => er.Title)
                .IsRequired();
        }
    }
} 