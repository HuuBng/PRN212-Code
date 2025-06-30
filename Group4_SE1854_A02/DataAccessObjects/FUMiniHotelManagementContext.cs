using BusinessObjects; // Reference to your BusinessObjects project
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Required for IConfiguration

namespace DataAccessObjects
{
    public class FUMiniHotelManagementContext : DbContext
    {
        // Constructor that accepts DbContextOptions for dependency injection (useful for WPF)
        public FUMiniHotelManagementContext(DbContextOptions<FUMiniHotelManagementContext> options)
            : base(options)
        {
        }

        // Default constructor for design-time tooling (e.g., migrations) or direct instantiation
        public FUMiniHotelManagementContext() { }

        // DbSet properties for each of your entities
        public DbSet<BookingDetail> BookingDetail { get; set; }
        public DbSet<BookingReservation> BookingReservation { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<RoomInformation> RoomInformation { get; set; }
        public DbSet<RoomType> RoomType { get; set; }

        // This method is called by EF Core to configure the database connection
        // It's crucial for getting the connection string from appsettings.json
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Build configuration from appsettings.json
                // Ensure appsettings.json is copied to the output directory (Copy to Output Directory = Copy if newer)
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory) // Use BaseDirectory to find appsettings.json
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                string connectionString = configuration.GetConnectionString("FUMiniHotelManagementDB");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        // This method is used to configure the model, including relationships and composite keys
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary key for BookingDetail
            modelBuilder.Entity<BookingDetail>()
                .HasKey(bd => new { bd.BookingReservationID, bd.RoomID });

            // Configure relationships

            // BookingReservation (one) to BookingDetail (many)
            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.BookingReservation)
                .WithMany(br => br.BookingDetails)
                .HasForeignKey(bd => bd.BookingReservationID)
                .OnDelete(DeleteBehavior.Cascade); // Matches SQL CASCADE

            // RoomInformation (one) to BookingDetail (many)
            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.RoomInformation)
                .WithMany(ri => ri.BookingDetails)
                .HasForeignKey(bd => bd.RoomID)
                .OnDelete(DeleteBehavior.Cascade); // Matches SQL CASCADE

            // Customer (one) to BookingReservation (many)
            modelBuilder.Entity<BookingReservation>()
                .HasOne(br => br.Customer)
                .WithMany(c => c.BookingReservations)
                .HasForeignKey(br => br.CustomerID)
                .OnDelete(DeleteBehavior.Cascade); // Matches SQL CASCADE

            // RoomType (one) to RoomInformation (many)
            modelBuilder.Entity<RoomInformation>()
                .HasOne(ri => ri.RoomType)
                .WithMany(rt => rt.RoomInformations)
                .HasForeignKey(ri => ri.RoomTypeID)
                .OnDelete(DeleteBehavior.Cascade); // Matches SQL CASCADE

            // Configure unique constraint for EmailAddress in Customer
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.EmailAddress)
                .IsUnique();

            // Seed data (optional, but good for initial testing if not using SQL script)
            // If your SQL script already inserts data, this might be redundant or cause conflicts
            // However, if you were to use EF Core migrations to create the DB, this would be important.
            // For now, we assume your SQL script is the primary data source.
        }
    }
}