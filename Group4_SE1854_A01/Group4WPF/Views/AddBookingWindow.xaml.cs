using Data.DTO;
using System;
using System.Windows;

namespace Group4WPF.Views
{
    public partial class AddBookingWindow : Window
    {
        public int CustomerID { get; private set; }
        public int RoomID { get; private set; }
        public DateTime BookingDate { get; private set; }
        public DateTime CheckInDate { get; private set; }
        public DateTime CheckOutDate { get; private set; }

        private Booking editingBooking;
        private bool isEditing = false;

        public AddBookingWindow()
        {
            InitializeComponent();

            BookingDatePicker.SelectedDate = DateTime.Today;
            CheckInDatePicker.SelectedDate = DateTime.Today.AddDays(1);
            CheckOutDatePicker.SelectedDate = DateTime.Today.AddDays(2);
        }

        public AddBookingWindow(Booking bookingToEdit)
        {
            InitializeComponent();

            isEditing = true;
            editingBooking = bookingToEdit;

            // Gán dữ liệu cũ lên UI
            CustomerIDTextBox.Text = bookingToEdit.CustomerID.ToString();
            RoomIDTextBox.Text = bookingToEdit.RoomID.ToString();
            BookingDatePicker.SelectedDate = bookingToEdit.BookingDate;
            CheckInDatePicker.SelectedDate = bookingToEdit.CheckInDate;
            CheckOutDatePicker.SelectedDate = bookingToEdit.CheckOutDate;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Parse input
                int customerId = int.Parse(CustomerIDTextBox.Text);
                int roomId = int.Parse(RoomIDTextBox.Text);
                DateTime bookingDate = BookingDatePicker.SelectedDate ?? DateTime.Today;
                DateTime checkIn = CheckInDatePicker.SelectedDate ?? DateTime.Today.AddDays(1);
                DateTime checkOut = CheckOutDatePicker.SelectedDate ?? DateTime.Today.AddDays(2);

                if (checkOut <= checkIn)
                {
                    MessageBox.Show("Check-out date must be after check-in date.");
                    return;
                }

                if (isEditing && editingBooking != null)
                {
                    // Cập nhật trực tiếp Booking đang chỉnh sửa
                    editingBooking.CustomerID = customerId;
                    editingBooking.RoomID = roomId;
                    editingBooking.BookingDate = bookingDate;
                    editingBooking.CheckInDate = checkIn;
                    editingBooking.CheckOutDate = checkOut;

                    // TotalPrice nên được tính lại bên ngoài nếu cần
                }
                else
                {
                    // Gán cho properties nếu là thêm mới
                    CustomerID = customerId;
                    RoomID = roomId;
                    BookingDate = bookingDate;
                    CheckInDate = checkIn;
                    CheckOutDate = checkOut;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input. Please check your data.\n" + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
