using Data.DTO;
using System.Windows;

namespace Group4WPF.Views
{
    public partial class EditCustomerWindow : Window
    {
        private Customer _customer;

        public EditCustomerWindow(Customer customer)
        {
            InitializeComponent();
            _customer = customer;

            FullNameTextBox.Text = _customer.CustomerFullName;
            EmailTextBox.Text = _customer.EmailAddress;
            TelephoneTextBox.Text = _customer.Telephone;
            BirthdayPicker.SelectedDate = _customer.CustomerBirthday;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _customer.CustomerFullName = FullNameTextBox.Text.Trim();
            _customer.EmailAddress = EmailTextBox.Text.Trim();
            _customer.Telephone = TelephoneTextBox.Text.Trim();
            if (BirthdayPicker.SelectedDate.HasValue)
                _customer.CustomerBirthday = BirthdayPicker.SelectedDate.Value;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
