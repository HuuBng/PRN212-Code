using System.ComponentModel.DataAnnotations;

namespace Core.Objects
{
    public partial class RoomInformation
    {
        public RoomInformation(string RoomNumber,
                               string? RoomDetailDescription,
                               int? RoomMaxCapacity,
                               int RoomTypeID,
                               byte? RoomStatus,
                               decimal? RoomPricePerDay)
        {
            this.RoomNumber = RoomNumber;
            this.RoomDetailDescription = RoomDetailDescription;
            this.RoomMaxCapacity = RoomMaxCapacity;
            this.RoomTypeID = RoomTypeID;
            this.RoomStatus = RoomStatus;
            this.RoomPricePerDay = RoomPricePerDay;
        }

        public RoomInformation()
        {
        }

        [Required(ErrorMessage = "Room ID is required.")]
        public int RoomID { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(50, ErrorMessage = "Room number cannot exceed 50 characters.")]
        public string RoomNumber { get; set; }

        [StringLength(220, ErrorMessage = "Room detail description cannot exceed 220 characters.")]
        public string? RoomDetailDescription { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Room max capacity must be a positive integer.")]
        public int? RoomMaxCapacity { get; set; }

        [Required(ErrorMessage = "Room type ID is required.")]
        public int RoomTypeID { get; set; }

        [Length(1, 1, ErrorMessage = "Room status must be a single byte value.")]
        public byte? RoomStatus { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Room price per day must be a positive decimal value.")]
        public decimal? RoomPricePerDay { get; set; }

        public virtual RoomType RoomType { get; set; }
    }
}
