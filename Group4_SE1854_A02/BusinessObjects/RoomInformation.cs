using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel; // Required for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Required for CallerMemberName

namespace BusinessObjects
{
    public class RoomInformation : INotifyPropertyChanged // Implement INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // RoomID is IDENTITY(1,1) in SQL, so it's auto-incremented
        public int RoomID { get; set; }

        private string _roomNumber; // Add backing field for property change notification
        [Required]
        [StringLength(50)]
        public string RoomNumber
        {
            get => _roomNumber;
            set
            {
                if (_roomNumber != value)
                {
                    _roomNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _roomDetailDescription; // Add backing field for property change notification
       
        [StringLength(220)]
        public string RoomDetailDescription
        {
            get => _roomDetailDescription;
            set
            {
                if (_roomDetailDescription != value)
                {
                    _roomDetailDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? _roomMaxCapacity; // Add backing field for property change notification
        public int? RoomMaxCapacity
        {
            get => _roomMaxCapacity;
            set
            {
                if (_roomMaxCapacity != value)
                {
                    _roomMaxCapacity = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _roomTypeID; // Add backing field for property change notification
        [Required]
        public int RoomTypeID
        {
            get => _roomTypeID;
            set
            {
                if (_roomTypeID != value)
                {
                    _roomTypeID = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte? _roomStatus; // Add backing field for property change notification
        public byte? RoomStatus // tinyint in SQL
        {
            get => _roomStatus;
            set
            {
                if (_roomStatus != value)
                {
                    _roomStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal? _roomPricePerDay; // Add backing field for property change notification
        [Column(TypeName = "money")]
        public decimal? RoomPricePerDay
        {
            get => _roomPricePerDay;
            set
            {
                if (_roomPricePerDay != value)
                {
                    _roomPricePerDay = value;
                    OnPropertyChanged();
                }
            }
        }

        // Navigation properties for relationships
        public virtual RoomType RoomType { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }

        public RoomInformation()
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
