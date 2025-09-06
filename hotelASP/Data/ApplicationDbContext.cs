using hotelASP.Entities;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Room>()
                .HasOne(u => u.RoomType)
                .WithMany(r => r.Rooms)
                .HasForeignKey(u => u.IdType);

            modelBuilder.Entity<Room>()
                .HasOne(u => u.Standard)
                .WithMany(r => r.Rooms)
                .HasForeignKey(u => u.IdStandard);

            modelBuilder.Entity<Room>()
                .HasMany(u => u.RoomDescriptions)
                .WithOne(u => u.Room)
                .HasForeignKey(rd => rd.IdRoom);

            modelBuilder.Entity<Reservation>()
                .HasOne(u => u.Room)
                .WithMany(r => r.Reservations)
                .HasForeignKey(u => u.IdRoom);

            modelBuilder.Entity<RoomDescription>()
                .HasKey(rd => new { rd.IdRoom, rd.IdOption });

            modelBuilder.Entity<RoomDescription>()
                .HasOne(rd => rd.Room)
                .WithMany(r => r.RoomDescriptions)
                .HasForeignKey(rd => rd.IdRoom);

            modelBuilder.Entity<RoomDescriptionOption>()
                .HasMany(rd => rd.RoomDescriptions)
                .WithOne(o => o.DescriptionOption)
                .HasForeignKey(rd => rd.IdOption);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserAccount> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Room> Rooms { get; set; } 
        public DbSet<RoomType> Types { get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RoomDescription> RoomDescriptions { get; set; }
        public DbSet<RoomDescriptionOption> RoomDescriptionOptions { get; set; }
    }
}
