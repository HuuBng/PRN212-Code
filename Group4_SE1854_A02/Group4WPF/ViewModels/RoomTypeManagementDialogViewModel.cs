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
    public class RoomTypeManagementDialogViewModel : INotifyPropertyChanged
    {
        private readonly RoomService _roomService;

        private ObservableCollection<RoomType> _roomTypes;
        public ObservableCollection<RoomType> RoomTypes
        {
            get => _roomTypes;
            set
            {
                _roomTypes = value;
                OnPropertyChanged();
            }
        }

        private RoomType _selectedRoomType;
        public RoomType SelectedRoomType
        {
            get => _selectedRoomType;
            set
            {
                _selectedRoomType = value;
                OnPropertyChanged();
                if (value != null)
                {
                    // Create a copy for editing to avoid direct modification in the DataGrid
                    EditedRoomType = new RoomType
                    {
                        RoomTypeID = value.RoomTypeID,
                        RoomTypeName = value.RoomTypeName,
                        TypeDescription = value.TypeDescription,
                        TypeNote = value.TypeNote
                    };
                }
                else
                {
                    EditedRoomType = new RoomType(); // Clear form if no selection
                }
                // Update can execute states for commands
                ((RelayCommand)SaveRoomTypeCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteRoomTypeCommand).RaiseCanExecuteChanged();
            }
        }

        private RoomType _editedRoomType;
        public RoomType EditedRoomType
        {
            get => _editedRoomType;
            set
            {
                _editedRoomType = value;
                OnPropertyChanged();
                if (value != null)
                {
                    value.PropertyChanged -= EditedRoomType_PropertyChanged; // Unsubscribe to avoid multiple subscriptions
                    value.PropertyChanged += EditedRoomType_PropertyChanged; // Subscribe for validation
                }
                ValidateAllProperties(); // Revalidate when edited item changes
            }
        }

        private string _roomTypeNameError;
        public string RoomTypeNameError
        {
            get => _roomTypeNameError;
            set { _roomTypeNameError = value; OnPropertyChanged(); }
        }

        private string _typeDescriptionError; // NEW: Error property for TypeDescription
        public string TypeDescriptionError
        {
            get => _typeDescriptionError;
            set { _typeDescriptionError = value; OnPropertyChanged(); }
        }

        // No error property for TypeNote, as it will default to "N/A" if empty

        // Commands
        public ICommand LoadRoomTypesCommand { get; }
        public ICommand NewRoomTypeCommand { get; }
        public ICommand SaveRoomTypeCommand { get; }
        public ICommand DeleteRoomTypeCommand { get; }

        public RoomTypeManagementDialogViewModel()
        {
            _roomService = App.RoomServiceInstance;

            LoadRoomTypesCommand = new RelayCommand(ExecuteLoadRoomTypes);
            NewRoomTypeCommand = new RelayCommand(ExecuteNewRoomType);
            SaveRoomTypeCommand = new RelayCommand(ExecuteSaveRoomType, CanExecuteSaveDeleteRoomType);
            DeleteRoomTypeCommand = new RelayCommand(ExecuteDeleteRoomType, CanExecuteSaveDeleteRoomType);

            LoadRoomTypesCommand.Execute(null);
        }

        private void EditedRoomType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Re-evaluate CanExecute when properties of the edited item change
            ((RelayCommand)SaveRoomTypeCommand).RaiseCanExecuteChanged();
            ValidateProperty(e.PropertyName);
        }

        private void ValidateProperty(string propertyName)
        {
            if (EditedRoomType == null) return;

            switch (propertyName)
            {
                case nameof(EditedRoomType.RoomTypeName):
                    RoomTypeNameError = string.IsNullOrWhiteSpace(EditedRoomType.RoomTypeName) ? "Room Type Name is required." : string.Empty;
                    break;
                case nameof(EditedRoomType.TypeDescription): // NEW: Validation for TypeDescription
                    TypeDescriptionError = string.IsNullOrWhiteSpace(EditedRoomType.TypeDescription) ? "Description is required." : string.Empty;
                    break;
                case nameof(EditedRoomType.TypeNote): // NEW: No real-time error, just trigger CanExecuteChanged
                    break;
            }
        }

        private bool ValidateAllProperties()
        {
            // Clear all errors at the start of full validation
            RoomTypeNameError = string.Empty;
            TypeDescriptionError = string.Empty;

            bool isValid = true;

            if (EditedRoomType == null) return false;

            // Validate RoomTypeName
            if (string.IsNullOrWhiteSpace(EditedRoomType.RoomTypeName))
            {
                RoomTypeNameError = "Room Type Name is required.";
                isValid = false;
            }

            // NEW: Validate TypeDescription
            if (string.IsNullOrWhiteSpace(EditedRoomType.TypeDescription))
            {
                TypeDescriptionError = "Description is required.";
                isValid = false;
            }

            return isValid;
        }

        private void ExecuteLoadRoomTypes(object parameter)
        {
            try
            {
                RoomTypes = new ObservableCollection<RoomType>(_roomService.GetAllRoomTypes());
                SelectedRoomType = null; // Clear selection after loading
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading room types: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteNewRoomType(object parameter)
        {
            SelectedRoomType = null; // Clear selection
            EditedRoomType = new RoomType(); // Reset for new entry
            ValidateAllProperties(); // Clear validation errors for new entry
        }

        private void ExecuteSaveRoomType(object parameter)
        {
            // Perform full validation before attempting to save
            if (!ValidateAllProperties())
            {
                MessageBox.Show("Please correct the highlighted errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // NEW: If TypeNote is empty or whitespace, set it to "N/A"
                if (string.IsNullOrWhiteSpace(EditedRoomType.TypeNote))
                {
                    EditedRoomType.TypeNote = "N/A";
                }

                if (EditedRoomType.RoomTypeID == 0) // New Room Type
                {
                    _roomService.AddRoomType(EditedRoomType);
                    MessageBox.Show("Room Type added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Existing Room Type
                {
                    _roomService.UpdateRoomType(EditedRoomType);
                    MessageBox.Show("Room Type updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadRoomTypesCommand.Execute(null); // Reload list
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving room type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteSaveDeleteRoomType(object parameter)
        {
            if (parameter?.ToString() == "Save")
            {
                // Save button enabled if EditedRoomType is not null and passes all validations
                return EditedRoomType != null && ValidateAllProperties();
            }
            else if (parameter?.ToString() == "Delete")
            {
                return SelectedRoomType != null;
            }
            return false; // Default for other commands
        }

        private void ExecuteDeleteRoomType(object parameter)
        {
            if (SelectedRoomType == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete room type '{SelectedRoomType.RoomTypeName}' (ID: {SelectedRoomType.RoomTypeID})?\n\n" +
                "This action cannot be undone and will fail if rooms are currently assigned to this type.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _roomService.DeleteRoomType(SelectedRoomType.RoomTypeID);
                    MessageBox.Show("Room Type deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadRoomTypesCommand.Execute(null); // Reload list
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting room type: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
