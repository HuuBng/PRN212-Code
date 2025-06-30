using System.Windows;
using System.Windows.Controls; // For PasswordBox
using Group4WPF.ViewModels;
using System; // For EventArgs

namespace Group4WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private LoginViewModel _viewModel; // Keep a reference to the ViewModel

        public LoginWindow()
        {
            InitializeComponent();

            // Instantiate the ViewModel and set it as DataContext
            // The LoginViewModel's parameterless constructor will now get services from App.xaml.cs static instances
            _viewModel = new LoginViewModel();
            this.DataContext = _viewModel;

            // Pass the PasswordBox control to the ViewModel
            // Ensure your LoginWindow.xaml has an x:Name="PasswordBox" on your PasswordBox control.
            _viewModel.PasswordBox = this.PasswordBox;

            // Subscribe to the LoginSuccess event from the ViewModel
            _viewModel.LoginSuccess += ViewModel_LoginSuccess;
        }

        private void ViewModel_LoginSuccess(object sender, LoginSuccessEventArgs e)
        {
            // IMPORTANT: Hide the current login window instead of closing it.
            // This prevents the application from shutting down, as LoginWindow is the StartupUri.
            this.Hide();

            if (e.IsAdmin)
            {
                // Open Admin Main Window
                var adminMainWindow = new AdminMainWindow();
                adminMainWindow.Show();
            }
            else
            {
                // Open Customer Main Window, passing the authenticated customer object
                var customerMainWindow = new CustomerMainWindow(e.Customer);
                customerMainWindow.Show();
            }

            // Do NOT call this.Close() here. The LoginWindow should remain hidden.
        }

        // It's good practice to unsubscribe from events to prevent memory leaks.
        // This is called when the window is truly closed, e.g., by app shutdown.
        protected override void OnClosed(EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.LoginSuccess -= ViewModel_LoginSuccess;
            }
            base.OnClosed(e);
        }
    }
}
