using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel; // Required for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Required for CallerMemberName

namespace BusinessObjects
{
    public class Customer : INotifyPropertyChanged // Implement INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // CustomerID is IDENTITY(3,1) in SQL, so it's auto-incremented
        public int CustomerID { get; set; }

        private string _customerFullName; // Add backing field for property change notification
        [StringLength(50)]
        public string CustomerFullName
        {
            get => _customerFullName;
            set
            {
                if (_customerFullName != value)
                {
                    _customerFullName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _telephone; // Add backing field for property change notification
        [StringLength(12)]
        public string Telephone
        {
            get => _telephone;
            set
            {
                if (_telephone != value)
                {
                    _telephone = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _emailAddress; // Add backing field for property change notification
        [Required]
        [StringLength(50)]
        public string EmailAddress // Unique in SQL
        {
            get => _emailAddress;
            set
            {
                if (_emailAddress != value)
                {
                    _emailAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _customerBirthday; // Add backing field for property change notification
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? CustomerBirthday
        {
            get => _customerBirthday;
            set
            {
                if (_customerBirthday != value)
                {
                    _customerBirthday = value;
                    OnPropertyChanged();
                }
            }
        }

        private byte? _customerStatus; // Add backing field for property change notification
        public byte? CustomerStatus // tinyint in SQL
        {
            get => _customerStatus;
            set
            {
                if (_customerStatus != value)
                {
                    _customerStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _password; // Add backing field for property change notification
        [StringLength(50)]
        public string Password // Note: In a real app, passwords should be hashed and not exposed like this.
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        // Navigation property for relationships
        public virtual ICollection<BookingReservation> BookingReservations { get; set; }

        public Customer()
        {
            BookingReservations = new HashSet<BookingReservation>();
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
