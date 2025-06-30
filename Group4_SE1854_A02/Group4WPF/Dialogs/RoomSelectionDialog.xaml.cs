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
using System.Windows.Shapes;
using Group4WPF.ViewModels; // Added using statement for ViewModel namespace

namespace Group4WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for RoomSelectionDialog.xaml
    /// </summary>
    public partial class RoomSelectionDialog : Window
    {
        private RoomSelectionDialogViewModel _viewModel; // Declare a private field for the ViewModel

        public RoomSelectionDialog(RoomSelectionDialogViewModel viewModel) // Modified constructor to accept ViewModel
        {
            InitializeComponent();
            _viewModel = viewModel; // Assign the passed ViewModel
            this.DataContext = _viewModel; // Set the DataContext for the dialog

            // Subscribe to the RequestClose event from the ViewModel
            _viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = _viewModel.DialogResult; // Set dialog result based on ViewModel
                this.Close(); // Close the dialog
            };
        }
    }
}
