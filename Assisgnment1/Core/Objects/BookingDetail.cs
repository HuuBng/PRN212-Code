using System.ComponentModel.DataAnnotations;

namespace Core.Objects
{
    public partial class BookingDetail
    {
        public BookingDetail(int bookingReservationID,
                             int roomID,
                             DateOnly startDate,
                             DateOnly endDate,
                             decimal? actualPrice)
        {
            this.BookingReservationID = bookingReservationID;
            this.RoomID = roomID;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.ActualPrice = actualPrice;
        }

        public BookingDetail()
        {
        }

        [Required(ErrorMessage = "Booking Reservation ID is required.")]
        public int BookingReservationID { get; set; }

        [Required(ErrorMessage = "Room ID is required.")]
        public int RoomID { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [AllowedValues("EndDate", "StartDate", ErrorMessage = "End date must be greater than or equal to start date.")]
        public DateOnly EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Actual price must be a non-negative value.")]
        public decimal? ActualPrice { get; set; }

        public virtual BookingReservation BookingReservation { get; set; }
        public virtual RoomInformation RoomInformation { get; set; }
    }
}
