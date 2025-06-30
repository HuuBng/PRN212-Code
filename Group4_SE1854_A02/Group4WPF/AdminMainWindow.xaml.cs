using System.Windows;
using Group4WPF.ViewModels;
using System; // For EventArgs

namespace Group4WPF
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
            var viewModel = new AdminMainViewModel();
            this.DataContext = viewModel;

            // Subscribe to the Logout event from the ViewModel
            viewModel.LogoutRequested += ViewModel_LogoutRequested;
        }

        private void ViewModel_LogoutRequested(object sender, EventArgs e)
        {
            // Close the Admin Main Window
            this.Close();

            // Open the Login Window again
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}