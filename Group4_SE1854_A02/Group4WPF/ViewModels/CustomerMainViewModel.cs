using BusinessObjects;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Group4WPF.ViewModels
{
    public class CustomerMainViewModel : INotifyPropertyChanged
    {
        private Customer _customer; // The authenticated customer
        public Customer Customer
        {
            get => _customer;
            set
            {
                _customer = value;
                OnPropertyChanged();
            }
        }

        private object _currentViewModel; // Holds the ViewModel for the current content pane
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        // Commands for navigation
        public ICommand ShowProfileCommand { get; }
        public ICommand ShowBookingHistoryCommand { get; }
        public ICommand LogoutCommand { get; }

        // Event for logout
        public event EventHandler LogoutRequested;

        public CustomerMainViewModel(Customer customer)
        {
            Customer = customer; // Set the authenticated customer

            // Initialize commands and assign the navigation logic
            ShowProfileCommand = new RelayCommand(p => CurrentViewModel = new CustomerProfileViewModel(Customer));
            ShowBookingHistoryCommand = new RelayCommand(p => CurrentViewModel = new CustomerBookingHistoryViewModel(Customer));
            LogoutCommand = new RelayCommand(p => LogoutRequested?.Invoke(this, EventArgs.Empty));

            // Set initial view to Customer Profile
            CurrentViewModel = new CustomerProfileViewModel(Customer);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}