using Microsoft.EntityFrameworkCore;
using DomainModels;

namespace API.Data
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingUser> BookingUsers { get; set; }
        public DbSet<BookingRoom> BookingRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BookingUser many-to-many configuration
            modelBuilder.Entity<BookingUser>()
                .HasOne(bu => bu.Booking)
                .WithMany(b => b.BookingUsers)
                .HasForeignKey(bu => bu.BookingId);

            modelBuilder.Entity<BookingUser>()
                .HasOne(bu => bu.User)
                .WithMany(u => u.BookingUsers)
                .HasForeignKey(bu => bu.UserId);

            // BookingRoom many-to-many configuration
            modelBuilder.Entity<BookingRoom>()
                .HasOne(br => br.Booking)
                .WithMany(b => b.BookingRooms)
                .HasForeignKey(br => br.BookingId);

            modelBuilder.Entity<BookingRoom>()
                .HasOne(br => br.Room)
                .WithMany(r => r.BookingRooms)
                .HasForeignKey(br => br.RoomId);
        }

        // Automatically set the Id, CreatedAt and UpdatedAt fields
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Common entity)
                {
                    if (string.IsNullOrEmpty(entity.Id))
                        entity.Id = Guid.NewGuid().ToString();

                    if (entry.State == EntityState.Added)
                        entity.CreatedAt = DateTime.UtcNow;

                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        // Override the SaveChangesAsync method to set the Id, CreatedAt and UpdatedAt fields
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Common entity)
                {
                    if (string.IsNullOrEmpty(entity.Id))
                        entity.Id = Guid.NewGuid().ToString();

                    if (entry.State == EntityState.Added)
                        entity.CreatedAt = DateTime.UtcNow;

                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
