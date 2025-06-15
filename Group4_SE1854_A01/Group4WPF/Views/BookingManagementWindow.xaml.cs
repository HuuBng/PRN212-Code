using Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Data.DAL;

namespace Group4WPF.Views
{
    public partial class BookingManagementWindow : UserControl
    {
        public List<Booking> Bookings { get; set; }
        public List<RoomInformation> Rooms { get; set; }

        public Booking SelectedBooking { get; set; }

        private RoomRepository roomRepository;

        public BookingManagementWindow()
        {
            InitializeComponent();

            Bookings = BookingRepository.Instance.GetBookings();

            roomRepository = new RoomRepository();
            Rooms = roomRepository.GetAllRooms();

            this.DataContext = this;


            StartDatePicker.SelectedDate = DateTime.Today.AddMonths(-1);
            EndDatePicker.SelectedDate = DateTime.Today;
        }

        private void AddBooking_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddBookingWindow();
            addWindow.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);


            bool? result = addWindow.ShowDialog();
            if (result == true)
            {
                var selectedRoom = Rooms.FirstOrDefault(r => r.RoomID == addWindow.RoomID);
                if (selectedRoom == null)
                {
                    MessageBox.Show("Room not found!");
                    return;
                }

                int numberOfDays = (addWindow.CheckOutDate - addWindow.CheckInDate).Days;
                if (numberOfDays <= 0)
                {
                    MessageBox.Show("Invalid booking period.");
                    return;
                }

                var newBooking = new Booking
                {
                    BookingID = Bookings.Any() ? Bookings.Max(b => b.BookingID) + 1 : 1,
                    CustomerID = addWindow.CustomerID,
                    RoomID = addWindow.RoomID,
                    BookingDate = addWindow.BookingDate,
                    CheckInDate = addWindow.CheckInDate,
                    CheckOutDate = addWindow.CheckOutDate,
                    TotalPrice = selectedRoom.RoomPricePerDate * numberOfDays
                };

                Bookings.Add(newBooking);
                BookingDataGrid.Items.Refresh();
                MessageBox.Show("Added a new booking successfully!");
            }
        }

        private void UpdateBooking_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBooking == null)
            {
                MessageBox.Show("Please select a booking to update.");
                return;
            }

            var editWindow = new AddBookingWindow(SelectedBooking);
            editWindow.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                var room = Rooms.FirstOrDefault(r => r.RoomID == SelectedBooking.RoomID);
                if (room != null)
                {
                    int days = (SelectedBooking.CheckOutDate - SelectedBooking.CheckInDate).Days;
                    if (days > 0)
                    {
                        SelectedBooking.TotalPrice = room.RoomPricePerDate * days;
                    }
                }
                BookingDataGrid.Items.Refresh();
                MessageBox.Show("Booking updated successfully.");
            }
        }

        private void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBooking == null)
            {
                MessageBox.Show("Please select a booking to delete.");
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete booking ID {SelectedBooking.BookingID}?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Bookings.Remove(SelectedBooking);
                SelectedBooking = null;
                BookingDataGrid.Items.Refresh();
                MessageBox.Show("Deleted booking.");
            }
        }

        private void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            DateTime? start = StartDatePicker.SelectedDate;
            DateTime? end = EndDatePicker.SelectedDate;

            if (!start.HasValue || !end.HasValue)
            {
                MessageBox.Show("Please select both start and end dates.");
                return;
            }

            if (start > end)
            {
                MessageBox.Show("Start date must be before or equal to End date.");
                return;
            }

            var filtered = Bookings.Where(b =>
                b.CheckInDate.Date >= start.Value.Date &&
                b.CheckOutDate.Date <= end.Value.Date).ToList();

            ReportDataGrid.ItemsSource = filtered;

            if (filtered.Count == 0)
                MessageBox.Show("No bookings found in selected date range.");
        }

        
    }
}
