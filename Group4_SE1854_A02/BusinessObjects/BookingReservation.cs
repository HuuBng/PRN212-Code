using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel; // Required for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Required for CallerMemberName

namespace BusinessObjects
{
    public class BookingReservation : INotifyPropertyChanged // Implement INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // BookingReservationID is not auto-incremented in DB, it's manually assigned or comes from an external source in the SQL script
        public int BookingReservationID { get; set; }

        private DateTime? _bookingDate; // Add backing field for property change notification
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? BookingDate
        {
            get => _bookingDate;
            set
            {
                if (_bookingDate != value)
                {
                    _bookingDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal? _totalPrice; // Add backing field for property change notification
        [Column(TypeName = "money")]
        public decimal? TotalPrice
        {
            get => _totalPrice;
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _customerID; // Add backing field for property change notification
        [Required]
        public int CustomerID
        {
            get => _customerID;
            set
            {
                if (_customerID != value)
                {
                    _customerID = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte? _bookingStatus; // Add backing field for property change notification
        public byte? BookingStatus // tinyint in SQL
        {
            get => _bookingStatus;
            set
            {
                if (_bookingStatus != value)
                {
                    _bookingStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        // Navigation properties for relationships
        public virtual Customer Customer { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }

        public BookingReservation()
        {
            BookingDetails = new HashSet<BookingDetail>();
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
