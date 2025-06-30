using DataAccessObjects;
using DataAccessObjects.Repository;
using BusinessLogicLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows;

namespace Group4WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IConfiguration _configuration;
        private FUMiniHotelManagementContext _dbContext;
        private UnitOfWork _unitOfWork;
        private CustomerService _customerService;
        private RoomService _roomService;
        private BookingService _bookingService;
        private ConfigurationService _configService;

        // Public properties to access services from other parts of the application (e.g., ViewModels)
        public static CustomerService CustomerServiceInstance { get; private set; }
        public static RoomService RoomServiceInstance { get; private set; }
        public static BookingService BookingServiceInstance { get; private set; }
        public static ConfigurationService ConfigurationServiceInstance { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Load Configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Configure DbContext
            var optionsBuilder = new DbContextOptionsBuilder<FUMiniHotelManagementContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("FUMiniHotelManagementDB"));
            _dbContext = new FUMiniHotelManagementContext(optionsBuilder.Options);

            // Ensure database is created/migrated (useful for first run if not using SQL script directly)
            // Note: For this assignment, we assume the SQL script will be run manually first.
            // _dbContext.Database.Migrate(); 

            // 3. Initialize UnitOfWork and Services
            _unitOfWork = new UnitOfWork(_dbContext);
            _customerService = new CustomerService(_unitOfWork);
            _roomService = new RoomService(_unitOfWork);
            _bookingService = new BookingService(_unitOfWork);
            _configService = new ConfigurationService(); // ConfigurationService doesn't depend on UnitOfWork

            // Expose services statically for simplicity in this small application
            // In larger apps, consider a proper IoC container like Autofac or Unity.
            CustomerServiceInstance = _customerService;
            RoomServiceInstance = _roomService;
            BookingServiceInstance = _bookingService;
            ConfigurationServiceInstance = _configService;

           
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _dbContext?.Dispose(); // Dispose of the DbContext on application exit
            _unitOfWork?.Dispose(); // Dispose of the UnitOfWork
            base.OnExit(e);
        }
    }
}