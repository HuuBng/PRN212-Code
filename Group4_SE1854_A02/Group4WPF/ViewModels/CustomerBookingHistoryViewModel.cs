using BusinessLogicLayer;
using BusinessObjects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows; // For MessageBox

namespace Group4WPF.ViewModels
{
    public class CustomerBookingHistoryViewModel : INotifyPropertyChanged
    {
        private readonly BookingService _bookingService;
        private Customer _authenticatedCustomer; // The currently logged-in customer

        private ObservableCollection<BookingReservation> _customerBookings;
        public ObservableCollection<BookingReservation> CustomerBookings
        {
            get => _customerBookings;
            set
            {
                _customerBookings = value;
                OnPropertyChanged();
            }
        }

        public CustomerBookingHistoryViewModel(Customer authenticatedCustomer)
        {
            _bookingService = App.BookingServiceInstance;
            _authenticatedCustomer = authenticatedCustomer; // Store the authenticated customer

            LoadCustomerBookings(); // Load bookings when ViewModel is initialized
        }

        private void LoadCustomerBookings()
        {
            if (_authenticatedCustomer == null)
            {
                MessageBox.Show("Customer information not available to load booking history.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Fetch booking reservations for the specific customer, ordered by date descending
                CustomerBookings = new ObservableCollection<BookingReservation>(
                    _bookingService.GetBookingReservationsByCustomerId(_authenticatedCustomer.CustomerID)
                                   .OrderByDescending(b => b.BookingDate)
                );

                if (!CustomerBookings.Any())
                {
                    MessageBox.Show("You have no booking history.", "No Bookings", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading booking history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}