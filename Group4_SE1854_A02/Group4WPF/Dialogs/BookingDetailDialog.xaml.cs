using System.Windows;
using Group4WPF.ViewModels;

namespace Group4WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for BookingDetailDialog.xaml
    /// </summary>
    public partial class BookingDetailDialog : Window
    {
        private BookingDetailDialogViewModel _viewModel;

        public BookingDetailDialog(BookingDetailDialogViewModel viewModel)
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