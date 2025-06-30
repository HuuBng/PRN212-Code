using BusinessLogicLayer;
using BusinessObjects;
using Group4WPF.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices; // Required for OnPropertyChanged CallerMemberName

namespace Group4WPF.ViewModels // ADD THIS NAMESPACE DECLARATION
{
    public class RoomManagementViewModel : INotifyPropertyChanged
    {
        private readonly RoomService _roomService;

        private ObservableCollection<RoomInformation> _rooms;
        public ObservableCollection<RoomInformation> Rooms
        {
            get => _rooms;
            set
            {
                _rooms = value;
                OnPropertyChanged();
            }
        }

        private RoomInformation _selectedRoom;
        public RoomInformation SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
                // This line will now work correctly because RelayCommand is public and accessible
                ((RelayCommand)UpdateRoomCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteRoomCommand).RaiseCanExecuteChanged();
            }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                SearchRoomsCommand.Execute(null);
            }
        }

        // Commands
        public ICommand LoadRoomsCommand { get; }
        public ICommand AddRoomCommand { get; }
        public ICommand UpdateRoomCommand { get; }
        public ICommand DeleteRoomCommand { get; }
        public ICommand SearchRoomsCommand { get; }
        public ICommand ManageRoomTypesCommand { get; }

        public RoomManagementViewModel()
        {
            _roomService = App.RoomServiceInstance;

            LoadRoomsCommand = new RelayCommand(ExecuteLoadRooms);
            AddRoomCommand = new RelayCommand(ExecuteAddRoom);
            UpdateRoomCommand = new RelayCommand(ExecuteUpdateRoom, CanExecuteUpdateDeleteRoom);
            DeleteRoomCommand = new RelayCommand(ExecuteDeleteRoom, CanExecuteUpdateDeleteRoom);
            SearchRoomsCommand = new RelayCommand(ExecuteSearchRooms);
            ManageRoomTypesCommand = new RelayCommand(ExecuteManageRoomTypes);

            LoadRoomsCommand.Execute(null);
        }

        private void ExecuteLoadRooms(object parameter)
        {
            try
            {
                Rooms = new ObservableCollection<RoomInformation>(_roomService.GetAllRooms());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddRoom(object parameter)
        {
            var dialogViewModel = new RoomDetailDialogViewModel(new RoomInformation());
            var dialog = new RoomDetailDialog(dialogViewModel);

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _roomService.AddRoom(dialogViewModel.Room);
                    MessageBox.Show("Room added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadRoomsCommand.Execute(null);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteUpdateRoom(object parameter)
        {
            if (SelectedRoom == null) return;

            var roomToEdit = new RoomInformation
            {
                RoomID = SelectedRoom.RoomID,
                RoomNumber = SelectedRoom.RoomNumber,
                RoomDetailDescription = SelectedRoom.RoomDetailDescription,
                RoomMaxCapacity = SelectedRoom.RoomMaxCapacity,
                RoomTypeID = SelectedRoom.RoomTypeID,
                RoomStatus = SelectedRoom.RoomStatus,
                RoomPricePerDay = SelectedRoom.RoomPricePerDay
            };

            var dialogViewModel = new RoomDetailDialogViewModel(roomToEdit, isEditing: true);
            var dialog = new RoomDetailDialog(dialogViewModel);

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _roomService.UpdateRoom(dialogViewModel.Room);
                    MessageBox.Show("Room updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadRoomsCommand.Execute(null);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteDeleteRoom(object parameter)
        {
            if (SelectedRoom == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete room '{SelectedRoom.RoomNumber}' (ID: {SelectedRoom.RoomID})? " +
                "If this room is part of a booking, its status will be changed instead of being deleted.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _roomService.DeleteRoom(SelectedRoom.RoomID);
                    MessageBox.Show("Room operation completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadRoomsCommand.Execute(null);
                    SelectedRoom = null;
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanExecuteUpdateDeleteRoom(object parameter)
        {
            return SelectedRoom != null;
        }

        private void ExecuteSearchRooms(object parameter)
        {
            try
            {
                Rooms = new ObservableCollection<RoomInformation>(_roomService.SearchRooms(SearchTerm));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteManageRoomTypes(object parameter)
        {
            var dialogViewModel = new RoomTypeManagementDialogViewModel();
            var dialog = new RoomTypeManagementDialog(dialogViewModel);
            dialog.ShowDialog();
            LoadRoomsCommand.Execute(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} // CLOSE NAMESPACE
