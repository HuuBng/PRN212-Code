using BusinessLogicLayer;
using BusinessObjects;
using Group4WPF.Dialogs; // For BookingDetailDialog
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows; // For MessageBox
using System.Windows.Input; // For ICommand

namespace Group4WPF.ViewModels // ADD THIS NAMESPACE DECLARATION
{
    public class BookingManagementViewModel : INotifyPropertyChanged
    {
        private readonly BookingService _bookingService;

        private ObservableCollection<BookingReservation> _bookingReservations;
        public ObservableCollection<BookingReservation> BookingReservations
        {
            get => _bookingReservations;
            set
            {
                _bookingReservations = value;
                OnPropertyChanged();
            }
        }

        private BookingReservation _selectedBookingReservation;
        public BookingReservation SelectedBookingReservation
        {
            get => _selectedBookingReservation;
            set
            {
                _selectedBookingReservation = value;
                OnPropertyChanged();
                // This line will now work correctly as RelayCommand is in the same namespace
                ((RelayCommand)UpdateBookingCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteBookingCommand).RaiseCanExecuteChanged();
            }
        }

        // Commands
        public ICommand LoadBookingsCommand { get; }
        public ICommand AddBookingCommand { get; }
        public ICommand UpdateBookingCommand { get; }
        public ICommand DeleteBookingCommand { get; }

        public BookingManagementViewModel()
        {
            _bookingService = App.BookingServiceInstance;

            LoadBookingsCommand = new RelayCommand(ExecuteLoadBookings);
            AddBookingCommand = new RelayCommand(ExecuteAddBooking);
            UpdateBookingCommand = new RelayCommand(ExecuteUpdateBooking, CanExecuteUpdateDeleteBooking);
            DeleteBookingCommand = new RelayCommand(ExecuteDeleteBooking, CanExecuteUpdateDeleteBooking);

            LoadBookingsCommand.Execute(null); // Load initial data when ViewModel is created
        }

        private void ExecuteLoadBookings(object parameter)
        {
            try
            {
                BookingReservations = new ObservableCollection<BookingReservation>(_bookingService.GetAllBookingReservations());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddBooking(object parameter)
        {
            var newBooking = new BookingReservation();
            var dialogViewModel = new BookingDetailDialogViewModel(newBooking, new ObservableCollection<BookingDetail>());
            var dialog = new BookingDetailDialog(dialogViewModel);

            if (dialog.ShowDialog() == true) // ShowDialog returns true if OK was pressed
            {
                try
                {
                    // The dialogViewModel.BookingReservation now contains the main booking data
                    // dialogViewModel.BookingDetails contains the list of selected rooms
                    _bookingService.AddBookingReservation(dialogViewModel.BookingReservation, dialogViewModel.BookingDetails.ToList());
                    MessageBox.Show("Booking added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBookingsCommand.Execute(null); // Reload list to show new booking
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteUpdateBooking(object parameter)
        {
            if (SelectedBookingReservation == null) return;

            // Load full details for editing (including associated BookingDetails)
            var fullBooking = _bookingService.GetBookingReservationById(SelectedBookingReservation.BookingReservationID);
            if (fullBooking == null)
            {
                MessageBox.Show("Selected booking not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadBookingsCommand.Execute(null); // Refresh list in case it was deleted externally
                return;
            }

            // Create a copy of booking details for the dialog to allow cancel/revert
            var bookingDetailsCopy = new ObservableCollection<BookingDetail>(
                fullBooking.BookingDetails.Select(bd => new BookingDetail
                {
                    BookingReservationID = bd.BookingReservationID,
                    RoomID = bd.RoomID,
                    StartDate = bd.StartDate,
                    EndDate = bd.EndDate,
                    ActualPrice = bd.ActualPrice,
                    RoomInformation = bd.RoomInformation // Important: Keep reference for display purposes in the dialog
                }).ToList()
            );

            var dialogViewModel = new BookingDetailDialogViewModel(fullBooking, bookingDetailsCopy, isEditing: true);
            var dialog = new BookingDetailDialog(dialogViewModel);

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Update the main booking reservation object and its details
                    _bookingService.UpdateBookingReservationAndDetails(dialogViewModel.BookingReservation, dialogViewModel.BookingDetails.ToList());
                    MessageBox.Show("Booking updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBookingsCommand.Execute(null); // Reload list to reflect changes
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteDeleteBooking(object parameter)
        {
            if (SelectedBookingReservation == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete booking reservation ID: {SelectedBookingReservation.BookingReservationID} " +
                $"for customer '{SelectedBookingReservation.Customer.CustomerFullName}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _bookingService.DeleteBookingReservation(SelectedBookingReservation.BookingReservationID);
                    MessageBox.Show("Booking deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBookingsCommand.Execute(null); // Reload list after deletion
                    SelectedBookingReservation = null; // Clear selection
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanExecuteUpdateDeleteBooking(object parameter)
        {
            return SelectedBookingReservation != null; // Enable Update/Delete buttons only if a booking is selected
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 
