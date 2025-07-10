using Services;
using System.Windows;
using System.Windows.Controls;

namespace ResearchProjectManagement_NoXXXXX
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UserAccountService _userAccountService;
        public LoginWindow()
        {
            InitializeComponent();
            _userAccountService = new UserAccountService();
        }

        public void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;


            var user = _userAccountService.Login(email, password);

            if (user != null)
            {
                MessageBox.Show($"Login successful! Role: {user.Role}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                var mainWindow = new ResearchProjectManagementWindow(user.Role);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid email or password!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
