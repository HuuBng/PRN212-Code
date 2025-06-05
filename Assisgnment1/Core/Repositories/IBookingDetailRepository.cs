using Core.Objects;

namespace Core.Repositories
{
    public interface IBookingDetailRepository
    {
        Task<BookingDetail> GetBookingDetailByIdAsync(int bookingDetailId);
        Task<IEnumerable<BookingDetail>> GetAllBookingDetailsAsync();
        Task AddBookingDetailAsync(BookingDetail bookingDetail);
        Task UpdateBookingDetailAsync(BookingDetail bookingDetail);
        Task DeleteBookingDetailAsync(int bookingDetailId);
        Task<bool> BookingDetailExistsAsync(int bookingDetailId);
    }
}
