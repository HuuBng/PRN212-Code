using Core.Objects;

namespace Core.Repositories
{
    public interface IBookingReservationRepository
    {
        Task<BookingReservation> GetBookingReservationByIdAsync(int bookingReservationId);
        Task<IEnumerable<BookingReservation>> GetAllBookingReservationsAsync();
        Task AddBookingReservationAsync(BookingReservation bookingReservation);
        Task UpdateBookingReservationAsync(BookingReservation bookingReservation);
        Task DeleteBookingReservationAsync(int bookingReservationId);
        Task<bool> BookingReservationExistsAsync(int bookingReservationId);
    }
}
