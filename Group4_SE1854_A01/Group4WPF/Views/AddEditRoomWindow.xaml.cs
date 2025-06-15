using Data.DTO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Group4WPF.Views
{
    public partial class AddEditRoomWindow : Window
    {
        private RoomInformation _room;
        private List<RoomType> _roomTypes;

        public RoomInformation RoomResult { get; private set; }

        // Constructor thêm mới (không truyền room)
        public AddEditRoomWindow(List<RoomType> roomTypes)
        {
            InitializeComponent();
            _roomTypes = roomTypes;
            RoomTypeComboBox.ItemsSource = _roomTypes;
            this.Title = "Add New Room";
        }

        // Constructor sửa (truyền room cần chỉnh sửa)
        public AddEditRoomWindow(RoomInformation room, List<RoomType> roomTypes)
        {
            InitializeComponent();
            _room = room;
            _roomTypes = roomTypes;
            RoomTypeComboBox.ItemsSource = _roomTypes;
            this.Title = "Edit Room";

            LoadRoomData();
        }

        private void LoadRoomData()
        {
            if (_room != null)
            {
                RoomNumberTextBox.Text = _room.RoomNumber;
                DescriptionTextBox.Text = _room.RoomDescription;
                CapacityTextBox.Text = _room.RoomMaxCapacity.ToString();
                PriceTextBox.Text = _room.RoomPricePerDate.ToString("F2");
                RoomTypeComboBox.SelectedValue = _room.RoomTypeID;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate dữ liệu cơ bản
            if (string.IsNullOrWhiteSpace(RoomNumberTextBox.Text))
            {
                MessageBox.Show("Room number is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CapacityTextBox.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Max capacity must be a positive integer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Price must be a non-negative number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RoomTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select a room type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_room == null)
            {
                _room = new RoomInformation();
                _room.RoomStatus = 1; // Active mặc định khi tạo mới
            }

            _room.RoomNumber = RoomNumberTextBox.Text.Trim();
            _room.RoomDescription = DescriptionTextBox.Text.Trim();
            _room.RoomMaxCapacity = capacity;
            _room.RoomPricePerDate = price;
            _room.RoomTypeID = (int)RoomTypeComboBox.SelectedValue;

            RoomResult = _room;
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
