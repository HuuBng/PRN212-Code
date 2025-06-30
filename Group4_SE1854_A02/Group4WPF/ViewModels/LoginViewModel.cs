using BusinessLogicLayer;
using BusinessObjects;
using Group4WPF;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Controls;
using System.Windows.Input; // For ICommand

namespace Group4WPF.ViewModels
{
    // EventArgs for login success notification - DEFINED HERE ONCE
    public class LoginSuccessEventArgs : EventArgs
    {
        public bool IsAdmin { get; set; }
        public Customer Customer { get; set; }
    }

    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _customerService;
        private readonly ConfigurationService _configService;

        private string _emailAddress;
        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                _emailAddress = value;
                OnPropertyChanged();
            }
        }

        public PasswordBox PasswordBox { get; set; }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public event EventHandler<LoginSuccessEventArgs> LoginSuccess;

        public LoginViewModel()
        {
            _customerService = App.CustomerServiceInstance;
            _configService = App.ConfigurationServiceInstance;

            LoginCommand = new RelayCommand(ExecuteLogin); // This will now refer to the standalone RelayCommand
        }

        private void ExecuteLogin(object parameter)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(EmailAddress) || PasswordBox == null || string.IsNullOrEmpty(PasswordBox.Password))
            {
                ErrorMessage = "Please enter both email and password.";
                return;
            }

            var (adminEmail, adminPassword) = _configService.GetAdminAccount();

            if (EmailAddress.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) && PasswordBox.Password.Equals(adminPassword))
            {
                LoginSuccess?.Invoke(this, new LoginSuccessEventArgs { IsAdmin = true });
                return;
            }

            try
            {
                Customer customer = _customerService.AuthenticateCustomer(EmailAddress, PasswordBox.Password);
                if (customer != null)
                {
                    LoginSuccess?.Invoke(this, new LoginSuccessEventArgs { IsAdmin = false, Customer = customer });
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during login: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
