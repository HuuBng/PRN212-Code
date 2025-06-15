using Data.BLL;
using Data.DAL;
using Group4WPF.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Group4WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var repo = UserRepository.Instance;
            _userService = new UserService(repo, config);

            BookingRepository.Initialize(UserRepository.Instance.GetAllCustomers(), RoomRepository.Instance.GetAllRooms());
        }

        public void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;

            var role = _userService.Authenticate(email, password);
            if (role == "Admin")
            {
                if (role == "Admin")
                {
                    MessageBox.Show("Welcome Admin!");
                    var adminWindow = new AdminWindow();
                    adminWindow.Show();
                    this.Close();
                }

            }
            else if (role == "Customer")
            {
                MessageBox.Show("Welcome Customer!");

                // Lấy customer từ UserRepository
                var customer = UserRepository.Instance.GetCustomerByEmailAndPassword(email, password);

                if (customer != null)
                {
                    CustomerWindow customerWindow = new CustomerWindow(customer);
                    customerWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Customer not found or inactive.");
                }
            }
            else
            {
                MessageBox.Show("Wrong Email Or Password");
            }
        }
    }
}
