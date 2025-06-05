using System.ComponentModel.DataAnnotations;

namespace Core.Objects
{
    public partial class BookingReservation
    {
        public BookingReservation(int bookingReservationID,
                                  DateTime? bookingDate,
                                  decimal? totalPrice,
                                  int customerID,
                                  byte? bookingStatus)
        {
            this.BookingReservationID = bookingReservationID;
            this.BookingDate = bookingDate;
            this.TotalPrice = totalPrice;
            this.CustomerID = customerID;
            this.BookingStatus = bookingStatus;
        }

        public BookingReservation()
        {
        }

        [Required(ErrorMessage = "Booking Reservation ID is required.")]
        public int BookingReservationID { get; set; }

        public DateTime? BookingDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total price must be a non-negative value.")]
        public decimal? TotalPrice { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerID { get; set; }

        [Length(1, 1, ErrorMessage = "Booking status must be a single byte value.")]
        public byte? BookingStatus { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual List<BookingDetail> BookingDetails { get; set; }
    }
}
