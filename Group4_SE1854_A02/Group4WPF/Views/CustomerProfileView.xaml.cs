using System.Windows.Controls;
using Group4WPF.ViewModels;
using System.Windows; // For RoutedEventArgs

namespace Group4WPF
{
    /// <summary>
    /// Interaction logic for CustomerProfileView.xaml
    /// </summary>
    public partial class CustomerProfileView : UserControl
    {
        public CustomerProfileView()
        {
            InitializeComponent();
            // The DataContext will be set by the CustomerMainViewModel
            // We subscribe to the Loaded event to ensure PasswordBoxes are available
            this.Loaded += CustomerProfileView_Loaded;
            // NEW: Subscribe to the Unloaded event
            this.Unloaded += CustomerProfileView_Unloaded;
        }

        // Event handler for the UserControl's Loaded event
        private void CustomerProfileView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CustomerProfileViewModel viewModel)
            {
                // Pass references of the PasswordBoxes from the View to the ViewModel
                // This allows the ViewModel to read the Password property which cannot be directly bound.
                viewModel.CurrentPasswordBox = this.CurrentPasswordBox;
                viewModel.NewPasswordBox = this.NewPasswordBox;
                viewModel.ConfirmNewPasswordBox = this.ConfirmNewPasswordBox;

                // Initially validate password fields on load.
                // This is important to ensure the "Change Password" button's initial state is correct.
                // However, individual error messages will only show if the user interacts (see PasswordBox_PasswordChanged).
                viewModel.ValidatePasswordFields();
            }
        }

        // Event handler for PasswordBox PasswordChanged to update ViewModel
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is CustomerProfileViewModel viewModel)
            {
                // Instead of individual assignments, call the ViewModel's central validation method.
                // This method will read all PasswordBox values, update ViewModel properties,
                // trigger validations, and re-evaluate the SavePasswordCommand.
                viewModel.ValidatePasswordFields();
            }
        }

        // RENAMED AND MODIFIED: Event handler for the UserControl's Unloaded event
        // This is where you should unsubscribe from events to prevent memory leaks.
        private void CustomerProfileView_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from the Loaded event itself
            this.Loaded -= CustomerProfileView_Loaded;

            // Unsubscribe from the Unloaded event itself to avoid multiple subscriptions if control is re-used
            this.Unloaded -= CustomerProfileView_Unloaded;

            // Unsubscribe PasswordChanged event handlers from all PasswordBoxes
            // This is important to prevent memory leaks, especially in UserControls
            // that might be loaded/unloaded multiple times.
            if (CurrentPasswordBox != null)
                CurrentPasswordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            if (NewPasswordBox != null)
                NewPasswordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            if (ConfirmNewPasswordBox != null)
                ConfirmNewPasswordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            // No base.OnUnloaded(e) call is needed here because this is an event handler, not an override.
        }
    }
}
