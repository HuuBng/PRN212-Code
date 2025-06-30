using BusinessLogicLayer;
using BusinessObjects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows; // For MessageBox
using System.Windows.Input; // For ICommand

namespace Group4WPF.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        private readonly BookingService _bookingService;

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                // Use null-conditional operator to prevent error if command is not yet initialized
                ((RelayCommand)GenerateReportCommand)?.RaiseCanExecuteChanged();
                ValidateProperty(nameof(StartDate));
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                // Use null-conditional operator to prevent error if command is not yet initialized
                ((RelayCommand)GenerateReportCommand)?.RaiseCanExecuteChanged();
                ValidateProperty(nameof(EndDate));
            }
        }

        private ObservableCollection<BookingReservation> _reportBookings;
        public ObservableCollection<BookingReservation> ReportBookings
        {
            get => _reportBookings;
            set
            {
                _reportBookings = value;
                OnPropertyChanged();
            }
        }

        private string _dateRangeError;
        public string DateRangeError
        {
            get => _dateRangeError;
            set { _dateRangeError = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand GenerateReportCommand { get; }

        public ReportViewModel()
        {
            _bookingService = App.BookingServiceInstance;

            // FIX: Initialize the command BEFORE setting the dates.
            // This ensures GenerateReportCommand is not null when StartDate/EndDate setters are called.
            GenerateReportCommand = new RelayCommand(ExecuteGenerateReport, CanExecuteGenerateReport);

            // Set default date range (e.g., last 30 days or current month)
            // Now, when these setters are called, GenerateReportCommand will already be an instantiated object.
            StartDate = DateTime.Now.Date.AddDays(-30);
            EndDate = DateTime.Now.Date;

            // Generate report on initial load
            GenerateReportCommand.Execute(null);
        }

        private void ValidateProperty(string propertyName)
        {
            DateRangeError = string.Empty;
            if (StartDate > EndDate)
            {
                DateRangeError = "Start Date cannot be after End Date.";
            }
        }

        private bool ValidateAllProperties()
        {
            DateRangeError = string.Empty;
            bool isValid = true;

            if (StartDate > EndDate)
            {
                DateRangeError = "Start Date cannot be after End Date.";
                isValid = false;
            }

            return isValid;
        }

        private void ExecuteGenerateReport(object parameter)
        {
            if (!ValidateAllProperties())
            {
                MessageBox.Show(DateRangeError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Fetch bookings within the date range from BLL
                // The assignment specifies sorting in descending order (implicitly by BookingDate)
                var bookings = _bookingService.GetBookingReservationsByPeriod(StartDate, EndDate)
                                                .OrderByDescending(b => b.BookingDate)
                                                .ToList();
                ReportBookings = new ObservableCollection<BookingReservation>(bookings);

                if (!ReportBookings.Any())
                {
                    MessageBox.Show("No booking data found for the selected date range.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanExecuteGenerateReport(object parameter)
        {
            return StartDate <= EndDate; // Only enable if dates are valid
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} // Closing namespace
