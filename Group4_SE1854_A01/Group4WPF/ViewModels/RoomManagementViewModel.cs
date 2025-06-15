using Data.DAL;
using Data.DTO;
using Group4WPF.Helpers;
using Group4WPF.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Group4WPF.ViewModels
{
    public class RoomManagementViewModel : BaseViewModel
    {
        private readonly RoomRepository _roomRepo;

        public ObservableCollection<RoomInformation> Rooms { get; set; }
        public RoomInformation SelectedRoom { get; set; }

        public string SearchKeyword { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public RoomManagementViewModel()
        {
            _roomRepo = RoomRepository.Instance;

            SearchCommand = new RelayCommand(_ => Search());
            ClearCommand = new RelayCommand(_ => Clear());
            AddCommand = new RelayCommand(_ => AddRoom());
            EditCommand = new RelayCommand(_ => EditRoom());
            DeleteCommand = new RelayCommand(_ => DeleteRoom());

            LoadRooms();
        }

        private void LoadRooms()
        {
            var rooms = _roomRepo.GetRooms();
            Rooms = new ObservableCollection<RoomInformation>(rooms);
            OnPropertyChanged(nameof(Rooms));
        }

        private void Search()
        {
            var keyword = SearchKeyword?.ToLower() ?? "";
            var filtered = _roomRepo.GetRooms().Where(r =>
                r.RoomNumber.ToLower().Contains(keyword) ||
                r.RoomDescription.ToLower().Contains(keyword)
            ).ToList();

            Rooms = new ObservableCollection<RoomInformation>(filtered);
            OnPropertyChanged(nameof(Rooms));
        }

        private void Clear()
        {
            SearchKeyword = "";
            OnPropertyChanged(nameof(SearchKeyword));
            LoadRooms();
        }

        private void AddRoom()
        {
            var addWindow = new AddEditRoomWindow(null, _roomRepo.GetRoomTypes());
            if (addWindow.ShowDialog() == true)
            {
                var newRoom = addWindow.RoomResult;

        
                bool roomExists = _roomRepo.GetRooms()
                    .Any(r => r.RoomNumber.Trim().ToLower() == newRoom.RoomNumber.Trim().ToLower());

                if (roomExists)
                {
                    MessageBox.Show($"Room number '{newRoom.RoomNumber}' already exists.", "Duplicate Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _roomRepo.AddRoom(newRoom);
                LoadRooms();
                MessageBox.Show("New room added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditRoom()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Please select a room to edit.");
                return;
            }

            var roomCopy = new RoomInformation
            {
                RoomID = SelectedRoom.RoomID,
                RoomNumber = SelectedRoom.RoomNumber,
                RoomDescription = SelectedRoom.RoomDescription,
                RoomMaxCapacity = SelectedRoom.RoomMaxCapacity,
                RoomPricePerDate = SelectedRoom.RoomPricePerDate,
                RoomStatus = SelectedRoom.RoomStatus,
                RoomTypeID = SelectedRoom.RoomTypeID,
                RoomType = SelectedRoom.RoomType
            };

            var editWindow = new AddEditRoomWindow(roomCopy, _roomRepo.GetRoomTypes());
            if (editWindow.ShowDialog() == true)
            {
                _roomRepo.UpdateRoom(editWindow.RoomResult);
                LoadRooms();
            }
        }

        private void DeleteRoom()
        {
            if (SelectedRoom == null)
            {
                MessageBox.Show("Please select a room to delete.");
                return;
            }

            if (MessageBox.Show($"Delete room {SelectedRoom.RoomNumber}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _roomRepo.DeleteRoom(SelectedRoom.RoomID);
                LoadRooms();
            }
        }
    }
}
