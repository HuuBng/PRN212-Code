using BusinessLogicLayer;
using BusinessObjects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows; // For MessageBox

namespace Group4WPF.ViewModels
{
    public class BookingDetailDialogViewModel : INotifyPropertyChanged
    {
        public event EventHandler RequestClose; // Event to request the dialog to close
        public bool? DialogResult { get; private set; } // Result to return from the dialog (true for OK, false for Cancel)

        private readonly BookingService _bookingService;
        private readonly CustomerService _customerService;
        private readonly RoomService _roomService; // To get available rooms for selection

        private BookingReservation _bookingReservation;
        public BookingReservation BookingReservation
        {
            get => _bookingReservation;
            set
            {
                _bookingReservation = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<BookingDetail> _bookingDetails;
        public ObservableCollection<BookingDetail> BookingDetails
        {
            get => _bookingDetails;
            set
            {
                _bookingDetails = value;
                OnPropertyChanged();
                UpdateTotalPrice(); // Recalculate total price when booking details change
            }
        }

        private BookingDetail _selectedBookingDetail;
        public BookingDetail SelectedBookingDetail
        {
            get => _selectedBookingDetail;
            set
            {
                _selectedBookingDetail = value;
                OnPropertyChanged();
                // No specific action here, but useful for DataGrid row selection
            }
        }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set { _customers = value; OnPropertyChanged(); }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                if (value != null)
                {
                    BookingReservation.CustomerID = value.CustomerID; // Update BookingReservation's CustomerID
                }
                ValidateProperty(nameof(SelectedCustomer)); // Validate customer selection
            }
        }

        public ObservableCollection<string> BookingStatuses { get; } = new ObservableCollection<string>
            {
                "Active", "Cancelled", "Completed" // Available booking statuses for ComboBox
            };

        private string _selectedBookingStatus;
        public string SelectedBookingStatus
        {
            get => _selectedBookingStatus;
            set
            {
                _selectedBookingStatus = value;
                OnPropertyChanged();
                // Map string status to byte for BookingReservation.BookingStatus
                // Assuming mapping: 1=Active, 0=Cancelled, 2=Completed based on typical tinyint usage
                BookingReservation.BookingStatus = (byte)(value switch
                {
                    "Active" => 1,
                    "Cancelled" => 0,
                    "Completed" => 2,
                    _ => 1 // Default to active if unknown
                });
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
            }
        }

