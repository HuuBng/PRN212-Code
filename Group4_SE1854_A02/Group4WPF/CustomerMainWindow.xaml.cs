using System.Windows;
using BusinessObjects;
using Group4WPF.ViewModels;
using System;

namespace Group4WPF
{
    /// <summary>
    /// Interaction logic for CustomerMainWindow.xaml
    /// </summary>
    public partial class CustomerMainWindow : Window
    {
        public CustomerMainWindow(Customer customer)
        {
            InitializeComponent();
            var viewModel = new CustomerMainViewModel(customer);
            this.DataContext = viewModel;

            // Subscribe to the Logout event from the ViewModel
            viewModel.LogoutRequested += ViewModel_LogoutRequested;
        }

        private void ViewModel_LogoutRequested(object sender, EventArgs e)
        {
            // Close the Customer Main Window
            this.Close();

            // Open the Login Window again
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}