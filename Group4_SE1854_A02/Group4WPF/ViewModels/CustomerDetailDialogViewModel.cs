using BusinessObjects;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows; // For Visibility
using System.Windows.Controls;
using System.Windows.Input;

namespace Group4WPF.ViewModels
{
    public class CustomerDetailDialogViewModel : INotifyPropertyChanged
    {
        public event EventHandler RequestClose; // Event to request closing the dialog
        public bool? DialogResult { get; private set; } // Result for the dialog (true for OK, false for Cancel)

        private Customer _customer;
        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing { get; private set; } // To differentiate Add vs. Edit mode

        private string _windowTitle;
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                _windowTitle = value;
                OnPropertyChanged();
            }
        }

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

        private string _passwordError;
        public string PasswordError
        {
            get => _passwordError;
            set { _passwordError = value; OnPropertyChanged(); }
        }

        private Visibility _passwordVisibility;
        public Visibility PasswordVisibility
        {
            get => _passwordVisibility;
            set { _passwordVisibility = value; OnPropertyChanged(); }
        }

        public PasswordBox PasswordBox { get; set; } // For getting password from the dialog's PasswordBox

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CustomerDetailDialogViewModel(Customer customer, bool isEditing = false)
        {
            Customer = customer ?? new Customer();
            IsEditing = isEditing;
            WindowTitle = isEditing ? "Edit Customer" : "Add New Customer";

            // Show password field only for Add mode
            PasswordVisibility = isEditing ? Visibility.Collapsed : Visibility.Visible;

            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            // Subscribe to property changes for real-time validation
            Customer.PropertyChanged += Customer_PropertyChanged;
        }

        private void Customer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Re-evaluate CanExecute when relevant properties change
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            ValidateProperty(e.PropertyName);
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
                    // Updated Regex to allow ONLY 10 digits
                    if (!string.IsNullOrWhiteSpace(Customer.Telephone) && !Regex.IsMatch(Customer.Telephone, @"^\d{10}$"))
                    {
                        TelephoneError = "Telephone must be exactly 10 digits.";
                    }
                    break;
                case nameof(Customer.EmailAddress):
                    EmailError = string.IsNullOrWhiteSpace(Customer.EmailAddress) ? "Email is required." : string.Empty;
                    if (!string.IsNullOrWhiteSpace(Customer.EmailAddress) && !Regex.IsMatch(Customer.EmailAddress, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        EmailError = "Invalid email format.";
                    }
                    break;
                case nameof(Customer.CustomerBirthday):
                    // In real-time, only check if future date. Full "required" check is on save.
                    if (Customer.CustomerBirthday.HasValue && Customer.CustomerBirthday.Value > DateTime.Now)
                    {
                        BirthdayError = "Birthday cannot be in the future.";
                    }
                    else
                    {
                        // Clear error if valid, but won't set "required" error here
                        BirthdayError = string.Empty;
                    }
                    break;
                    // Note: Password validation for real-time needs explicit trigger from PasswordBox_PasswordChanged in View's code-behind.
                    // The case nameof(PasswordBox.Password) won't be triggered by Customer.PropertyChanged.
                    // We'll rely on ValidateAllProperties on save, and a separate method for real-time if desired.
            }
        }

        // Method to call from PasswordBox_PasswordChanged in the View's code-behind for real-time password validation
        public void ValidatePasswordForAddMode()
        {
            if (!IsEditing) // Only validate password for Add mode
            {
                if (PasswordBox == null || string.IsNullOrEmpty(PasswordBox.Password))
                {
                    PasswordError = "Password is required for new customers.";
                }
                else
                {
                    PasswordError = string.Empty;
                }
            }
            else
            {
                PasswordError = string.Empty; // Clear error if in edit mode or password isn't visible
            }
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged(); // Update Save button state
        }


        private bool ValidateAllProperties()
        {
            // Clear all errors at the start of full validation
            FullNameError = string.Empty;
            TelephoneError = string.Empty;
            EmailError = string.Empty;
            BirthdayError = string.Empty;
            PasswordError = string.Empty;

            bool isValid = true;

            // Full Name Validation
            if (string.IsNullOrWhiteSpace(Customer.CustomerFullName)) { FullNameError = "Full Name is required."; isValid = false; }

            // Telephone Validation
            if (string.IsNullOrWhiteSpace(Customer.Telephone)) { TelephoneError = "Telephone is required."; isValid = false; }
            // Updated Regex to allow ONLY 10 digits
            else if (!Regex.IsMatch(Customer.Telephone, @"^\d{10}$")) { TelephoneError = "Telephone must be exactly 10 digits."; isValid = false; }

            // Email Validation
            if (string.IsNullOrWhiteSpace(Customer.EmailAddress)) { EmailError = "Email is required."; isValid = false; }
            else if (!Regex.IsMatch(Customer.EmailAddress, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) { EmailError = "Invalid email format."; isValid = false; }

            // Birthday Validation (Now mandatory)
            if (!Customer.CustomerBirthday.HasValue) // Check if Birthday is empty/null
            {
                BirthdayError = "Birthday is required.";
                isValid = false;
            }
            else if (Customer.CustomerBirthday.Value > DateTime.Now) // Check if Birthday is in the future
            {
                BirthdayError = "Birthday cannot be in the future.";
                isValid = false;
            }

            // Password Validation (only for Add mode)
            if (!IsEditing)
            {
                if (PasswordBox == null || string.IsNullOrEmpty(PasswordBox.Password))
                {
                    PasswordError = "Password is required.";
                    isValid = false;
                }
                else
                {
                    Customer.Password = PasswordBox.Password; // Set the password to the Customer object before saving
                }
            }

            return isValid;
        }

        private void ExecuteSave(object parameter)
        {
            if (ValidateAllProperties()) // This triggers all validation and sets error messages
            {
                DialogResult = true; // Indicate success
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // Message Box will be shown only if there are validation errors
                MessageBox.Show("Please correct the highlighted errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanExecuteSave(object parameter)
        {
            // This checks basic non-empty requirements for Save button to be enabled
            // We can simplify this now that ValidateAllProperties is robust
            bool hasBasicInfo = !string.IsNullOrWhiteSpace(Customer.CustomerFullName) &&
                                // Updated to check for exactly 10 digits for telephone
                                (Customer.Telephone != null && Regex.IsMatch(Customer.Telephone, @"^\d{10}$")) &&
                                !string.IsNullOrWhiteSpace(Customer.EmailAddress) &&
                                Customer.CustomerBirthday.HasValue; // Added Birthday to basic check

            // For Add mode, password must also be present
            if (!IsEditing)
            {
                return hasBasicInfo && (PasswordBox != null && !string.IsNullOrEmpty(PasswordBox.Password));
            }
            return hasBasicInfo;
        }

        private void ExecuteCancel(object parameter)
        {
            DialogResult = false; // Indicate cancel
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
