using System.Windows;
using System.Windows.Controls; // Added for PasswordBox
using Group4WPF.ViewModels; // Keep this for ViewModel reference
using System; // Added for EventArgs

namespace Group4WPF.Dialogs // Changed namespace to reflect Dialogs folder
{
    /// <summary>
    /// Interaction logic for CustomerDetailDialog.xaml
    /// </summary>
    public partial class CustomerDetailDialog : Window
    {
        private CustomerDetailDialogViewModel _viewModel;

        public CustomerDetailDialog(CustomerDetailDialogViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = _viewModel;

            // Pass the PasswordBox to the ViewModel if we are in Add mode
            // Ensure CustomerPasswordBox is the x:Name of your PasswordBox in CustomerDetailDialog.xaml
            if (!_viewModel.IsEditing && this.CustomerPasswordBox != null)
            {
                _viewModel.PasswordBox = this.CustomerPasswordBox;
                // Subscribe to PasswordChanged for real-time password validation
                this.CustomerPasswordBox.PasswordChanged += CustomerPasswordBox_PasswordChanged;

                // Immediately validate password field if dialog opens in Add mode
                _viewModel.ValidatePasswordForAddMode();
            }

            _viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = _viewModel.DialogResult; // Set DialogResult based on ViewModel
                this.Close();
            };
        }

        // Event handler for the PasswordBox's PasswordChanged event
        private void CustomerPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.ValidatePasswordForAddMode(); // Call the ViewModel's method for real-time validation
            }
        }

        // Unsubscribe from events to prevent memory leaks when the dialog closes
        protected override void OnClosed(EventArgs e)
        {
            if (!_viewModel.IsEditing && this.CustomerPasswordBox != null)
            {
                this.CustomerPasswordBox.PasswordChanged -= CustomerPasswordBox_PasswordChanged;
            }
            base.OnClosed(e);
        }
    }
}
