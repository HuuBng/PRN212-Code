using System.Windows;
using Group4WPF.ViewModels;

namespace Group4WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for RoomDetailDialog.xaml
    /// </summary>
    public partial class RoomDetailDialog : Window
    {
        private RoomDetailDialogViewModel _viewModel;

        public RoomDetailDialog(RoomDetailDialogViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = _viewModel;

            _viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = _viewModel.DialogResult;
                this.Close();
            };
        }
    }
}