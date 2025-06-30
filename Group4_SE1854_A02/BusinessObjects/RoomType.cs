using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel; // Required for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Required for CallerMemberName

namespace BusinessObjects
{
    public class RoomType : INotifyPropertyChanged // Implement INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // RoomTypeID is IDENTITY(1,1) in SQL, so it's auto-incremented
        public int RoomTypeID { get; set; }

        private string _roomTypeName; // Add backing field for property change notification
        [Required]
        [StringLength(50)]
        public string RoomTypeName
        {
            get => _roomTypeName;
            set
            {
                if (_roomTypeName != value)
                {
                    _roomTypeName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _typeDescription; // Add backing field for property change notification
        [StringLength(250)]
        public string TypeDescription
        {
            get => _typeDescription;
            set
            {
                if (_typeDescription != value)
                {
                    _typeDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _typeNote; // Add backing field for property change notification
        [StringLength(250)]
        public string TypeNote
        {
            get => _typeNote;
            set
            {
                if (_typeNote != value)
                {
                    _typeNote = value;
                    OnPropertyChanged();
                }
            }
        }

        // Navigation property for relationships
        public virtual ICollection<RoomInformation> RoomInformations { get; set; }

        public RoomType()
        {
            RoomInformations = new HashSet<RoomInformation>();
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
