using System.Windows;
using Group4WPF.ViewModels;

namespace Group4WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for RoomTypeManagementDialog.xaml
    /// </summary>
    public partial class RoomTypeManagementDialog : Window
    {
        public RoomTypeManagementDialog(RoomTypeManagementDialogViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}