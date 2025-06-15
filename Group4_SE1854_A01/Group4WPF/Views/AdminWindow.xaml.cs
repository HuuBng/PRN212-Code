using Data.DAL;
using System.Windows;
using System.Windows.Controls;

namespace Group4WPF.Views
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();

            
        }

        private void ManageCustomersButton_Click(object sender, RoutedEventArgs e)
        {
            // Tạo instance của UserControl AdminCustomerManagement
            var customerManagementPage = new AdminCustomerManagement();

            // Load vào ContentControl MainContent
            MainContent.Content = customerManagementPage;
        }

        private void ManageRoomsButton_Click(object sender, RoutedEventArgs e)
        {
  
            var roomManagementPage = new RoomManagement();

            // Load vào ContentControl MainContent
            MainContent.Content = roomManagementPage;
        }

        private void ManageBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            var bookingManagementWindow = new BookingManagementWindow();

            // Load vào ContentControl MainContent
            MainContent.Content = bookingManagementWindow;
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