        private string _windowTitle;
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged();
            }
        }

        private string _customerError;
        public string CustomerError
        {
            get => _customerError;
            set { _customerError = value; OnPropertyChanged(); }
        }

        private string _bookingDetailsError;
        public string BookingDetailsError
        {
            get => _bookingDetailsError;
            set { _bookingDetailsError = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddRoomToBookingCommand { get; }
        public ICommand RemoveBookingDetailCommand { get; }

        public BookingDetailDialogViewModel(BookingReservation bookingReservation, ObservableCollection<BookingDetail> bookingDetails, bool isEditing = false)
        {
            _bookingService = App.BookingServiceInstance;
            _customerService = App.CustomerServiceInstance;
            _roomService = App.RoomServiceInstance;

            BookingReservation = bookingReservation ?? new BookingReservation();
            BookingDetails = bookingDetails ?? new ObservableCollection<BookingDetail>();
            IsEditing = isEditing;
            WindowTitle = isEditing ? $"Edit Booking ID: {BookingReservation.BookingReservationID}" : "Add New Booking";

            LoadCustomers(); // Load customers for the ComboBox

            // Set initial selected customer if in editing mode
            if (IsEditing && BookingReservation.CustomerID > 0)
            {
                SelectedCustomer = Customers.FirstOrDefault(c => c.CustomerID == BookingReservation.CustomerID);
            }
            // Set initial selected status based on existing booking
            SelectedBookingStatus = BookingReservation.BookingStatus switch
            {
                1 => "Active",
                0 => "Cancelled",
                2 => "Completed",
                _ => "Active" // Default for new bookings or unknown status
            };

            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
            AddRoomToBookingCommand = new RelayCommand(ExecuteAddRoomToBooking);
            RemoveBookingDetailCommand = new RelayCommand(ExecuteRemoveBookingDetail);

            // Subscribe to property changes for validation and command re-evaluation
            BookingReservation.PropertyChanged += BookingReservation_PropertyChanged;
            BookingDetails.CollectionChanged += BookingDetails_CollectionChanged;
        }

        private void BookingReservation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged(); // Re-evaluate save button state
            if (e.PropertyName == nameof(BookingReservation.CustomerID))
            {
                ValidateProperty(nameof(SelectedCustomer)); // Validate customer selection
            }
        }

        private void BookingDetails_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTotalPrice(); // Update total price when rooms are added/removed
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged(); // Re-evaluate save button state
            ValidateProperty(nameof(BookingDetails)); // Revalidate booking details (e.g., must have at least one room)
        }

        private void UpdateTotalPrice()
        {
            // Calculate total price from all booking details
            BookingReservation.TotalPrice = BookingDetails.Sum(bd => bd.ActualPrice ?? 0);
            OnPropertyChanged(nameof(BookingReservation.TotalPrice)); // Notify UI of total price change
        }

        private void LoadCustomers()
        {
            try
            {
                Customers = new ObservableCollection<Customer>(_customerService.GetAllCustomers());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddRoomToBooking(object parameter)
        {
            // Instantiate and show the RoomSelectionDialog
            var roomSelectionViewModel = new RoomSelectionDialogViewModel(_roomService.GetAllRooms().ToList());
            var roomSelectionDialog = new Dialogs.RoomSelectionDialog(roomSelectionViewModel);

            if (roomSelectionDialog.ShowDialog() == true && roomSelectionViewModel.SelectedRoom != null)
            {
                var selectedRoom = roomSelectionViewModel.SelectedRoom;
                // Check if the room is already added to prevent duplicates
                if (BookingDetails.Any(bd => bd.RoomID == selectedRoom.RoomID))
                {
                    MessageBox.Show("This room is already added to the booking.", "Duplicate Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // For simplicity, default dates and price. In a real application,
                // you would likely prompt the user for these or have more complex logic.
                DateTime startDate = DateTime.Now.Date;
                DateTime endDate = DateTime.Now.Date.AddDays(1); // Default to one day booking
                decimal actualPrice = selectedRoom.RoomPricePerDay ?? 0; // Use room's default price

                var newDetail = new BookingDetail
                {
                    RoomID = selectedRoom.RoomID,
                    StartDate = startDate,
                    EndDate = endDate,
                    ActualPrice = actualPrice,
                    RoomInformation = selectedRoom // Link RoomInformation for display in the DataGrid
                };
                BookingDetails.Add(newDetail); // Add to observable collection
            }
        }

        private void ExecuteRemoveBookingDetail(object parameter)
        {
            if (parameter is BookingDetail detailToRemove)
            {
                BookingDetails.Remove(detailToRemove); // Remove selected booking detail
            }
        }

        private void ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(SelectedCustomer):
                    CustomerError = (SelectedCustomer == null) ? "Customer is required." : string.Empty;
                    break;
                case nameof(BookingDetails):
                    BookingDetailsError = (BookingDetails == null || BookingDetails.Count == 0) ? "At least one room is required for a booking." : string.Empty;
                    break;
                    // Add more validation logic here if needed for dates, prices of individual booking details
            }
        }

        private bool ValidateAllProperties()
        {
            CustomerError = string.Empty;
            BookingDetailsError = string.Empty;
            bool isValid = true;

            if (SelectedCustomer == null) { CustomerError = "Customer is required."; isValid = false; }
            if (BookingDetails == null || BookingDetails.Count == 0) { BookingDetailsError = "At least one room is required for a booking."; isValid = false; }

            // Basic date validation for booking details
            foreach (var detail in BookingDetails)
            {
                if (detail.StartDate >= detail.EndDate)
                {
                    BookingDetailsError = "Start Date must be before End Date for all rooms.";
                    isValid = false;
                    break;
                }
                // More advanced validation (e.g., room availability check) would typically be in BLL
            }

            return isValid;
        }

        private void ExecuteSave(object parameter)
        {
            if (ValidateAllProperties())
            {
                // Ensure TotalPrice is updated one final time before signaling save
                BookingReservation.TotalPrice = BookingDetails.Sum(bd => bd.ActualPrice ?? 0);
                DialogResult = true; // Signal success to the calling window
                RequestClose?.Invoke(this, EventArgs.Empty); // Request dialog to close
            }
            else
            {
                MessageBox.Show("Please correct the errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanExecuteSave(object parameter)
        {
            // Enable Save button if a customer is selected and there's at least one booking detail
            return SelectedCustomer != null && BookingDetails != null && BookingDetails.Any();
        }

        private void ExecuteCancel(object parameter)
        {
            DialogResult = false; // Signal cancellation
            RequestClose?.Invoke(this, EventArgs.Empty); // Request dialog to close
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}