using BusinessObjects;
using DataAccessObjects.Repository;
using System;
using Microsoft.EntityFrameworkCore; // Added for DbContext access in CustomerService

namespace DataAccessObjects.Repository
{
    // The Unit of Work class orchestrates the repositories and manages the DbContext
    public class UnitOfWork : IDisposable
    {
        private readonly FUMiniHotelManagementContext _context;
        private IGenericRepository<BookingDetail> _bookingDetailRepository;
        private IGenericRepository<BookingReservation> _bookingReservationRepository;
        private IGenericRepository<Customer> _customerRepository;
        private IGenericRepository<RoomInformation> _roomInformationRepository;
        private IGenericRepository<RoomType> _roomTypeRepository;

        // Public property to expose the DbContext (useful for explicit state management in services)
        public FUMiniHotelManagementContext Context => _context;

        public UnitOfWork(FUMiniHotelManagementContext context)
        {
            _context = context;
        }

        public IGenericRepository<BookingDetail> BookingDetailRepository
        {
            get
            {
                if (_bookingDetailRepository == null)
                {
                    _bookingDetailRepository = new GenericRepository<BookingDetail>(_context);
                }
                return _bookingDetailRepository;
            }
        }

        public IGenericRepository<BookingReservation> BookingReservationRepository
        {
            get
            {
                if (_bookingReservationRepository == null)
                {
                    _bookingReservationRepository = new GenericRepository<BookingReservation>(_context);
                }
                return _bookingReservationRepository;
            }
        }

        public IGenericRepository<Customer> CustomerRepository
        {
            get
            {
                if (_customerRepository == null)
                {
                    _customerRepository = new GenericRepository<Customer>(_context);
                }
                return _customerRepository;
            }
        }

        public IGenericRepository<RoomInformation> RoomInformationRepository
        {
            get
            {
                if (_roomInformationRepository == null)
                {
                    _roomInformationRepository = new GenericRepository<RoomInformation>(_context);
                }
                return _roomInformationRepository;
            }
        }

        public IGenericRepository<RoomType> RoomTypeRepository
        {
            get
            {
                if (_roomTypeRepository == null)
                {
                    _roomTypeRepository = new GenericRepository<RoomType>(_context);
                }
                return _roomTypeRepository;
            }
        }

        // Save changes to the database
        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
