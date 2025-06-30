using BusinessLogicLayer;
using BusinessObjects;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows; // For MessageBox
using System.Windows.Controls; // For PasswordBox

namespace Group4WPF.ViewModels
{
    public class CustomerProfileViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _customerService;

        private Customer _customer;
        public Customer Customer // The customer object being edited
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged();
            }
        }

        // Password fields are separate as they are not directly on the Customer object (for security reasons)
        private string _currentPassword;
        public string CurrentPassword
        {
            get => _currentPassword;
            set { _currentPassword = value; OnPropertyChanged(); ValidateProperty(nameof(CurrentPassword)); /* RaiseCanExecuteChanged is called by ValidatePasswordFields() */ }
        }

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); ValidateProperty(nameof(NewPassword)); /* RaiseCanExecuteChanged is called by ValidatePasswordFields() */ }
        }

        private string _confirmNewPassword;
        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set { _confirmNewPassword = value; OnPropertyChanged(); ValidateProperty(nameof(ConfirmNewPassword)); /* RaiseCanExecuteChanged is called by ValidatePasswordFields() */ }
        }

        // Properties to hold references to PasswordBoxes from the View (set by code-behind)
        public PasswordBox CurrentPasswordBox { get; set; }
        public PasswordBox NewPasswordBox { get; set; }
        public PasswordBox ConfirmNewPasswordBox { get; set; }

        // Error properties for validation feedback
        private string _fullNameError;
        public string FullNameError
        {
            get => _fullNameError;
            set { _fullNameError = value; OnPropertyChanged(); }
        }

        private string _telephoneError;
        public string TelephoneError
        {
            get => _telephoneError;
            set { _telephoneError = value; OnPropertyChanged(); }
        }

        private string _emailError;
        public string EmailError
        {
            get => _emailError;
            set { _emailError = value; OnPropertyChanged(); }
        }

        private string _birthdayError;
        public string BirthdayError
        {
            get => _birthdayError;
            set { _birthdayError = value; OnPropertyChanged(); }
        }

        private string _currentPasswordError;
        public string CurrentPasswordError
        {
            get => _currentPasswordError;
            set { _currentPasswordError = value; OnPropertyChanged(); }
        }

        private string _newPasswordError;
        public string NewPasswordError
        {
            get => _newPasswordError;
            set { _newPasswordError = value; OnPropertyChanged(); }
        }

        private string _confirmPasswordError;
        public string ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set { _confirmPasswordError = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand SaveProfileInfoCommand { get; } // For personal information
        public ICommand SavePasswordCommand { get; }    // For password change

        public CustomerProfileViewModel(Customer customer)
        {
            _customerService = App.CustomerServiceInstance;
            Customer = customer; // The authenticated customer's data

            // Initialize new commands
            SaveProfileInfoCommand = new RelayCommand(ExecuteSaveProfileInfo, CanExecuteSaveProfileInfo);
            SavePasswordCommand = new RelayCommand(ExecuteSavePassword, CanExecuteSavePassword);

            // Subscribe to Customer property changes for real-time validation of profile info
            Customer.PropertyChanged += Customer_PropertyChanged;
        }

        private void Customer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Only validate relevant properties for profile info
            ValidateProperty(e.PropertyName);
            // Re-evaluate CanExecute for profile command
            ((RelayCommand)SaveProfileInfoCommand).RaiseCanExecuteChanged();
        }

        // Method to explicitly validate password fields (called from XAML.cs)
        public void ValidatePasswordFields()
        {
            // Update backing fields from PasswordBox, then trigger property changed
            CurrentPassword = CurrentPasswordBox?.Password;
            NewPassword = NewPasswordBox?.Password;
            ConfirmNewPassword = ConfirmNewPasswordBox?.Password;

            // Individual property validation calls already handle raising CanExecuteChanged
            // We ensure all password errors are checked here for initial state / consistency
            ValidateProperty(nameof(CurrentPassword));
            ValidateProperty(nameof(NewPassword));
            ValidateProperty(nameof(ConfirmNewPassword));

            // After all validations, force re-evaluation of the password command
            ((RelayCommand)SavePasswordCommand).RaiseCanExecuteChanged();
        }


        private void ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Customer.CustomerFullName):
                    FullNameError = string.IsNullOrWhiteSpace(Customer.CustomerFullName) ? "Full Name is required." : string.Empty;
                    break;
                case nameof(Customer.Telephone):
                    TelephoneError = string.IsNullOrWhiteSpace(Customer.Telephone) ? "Telephone is required." : string.Empty;
                    if (!string.IsNullOrWhiteSpace(Customer.Telephone) && !Regex.IsMatch(Customer.Telephone, @"^\d{10}$"))
                    {
                        TelephoneError = "Telephone must be exactly 10 digits.";
                    }
                    break;
                case nameof(Customer.EmailAddress): // Email is now editable, so validation is fully active for user input
                    EmailError = string.IsNullOrWhiteSpace(Customer.EmailAddress) ? "Email is required." : string.Empty;
                    if (!string.IsNullOrWhiteSpace(Customer.EmailAddress) && !Regex.IsMatch(Customer.EmailAddress, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        EmailError = "Invalid email format.";
                    }
                    break;
                case nameof(Customer.CustomerBirthday):
                    if (Customer.CustomerBirthday.HasValue && Customer.CustomerBirthday.Value > DateTime.Now)
                    {
                        BirthdayError = "Birthday cannot be in the future.";
                    }
                    else if (!Customer.CustomerBirthday.HasValue) // Ensure birthday is required
                    {
                        BirthdayError = "Birthday is required.";
                    }
                    else
                    {
                        BirthdayError = string.Empty;
                    }
                    break;
                case nameof(CurrentPassword):
                    // Validation for current password - only if new password fields are being used
                    if (!string.IsNullOrEmpty(NewPassword) || !string.IsNullOrEmpty(ConfirmNewPassword))
                    {
                        CurrentPasswordError = string.IsNullOrEmpty(CurrentPassword) ? "Current password is required to change password." : string.Empty;
                    }
                    else
                    {
                        CurrentPasswordError = string.Empty; // Not needed if not changing password
                    }
                    break;
                case nameof(NewPassword):
                    NewPasswordError = string.Empty; // Always clear error as there's no length limit
                    ValidateProperty(nameof(ConfirmNewPassword)); // Re-validate confirm password whenever new password changes
                    break;
                case nameof(ConfirmNewPassword):
                    if (!string.IsNullOrEmpty(NewPassword) && NewPassword != ConfirmNewPassword)
                    {
                        ConfirmPasswordError = "New password and confirm password do not match.";
                    }
                    else
                    {
                        ConfirmPasswordError = string.Empty;
                    }
                    break;
            }
        }

        // Validation for only profile info
        private bool ValidateProfileInfo()
        {
            FullNameError = string.Empty;
            TelephoneError = string.Empty;
            EmailError = string.Empty; // Clear email error for this specific validation pass
            BirthdayError = string.Empty; // Clear for this specific validation pass

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(Customer.CustomerFullName)) { FullNameError = "Full Name is required."; isValid = false; }
            if (string.IsNullOrWhiteSpace(Customer.Telephone)) { TelephoneError = "Telephone is required."; isValid = false; }
            else if (!Regex.IsMatch(Customer.Telephone, @"^\d{10}$")) { TelephoneError = "Telephone must be exactly 10 digits."; isValid = false; }

            // Email validation is now active as it's editable
            if (string.IsNullOrWhiteSpace(Customer.EmailAddress)) { EmailError = "Email is required."; isValid = false; }
            else if (!Regex.IsMatch(Customer.EmailAddress, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) { EmailError = "Invalid email format."; isValid = false; }

            if (!Customer.CustomerBirthday.HasValue) { BirthdayError = "Birthday is required."; isValid = false; }
            else if (Customer.CustomerBirthday.Value > DateTime.Now) { BirthdayError = "Birthday cannot be in the future."; isValid = false; }

            return isValid;
        }

        // Validation for only password change
        private bool ValidatePasswordChange()
        {
            CurrentPasswordError = string.Empty;
            NewPasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;

            bool isValid = true;

            // All password fields are required if any of them are populated.
            // This implicitly means the user intends to change password.
            bool isChangingPassword = !string.IsNullOrEmpty(CurrentPassword) || !string.IsNullOrEmpty(NewPassword) || !string.IsNullOrEmpty(ConfirmNewPassword);

            if (isChangingPassword)
            {
                if (string.IsNullOrEmpty(CurrentPassword)) { CurrentPasswordError = "Current password is required."; isValid = false; }
                if (string.IsNullOrEmpty(NewPassword)) { NewPasswordError = "New password is required."; isValid = false; }
                // Password length check is removed as per request.

                if (string.IsNullOrEmpty(ConfirmNewPassword)) { ConfirmPasswordError = "Confirm new password is required."; isValid = false; }
                else if (NewPassword != ConfirmNewPassword) { ConfirmPasswordError = "New password and confirm password do not match."; isValid = false; }
            }
            else
            {
                // If user is not attempting to change password, clear any errors
                CurrentPasswordError = string.Empty;
                NewPasswordError = string.Empty;
                ConfirmPasswordError = string.Empty;
            }

            return isValid;
        }

        // Execute method for saving profile info
        private void ExecuteSaveProfileInfo(object parameter)
        {
            if (!ValidateProfileInfo()) // Use specific profile validation
            {
                MessageBox.Show("Please correct the errors in your personal information before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Only update general customer information
                _customerService.UpdateCustomer(Customer);
                MessageBox.Show("Personal information updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving personal information: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Execute method for saving password
        private void ExecuteSavePassword(object parameter)
        {
            if (!ValidatePasswordChange()) // Use specific password validation
            {
                MessageBox.Show("Please correct the errors in password fields before changing password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // This ensures PasswordBox values are transferred to ViewModel properties for validation
            // and subsequent authentication/update calls.
            ValidatePasswordFields();

            try
            {
                // Authenticate current password before changing
                if (_customerService.AuthenticateCustomer(Customer.EmailAddress, CurrentPassword) != null)
                {
                    // Update password in the database
                    _customerService.UpdateCustomerPassword(Customer.CustomerID, NewPassword);
                    MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear password fields after successful change
                    if (CurrentPasswordBox != null) CurrentPasswordBox.Password = string.Empty;
                    if (NewPasswordBox != null) NewPasswordBox.Password = string.Empty;
                    if (ConfirmNewPasswordBox != null) ConfirmNewPasswordBox.Password = string.Empty;

                    // Also clear ViewModel properties
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmNewPassword = string.Empty;
                }
                else
                {
                    MessageBox.Show("Current password is incorrect.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    CurrentPasswordError = "Incorrect current password."; // Set specific error
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // CanExecute method for saving profile info
        private bool CanExecuteSaveProfileInfo(object parameter)
        {
            // Enable Save button if there are no validation errors for personal info
            return string.IsNullOrEmpty(FullNameError) &&
                   string.IsNullOrEmpty(TelephoneError) &&
                   string.IsNullOrEmpty(EmailError) && // Ensure email error is considered
                   string.IsNullOrEmpty(BirthdayError) &&
                   ValidateProfileInfo(); // Ensure all profile fields are valid
        }

        // CanExecute method for saving password
        private bool CanExecuteSavePassword(object parameter)
        {
            // Enable Change Password button if all password error properties are empty AND
            // if all password fields are non-empty AND they pass all password change validations
            // Note: Password length check is now removed
            return string.IsNullOrEmpty(CurrentPasswordError) &&
                   string.IsNullOrEmpty(NewPasswordError) &&
                   string.IsNullOrEmpty(ConfirmPasswordError) &&
                   !string.IsNullOrEmpty(CurrentPassword) && // Ensure current password is input
                   !string.IsNullOrEmpty(NewPassword) &&     // Ensure new password is input
                   !string.IsNullOrEmpty(ConfirmNewPassword) && // Ensure confirm password is input
                   ValidatePasswordChange(); // Ensure all password change rules are met (excluding length)
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
