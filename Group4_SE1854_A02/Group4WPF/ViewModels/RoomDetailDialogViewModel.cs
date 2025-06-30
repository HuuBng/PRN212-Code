using BusinessLogicLayer;
using BusinessObjects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Group4WPF.ViewModels
{
    public class RoomDetailDialogViewModel : INotifyPropertyChanged
    {
        public event EventHandler RequestClose;
        public bool? DialogResult { get; private set; }

        private readonly RoomService _roomService;

        private RoomInformation _room;
        public RoomInformation Room
        {
            get => _room;
            set
            {
                _room = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing { get; private set; }

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

        private ObservableCollection<RoomType> _roomTypes;
        public ObservableCollection<RoomType> RoomTypes
        {
            get => _roomTypes;
            set { _roomTypes = value; OnPropertyChanged(); }
        }

        private RoomType _selectedRoomType;
        public RoomType SelectedRoomType
        {
            get => _selectedRoomType;
            set
            {
                if (_selectedRoomType != value) // Only update if value is different
                {
                    _selectedRoomType = value;
                    OnPropertyChanged();

                    // Update Room.RoomTypeID when SelectedRoomType changes
                    // This is crucial for binding to the Room object for saving
                    Room.RoomTypeID = value?.RoomTypeID ?? 0; // Use null-conditional operator and default to 0 if null

                    // Always clear the error when a selection is made or cleared
                    RoomTypeError = string.Empty;

                    // Re-evaluate CanExecute after property changes
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _roomNumberError;
        public string RoomNumberError
        {
            get => _roomNumberError;
            set { _roomNumberError = value; OnPropertyChanged(); }
        }

        // RoomDetailDescriptionError property removed as description is now nullable
        // private string _roomDetailDescriptionError;
        // public string RoomDetailDescriptionError
        // {
        //     get => _roomDetailDescriptionError;
        //     set { _roomDetailDescriptionError = value; OnPropertyChanged(); }
        // }

        private string _maxCapacityError;
        public string MaxCapacityError
        {
            get => _maxCapacityError;
            set { _maxCapacityError = value; OnPropertyChanged(); }
        }

        private string _roomTypeError;
        public string RoomTypeError
        {
            get => _roomTypeError;
            set { _roomTypeError = value; OnPropertyChanged(); }
        }

        private string _pricePerDayError;
        public string PricePerDayError
        {
            get => _pricePerDayError;
            set { _pricePerDayError = value; OnPropertyChanged(); }
        }


        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public RoomDetailDialogViewModel(RoomInformation room, bool isEditing = false)
        {
            _roomService = App.RoomServiceInstance; // Get service instance
            Room = room ?? new RoomInformation();
            IsEditing = isEditing;
            WindowTitle = isEditing ? "Edit Room" : "Add New Room";

            // Initialize commands early
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            LoadRoomTypes(); // Load room types for the ComboBox

            // Set initial selected room type ONLY AFTER RoomTypes are loaded
            if (IsEditing && Room.RoomTypeID > 0 && RoomTypes != null && RoomTypes.Any())
            {
                SelectedRoomType = RoomTypes.FirstOrDefault(rt => rt.RoomTypeID == Room.RoomTypeID);
            }
            else if (!IsEditing) // If adding mode, ensure no room type is pre-selected
            {
                SelectedRoomType = null;
                Room.RoomTypeID = 0; // Explicitly set to 0 for initial validation if no selection
            }

            Room.PropertyChanged += Room_PropertyChanged;
        }

        private void LoadRoomTypes()
        {
            try
            {
                RoomTypes = new ObservableCollection<RoomType>(_roomService.GetAllRoomTypes());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading room types: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                RoomTypes = new ObservableCollection<RoomType>(); // Initialize to empty to prevent null ref
            }
        }

        private void Room_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();

            switch (e.PropertyName)
            {
                case nameof(Room.RoomNumber):
                    RoomNumberError = string.Empty;
                    break;
                // Case for RoomDetailDescription removed as it's now nullable
                // case nameof(Room.RoomDetailDescription):
                //     RoomDetailDescriptionError = string.Empty;
                //     break;
                case nameof(Room.RoomMaxCapacity):
                    MaxCapacityError = string.Empty;
                    break;
                case nameof(Room.RoomPricePerDay):
                    PricePerDayError = string.Empty;
                    break;
            }
        }

        private bool ValidateAllProperties()
        {
            // Clear all errors at the start of full validation pass
            RoomNumberError = string.Empty;
            // RoomDetailDescriptionError removed
            MaxCapacityError = string.Empty;
            RoomTypeError = string.Empty;
            PricePerDayError = string.Empty;

            bool isValid = true;

            // 1. Room Number Validation (string, required)
            if (string.IsNullOrWhiteSpace(Room.RoomNumber))
            {
                RoomNumberError = "Room Number is required.";
                isValid = false;
            }

            // 2. Room Detail Description Validation (NO LONGER REQUIRED)
            // Removed validation for description as it can be null/empty
            // if (string.IsNullOrWhiteSpace(Room.RoomDetailDescription))
            // {
            //     RoomDetailDescriptionError = "Description is required.";
            //     isValid = false;
            // }

            // 3. Max Capacity Validation (int, positive)
            if (Room.RoomMaxCapacity == null)
            {
                MaxCapacityError = "Max Capacity is required.";
                isValid = false;
            }
            else if (Room.RoomMaxCapacity <= 0)
            {
                MaxCapacityError = "Max Capacity must be a positive integer.";
                isValid = false;
            }

            // 4. Room Type Validation (required selection)
            if (Room.RoomTypeID == 0) // Assuming 0 means no room type selected
            {
                RoomTypeError = "Room Type is required.";
                isValid = false;
            }

            // 5. Price Per Day Validation (decimal, positive)
            if (Room.RoomPricePerDay == null)
            {
                PricePerDayError = "Price Per Day is required.";
                isValid = false;
            }
            else if (Room.RoomPricePerDay <= 0)
            {
                PricePerDayError = "Price Per Day must be a positive number.";
                isValid = false;
            }

            return isValid;
        }


        private void ExecuteSave(object parameter)
        {
            // Trigger all validation and set errors if any exist
            if (ValidateAllProperties())
            {
                DialogResult = true;
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Show a general message box indicating validation errors
                MessageBox.Show("Please correct the highlighted errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanExecuteSave(object parameter)
        {
            // This is a basic check to enable the Save button without showing errors in real-time.
            // Full validation errors will only appear after the Save button is pressed.
            // This ensures the user can still attempt to save even with invalid real-time input.
            return !string.IsNullOrWhiteSpace(Room.RoomNumber) &&
                   // Removed check for description
                   // !string.IsNullOrWhiteSpace(Room.RoomDetailDescription) &&
                   Room.RoomMaxCapacity.HasValue && Room.RoomMaxCapacity > 0 &&
                   Room.RoomTypeID > 0 && // RoomTypeID will be 0 if no room type is selected
                   Room.RoomPricePerDay.HasValue && Room.RoomPricePerDay > 0;
        }

        private void ExecuteCancel(object parameter)
        {
            DialogResult = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
