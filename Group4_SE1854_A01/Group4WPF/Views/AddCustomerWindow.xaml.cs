using Data.DTO;
using System;
using System.Windows;

namespace Group4WPF.Views
{
    public partial class AddCustomerWindow : Window
    {
        public AddCustomerWindow()
        {
            InitializeComponent();

            BirthdayPicker.SelectedDate = DateTime.Today;
        }

      
        // Property để lấy dữ liệu khi Save
        public string CustomerFullName => FullNameTextBox.Text.Trim();
        public string Email => EmailTextBox.Text.Trim();
        public string Telephone => TelephoneTextBox.Text.Trim();
        public DateTime? Birthday => BirthdayPicker.SelectedDate;
        public string Password => PasswordBox.Password;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CustomerFullName))
            {
                MessageBox.Show("Please enter Full Name.");
                return;
            }
            if (string.IsNullOrEmpty(Email))
            {
                MessageBox.Show("Please enter Email.");
                return;
            }
            if (Birthday == null)
            {
                MessageBox.Show("Please select Birthday.");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please enter Password.");
                return;
            }

           

            this.DialogResult = true; 
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
