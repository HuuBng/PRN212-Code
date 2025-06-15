using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DTO;

namespace Data.DAL
{
    public class BookingRepository
    {
        private static BookingRepository _instance;
        public static BookingRepository Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("BookingRepository is not initialized. Call Initialize() first.");
                return _instance;
            }
        }

        public static void Initialize(List<Customer> customers, List<RoomInformation> rooms)
        {
            if (_instance == null)
                _instance = new BookingRepository(customers, rooms);
        }

        // Dữ liệu và constructor 
        private List<Booking> _bookings = new List<Booking>();
        private List<Customer> _customers;
        private List<RoomInformation> _rooms;

        private BookingRepository(List<Customer> customers, List<RoomInformation> rooms)
        {
            _customers = customers;
            _rooms = rooms;

            var room1 = _rooms.First(r => r.RoomID == 1);
            var room2 = _rooms.First(r => r.RoomID == 2);
            var customer1 = _customers.First(c => c.CustomerID == 1);
            var customer2 = _customers.First(c => c.CustomerID == 2);

            // Booking 1
            var checkIn1 = DateTime.Now.AddDays(1);
            var checkOut1 = DateTime.Now.AddDays(3);
            var nights1 = (checkOut1.Date - checkIn1.Date).Days;
            _bookings.Add(new Booking
            {
                BookingID = 1,
                CustomerID = customer1.CustomerID,
                RoomID = room1.RoomID,
                BookingDate = DateTime.Now,
                CheckInDate = checkIn1,
                CheckOutDate = checkOut1,
                TotalPrice = room1.RoomPricePerDate * nights1
            });

            // Booking 2
            var checkIn2 = DateTime.Now.AddDays(4);
            var checkOut2 = DateTime.Now.AddDays(9);
            var nights2 = (checkOut2.Date - checkIn2.Date).Days;
            _bookings.Add(new Booking
            {
                BookingID = 2,
                CustomerID = customer2.CustomerID,
                RoomID = room2.RoomID,
                BookingDate = DateTime.Now,
                CheckInDate = checkIn2,
                CheckOutDate = checkOut2,
                TotalPrice = room2.RoomPricePerDate * nights2
            });

            // Booking 3
            var checkIn3 = DateTime.Now.AddDays(4);
            var checkOut3 = DateTime.Now.AddDays(12);
            var nights3 = (checkOut3.Date - checkIn3.Date).Days;
            _bookings.Add(new Booking
            {
                BookingID = 3,
                CustomerID = customer2.CustomerID,
                RoomID = room1.RoomID,
                BookingDate = DateTime.Now,
                CheckInDate = checkIn3,
                CheckOutDate = checkOut3,
                TotalPrice = room1.RoomPricePerDate * nights3
            });
        }


        public List<Booking> GetBookings()
        {
            return _bookings;
        }

        public void AddBooking(Booking booking)
        {
            booking.BookingID = _bookings.Any() ? _bookings.Max(b => b.BookingID) + 1 : 1;
            booking.Customer = _customers.FirstOrDefault(c => c.CustomerID == booking.CustomerID);
            booking.Room = _rooms.FirstOrDefault(r => r.RoomID == booking.RoomID);
            _bookings.Add(booking);
        }

        public void UpdateBooking(Booking booking)
        {
            var existing = _bookings.FirstOrDefault(b => b.BookingID == booking.BookingID);
            if (existing != null)
            {
                existing.CustomerID = booking.CustomerID;
                existing.RoomID = booking.RoomID;
                existing.BookingDate = booking.BookingDate;
                existing.CheckInDate = booking.CheckInDate;
                existing.CheckOutDate = booking.CheckOutDate;
                existing.TotalPrice = booking.TotalPrice;

                existing.Customer = _customers.FirstOrDefault(c => c.CustomerID == booking.CustomerID);
                existing.Room = _rooms.FirstOrDefault(r => r.RoomID == booking.RoomID);
            }
        }

        public void DeleteBooking(int bookingId)
        {
            var booking = _bookings.FirstOrDefault(b => b.BookingID == bookingId);
            if (booking != null)
            {
                _bookings.Remove(booking);
            }
        }
        public List<Booking> GetBookingsByPeriod(DateTime startDate, DateTime endDate)
        {
            return _bookings
                .Where(b => b.CheckInDate.Date >= startDate.Date && b.CheckOutDate.Date <= endDate.Date)
                .OrderByDescending(b => b.CheckInDate) // Hoặc: OrderByDescending(b => b.TotalPrice)
                .ToList();
        }

    }
}
