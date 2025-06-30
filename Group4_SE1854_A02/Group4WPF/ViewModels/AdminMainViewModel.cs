using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Group4WPF.ViewModels
{
    public class AdminMainViewModel : INotifyPropertyChanged
    {
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
        public ICommand ShowCustomerManagementCommand { get; }
        public ICommand ShowRoomManagementCommand { get; }
        public ICommand ShowBookingManagementCommand { get; }
        public ICommand ShowReportCommand { get; } // This command will now show the ReportView
        public ICommand LogoutCommand { get; }

        // Event for logout
        public event EventHandler LogoutRequested;

        public AdminMainViewModel()
        {
            // Initialize commands and assign the navigation logic
            ShowCustomerManagementCommand = new RelayCommand(p => CurrentViewModel = new CustomerManagementViewModel());
            ShowRoomManagementCommand = new RelayCommand(p => CurrentViewModel = new RoomManagementViewModel());
            ShowBookingManagementCommand = new RelayCommand(p => CurrentViewModel = new BookingManagementViewModel());
            ShowReportCommand = new RelayCommand(p => CurrentViewModel = new ReportViewModel()); 
            LogoutCommand = new RelayCommand(p => LogoutRequested?.Invoke(this, EventArgs.Empty));

            // Set initial view to Customer Management (or any default)
            CurrentViewModel = new CustomerManagementViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}