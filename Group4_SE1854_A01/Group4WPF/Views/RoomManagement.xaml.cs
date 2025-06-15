using System.Windows.Controls;
using Group4WPF.ViewModels;

namespace Group4WPF.Views
{
    public partial class RoomManagement : UserControl
    {
        public RoomManagement()
        {
            InitializeComponent();
            this.DataContext = new RoomManagementViewModel();
        }
    }
}
