using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel; // Required for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Required for CallerMemberName

namespace BusinessObjects
{
    public class BookingDetail : INotifyPropertyChanged // Implement INotifyPropertyChanged
    {
        // Composite Primary Key - No Identity column here, managed by BookingReservation and RoomInformation
        // This property will be part of the composite primary key in EF Core
        public int BookingReservationID { get; set; }
        // This property will be part of the composite primary key in EF Core
        public int RoomID { get; set; }

        private DateTime _startDate; // Add backing field for property change notification
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _endDate; // Add backing field for property change notification
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal? _actualPrice; // Add backing field for property change notification
        [Column(TypeName = "money")]
        public decimal? ActualPrice
        {
            get => _actualPrice;
            set
            {
                if (_actualPrice != value)
                {
                    _actualPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        // Navigation properties for relationships
        // Note: For these navigation properties, their setters don't typically need OnPropertyChanged()
        // unless you expect to swap the entire related object instance and need the UI to react.
        // For basic data binding, the property change notification is more critical for scalar properties.
        public virtual BookingReservation BookingReservation { get; set; }
        public virtual RoomInformation RoomInformation { get; set; } // Link to RoomInformation for display

        public BookingDetail()
        {
            // Initialize collections if any, though BookingDetail often doesn't have its own collections
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
