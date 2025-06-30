using BusinessLogicLayer;
using BusinessObjects;
using Group4WPF.Dialogs; // Added this using statement for the new Dialogs folder
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows; // For MessageBox
using System.Windows.Input; // For ICommand
using System; // Added for Exception
using System.ComponentModel;
using System.Runtime.CompilerServices; // <--- ADDED FOR OnPropertyChanged CallerMemberName

namespace Group4WPF.ViewModels
{
    public class CustomerManagementViewModel : INotifyPropertyChanged
    {
        private readonly CustomerService _customerService;

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged();
            }
        }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                // When selection changes, update can execute state of commands
                ((RelayCommand)UpdateCustomerCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCustomerCommand).RaiseCanExecuteChanged();
            }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                SearchCustomersCommand.Execute(null); // Perform search as typing
            }
        }

        // Commands
        public ICommand LoadCustomersCommand { get; }
        public ICommand AddCustomerCommand { get; }
        public ICommand UpdateCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand SearchCustomersCommand { get; }

        public CustomerManagementViewModel()
        {
            // Get CustomerService instance from App
            // Ensure App.CustomerServiceInstance is initialized before this ViewModel is created
            _customerService = App.CustomerServiceInstance;

            // Initialize Commands
            LoadCustomersCommand = new RelayCommand(ExecuteLoadCustomers);
            AddCustomerCommand = new RelayCommand(ExecuteAddCustomer);
            UpdateCustomerCommand = new RelayCommand(ExecuteUpdateCustomer, CanExecuteUpdateDeleteCustomer);
            DeleteCustomerCommand = new RelayCommand(ExecuteDeleteCustomer, CanExecuteUpdateDeleteCustomer);
            SearchCustomersCommand = new RelayCommand(ExecuteSearchCustomers);

            // Load initial data
            LoadCustomersCommand.Execute(null);
        }

        private void ExecuteLoadCustomers(object parameter)
        {
            try
            {
                Customers = new ObservableCollection<Customer>(_customerService.GetAllCustomers());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddCustomer(object parameter)
        {
            var dialogViewModel = new CustomerDetailDialogViewModel(new Customer());
            var dialog = new CustomerDetailDialog(dialogViewModel);

            if (dialog.ShowDialog() == true) // ShowDialog returns true if OK was pressed
            {
                try
                {
                    _customerService.AddCustomer(dialogViewModel.Customer);
                    MessageBox.Show("Customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCustomersCommand.Execute(null); // Reload list
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteUpdateCustomer(object parameter)
        {
            if (SelectedCustomer == null) return;

            // Create a copy for editing to avoid direct modification of the ObservableCollection item
            // if the user cancels the dialog.
            var customerToEdit = new Customer
            {
                CustomerID = SelectedCustomer.CustomerID,
                CustomerFullName = SelectedCustomer.CustomerFullName,
                Telephone = SelectedCustomer.Telephone,
                EmailAddress = SelectedCustomer.EmailAddress,
                CustomerBirthday = SelectedCustomer.CustomerBirthday,
                CustomerStatus = SelectedCustomer.CustomerStatus,
                Password = SelectedCustomer.Password // Password might not be displayed/edited, but keeping it for completeness
            };

            var dialogViewModel = new CustomerDetailDialogViewModel(customerToEdit, isEditing: true);
            var dialog = new CustomerDetailDialog(dialogViewModel);

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _customerService.UpdateCustomer(dialogViewModel.Customer);
                    MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCustomersCommand.Execute(null); // Reload list to reflect changes
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteDeleteCustomer(object parameter)
        {
            if (SelectedCustomer == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete customer '{SelectedCustomer.CustomerFullName}' (ID: {SelectedCustomer.CustomerID})?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _customerService.DeleteCustomer(SelectedCustomer.CustomerID);
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCustomersCommand.Execute(null); // Reload list
                    SelectedCustomer = null; // Clear selection
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanExecuteUpdateDeleteCustomer(object parameter)
        {
            return SelectedCustomer != null;
        }

        private void ExecuteSearchCustomers(object parameter)
        {
            try
            {
                Customers = new ObservableCollection<Customer>(_customerService.SearchCustomers(SearchTerm));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}