using Data.DAL;
using Data.DTO;
using Group4WPF.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace Group4WPF.ViewModels
{
    public class AdminCustomerViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Customer> Customers { get; set; }

        public string SearchText { get; set; }

        // Luu gia tri that cua SelectedCustomer
        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set { _selectedCustomer = value; OnPropertyChanged(); }
        }

        //  Kết nối giữa View và ViewModel cho các thao tác như tìm, thêm, sửa, xoá.
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }


        // Lấy tất cả khách hàng đang hoạt động (status = 1) để hiển thị lên DataGrid.
        public AdminCustomerViewModel()
        {
            Customers = new ObservableCollection<Customer>(UserRepository.Instance
                .GetAllCustomers().Where(c => c.CustomerStatus == 1));

            SearchCommand = new RelayCommand(_ => Search());
            ClearCommand = new RelayCommand(_ => Clear());
            AddCommand = new RelayCommand(_ => Add());
            // Edit và Delete chỉ enable khi có khách được chọn.
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedCustomer != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedCustomer != null);
        }

        private void Search()
        {
            var keyword = SearchText?.ToLower()?.Trim();
            var filtered = UserRepository.Instance.GetAllCustomers()
                .Where(c => c.CustomerStatus == 1 &&
                    (c.CustomerFullName.ToLower().Contains(keyword) ||
                     c.EmailAddress.ToLower().Contains(keyword)))
                .ToList();
            Customers.Clear();
            foreach (var item in filtered) Customers.Add(item);
        }

        private void Clear()
        {
            SearchText = string.Empty;
            OnPropertyChanged(nameof(SearchText));
            Customers.Clear();
            foreach (var c in UserRepository.Instance.GetAllCustomers().Where(c => c.CustomerStatus == 1))
                Customers.Add(c);
        }

        private void Add()
        {
            var addWindow = new Views.AddCustomerWindow();
            if (addWindow.ShowDialog() == true)
            {
                var email = addWindow.Email.Trim().ToLower();
                var phone = addWindow.Telephone.Trim();


                bool emailExists = UserRepository.Instance.GetAllCustomers()
                    .Any(c => c.EmailAddress.Trim().ToLower() == email && c.CustomerStatus == 1);


                bool phoneExists = UserRepository.Instance.GetAllCustomers()
                    .Any(c => c.Telephone.Trim() == phone && c.CustomerStatus == 1);

                if (emailExists)
                {
                    MessageBox.Show($"Email '{addWindow.Email}' is already in use.", "Duplicate Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (phoneExists)
                {
                    MessageBox.Show($"Phone number '{addWindow.Telephone}' is already in use.", "Duplicate Phone", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newCustomer = new Customer
                {
                    CustomerID = Customers.Any() ? Customers.Max(c => c.CustomerID) + 1 : 1,
                    CustomerFullName = addWindow.CustomerFullName,
                    EmailAddress = addWindow.Email,
                    Telephone = addWindow.Telephone,
                    CustomerBirthday = addWindow.Birthday.Value,
                    CustomerStatus = 1,
                    Password = addWindow.Password
                };

                UserRepository.Instance.GetAllCustomers().Add(newCustomer);
                Customers.Add(newCustomer);

                MessageBox.Show("New customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void Edit()
        {
            var editWindow = new Views.EditCustomerWindow(SelectedCustomer);
            if (editWindow.ShowDialog() == true)
            {
                Clear();
            }
        }

        private void Delete()
        {
            if (MessageBox.Show($"Are you sure to delete {SelectedCustomer.CustomerFullName}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SelectedCustomer.CustomerStatus = 2;
                Clear();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}