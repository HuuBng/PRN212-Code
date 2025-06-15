using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Data.DTO;
using Data.DAL;

namespace Group4WPF.Views
{
    public partial class BookingHistoryWindow : Window
    {
        private readonly Customer _customer;
        private readonly BookingRepository _bookingRepo;

        public BookingHistoryWindow(Customer customer, BookingRepository bookingRepo)
        {
            InitializeComponent();
            _customer = customer;
            _bookingRepo = bookingRepo;

            LoadBookingHistory();
        }

        private void LoadBookingHistory()
        {
            var bookings = _bookingRepo.GetBookings()
                .Where(b => b.CustomerID == _customer.CustomerID)
                .ToList();

            BookingDataGrid.ItemsSource = bookings;
        }
    }
}
