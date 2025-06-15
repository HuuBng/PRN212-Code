using System.Windows;
using Data.DAL;
using Data.DTO;

namespace Group4WPF.Views
{
    public partial class CustomerWindow : Window
    {
        private Customer _loggedInCustomer;

        public CustomerWindow(Customer customer)
        {
            InitializeComponent();
            _loggedInCustomer = customer;
        }

        private void ManageProfileButton_Click(object sender, RoutedEventArgs e)
        {
            EditCustomerWindow editWindow = new EditCustomerWindow(_loggedInCustomer);
            bool? result = editWindow.ShowDialog();

            if (result == true)
            {
                MessageBox.Show("Profile updated successfully!");
            }
        }
        private void ViewBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var bookingRepo = BookingRepository.Instance;

            var historyWindow = new BookingHistoryWindow(_loggedInCustomer, bookingRepo);
            historyWindow.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }


    }
}
 