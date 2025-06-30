using BusinessObjects;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Group4WPF.ViewModels
{
    public class BookingDetailItemViewModel : INotifyPropertyChanged
    {
        private BookingDetail _bookingDetail;
        // The underlying BusinessObjects.BookingDetail instance
        public BookingDetail BookingDetail
        {
            get => _bookingDetail;
            set
            {
                _bookingDetail = value;
                OnPropertyChanged();
            }
        }

        // Expose properties of the inner BookingDetail for direct binding in XAML (e.g., DataGrid columns)
        // These properties use the inner BookingDetail's values and raise PropertyChanged when they change.
        public int BookingReservationID => _bookingDetail.BookingReservationID;
        public int RoomID => _bookingDetail.RoomID;

        public DateTime StartDate
        {
            get => _bookingDetail.StartDate;
            set { _bookingDetail.StartDate = value; OnPropertyChanged(); }
        }
        public DateTime EndDate
        {
            get => _bookingDetail.EndDate;
            set { _bookingDetail.EndDate = value; OnPropertyChanged(); }
        }
        public decimal? ActualPrice
        {
            get => _bookingDetail.ActualPrice;
            set { _bookingDetail.ActualPrice = value; OnPropertyChanged(); }
        }
        public RoomInformation RoomInformation => _bookingDetail.RoomInformation; // Provides Room details for display (e.g., Room Number, Type)

        public BookingDetailItemViewModel(BookingDetail bookingDetail)
        {
            _bookingDetail = bookingDetail;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}