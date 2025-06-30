using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Group4WPF.ViewModels
{
    public class RoomSelectionDialogViewModel : INotifyPropertyChanged
    {
        public event EventHandler RequestClose; // Event to request the dialog to close
        public bool? DialogResult { get; private set; } // Result to return from the dialog

        private ObservableCollection<RoomInformation> _availableRooms;
        public ObservableCollection<RoomInformation> AvailableRooms
        {
            get => _availableRooms;
            set { _availableRooms = value; OnPropertyChanged(); }
        }

        private RoomInformation _selectedRoom;
        public RoomInformation SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
                ((RelayCommand)SelectRoomCommand).RaiseCanExecuteChanged(); // Re-evaluate Select button state
            }
        }

        public ICommand SelectRoomCommand { get; }
        public ICommand CancelCommand { get; }

        public RoomSelectionDialogViewModel(List<RoomInformation> rooms)
        {
            // Filter to only show active rooms (assuming RoomStatus 1 means active)
            AvailableRooms = new ObservableCollection<RoomInformation>(rooms.Where(r => r.RoomStatus == 1));
            SelectRoomCommand = new RelayCommand(ExecuteSelectRoom, CanExecuteSelectRoom);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private void ExecuteSelectRoom(object parameter)
        {
            DialogResult = true; // Signal successful selection
            RequestClose?.Invoke(this, EventArgs.Empty); // Request dialog to close
        }

        private bool CanExecuteSelectRoom(object parameter)
        {
            return SelectedRoom != null; // Enable Select button only if a room is chosen
        }

        private void ExecuteCancel(object parameter)
        {
            DialogResult = false; // Signal cancellation
            SelectedRoom = null; // Ensure no room is returned if cancelled
            RequestClose?.Invoke(this, EventArgs.Empty); // Request dialog to close
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}