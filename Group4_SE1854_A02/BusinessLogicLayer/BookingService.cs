using BusinessObjects;
using DataAccessObjects.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Added for tracking state for booking details update

namespace BusinessLogicLayer
{
    // Service for managing BookingReservation and BookingDetail related business logic
    public class BookingService
    {
        private UnitOfWork _unitOfWork;

        public BookingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<BookingReservation> GetAllBookingReservations()
        {
            // Include Customer for displaying customer name in UI, and BookingDetails for details
            return _unitOfWork.BookingReservationRepository.Get(includeProperties: "Customer,BookingDetails");
        }

        public BookingReservation GetBookingReservationById(int id)
        {
            // Include Customer and BookingDetails, and also RoomInformation within BookingDetails
            return _unitOfWork.BookingReservationRepository.Get(
                br => br.BookingReservationID == id,
                includeProperties: "Customer,BookingDetails.RoomInformation,BookingDetails.RoomInformation.RoomType"
            ).FirstOrDefault();
        }

        public void AddBookingReservation(BookingReservation bookingReservation, List<BookingDetail> bookingDetails)
        {
            if (bookingReservation.CustomerID == 0) // Basic validation
            {
                throw new ArgumentException("Customer ID is required for a booking reservation.");
            }
            if (bookingDetails == null || !bookingDetails.Any())
            {
                throw new ArgumentException("At least one booking detail is required.");
            }

            // Generate a new unique BookingReservationID (since it's not auto-incremented in DB)
            // This is a simple approach; in a production system, consider a more robust ID generation strategy (e.g., GUIDs or database sequences)
            int newId = 1;
            if (_unitOfWork.BookingReservationRepository.GetAll().Any())
            {
                newId = _unitOfWork.BookingReservationRepository.GetAll().Max(br => br.BookingReservationID) + 1;
            }
            bookingReservation.BookingReservationID = newId;

            bookingReservation.BookingDate = DateTime.Now.Date; // Set booking date to current date
            bookingReservation.BookingStatus = 1; // Default status for new booking (e.g., active)

            _unitOfWork.BookingReservationRepository.Add(bookingReservation);

            // Add booking details
            foreach (var detail in bookingDetails)
            {
                detail.BookingReservationID = bookingReservation.BookingReservationID; // Link details to the new reservation
                // Optional: Calculate ActualPrice if not provided, based on RoomPricePerDay and duration
                if (!detail.ActualPrice.HasValue)
                {
                    var room = _unitOfWork.RoomInformationRepository.GetById(detail.RoomID);
                    if (room != null && room.RoomPricePerDay.HasValue)
                    {
                        var duration = (detail.EndDate - detail.StartDate).Days;
                        detail.ActualPrice = room.RoomPricePerDay.Value * (duration > 0 ? duration : 1);
                    }
                    else
                    {
                        // Fallback if room or price is missing (or throw error)
                        detail.ActualPrice = 0;
                    }
                }
                _unitOfWork.BookingDetailRepository.Add(detail);
            }

            // Calculate total price based on added details
            bookingReservation.TotalPrice = bookingDetails.Sum(bd => bd.ActualPrice ?? 0);

            _unitOfWork.Save(); // Save all changes
        }

        // Renamed and enhanced to handle both reservation and its details
        public void UpdateBookingReservationAndDetails(BookingReservation bookingReservation, List<BookingDetail> updatedBookingDetails)
        {
            if (bookingReservation.BookingReservationID == 0)
            {
                throw new ArgumentException("Booking Reservation ID is required for update.");
            }

            // 1. Update the main BookingReservation entity
            _unitOfWork.BookingReservationRepository.Update(bookingReservation);

            // 2. Manage BookingDetails: Add, Update, Delete
            var existingDetails = _unitOfWork.BookingDetailRepository
                                            .Get(bd => bd.BookingReservationID == bookingReservation.BookingReservationID)
                                            .ToList();

            // Details to Add: Present in updated list but not in existing list
            var detailsToAdd = updatedBookingDetails.Where(ud => !existingDetails.Any(ed => ed.RoomID == ud.RoomID && ed.StartDate == ud.StartDate && ed.EndDate == ud.EndDate)).ToList();
            foreach (var detail in detailsToAdd)
            {
                detail.BookingReservationID = bookingReservation.BookingReservationID;
                _unitOfWork.BookingDetailRepository.Add(detail);
            }

            // Details to Update: Present in both lists (based on RoomID, StartDate, EndDate as primary keys for simplicity)
            foreach (var existingDetail in existingDetails)
            {
                var matchingUpdatedDetail = updatedBookingDetails.FirstOrDefault(ud => ud.RoomID == existingDetail.RoomID && ud.StartDate == existingDetail.StartDate && ud.EndDate == existingDetail.EndDate);
                if (matchingUpdatedDetail != null)
                {
                    // Update properties of existing detail from matching updated detail
                    // Only update properties that are modifiable through the UI
                    existingDetail.ActualPrice = matchingUpdatedDetail.ActualPrice;
                    existingDetail.StartDate = matchingUpdatedDetail.StartDate; // Assuming these can be updated
                    existingDetail.EndDate = matchingUpdatedDetail.EndDate;     // Assuming these can be updated
                    // No explicit _unitOfWork.BookingDetailRepository.Update(existingDetail) needed here if 'existingDetail' is already tracked
                    // and its properties are being modified directly. EF Core will detect changes upon Save().
                }
            }

            // Details to Delete: Present in existing list but not in updated list
            var detailsToDelete = existingDetails.Where(ed => !updatedBookingDetails.Any(ud => ud.RoomID == ed.RoomID && ud.StartDate == ed.StartDate && ud.EndDate == ed.EndDate)).ToList();
            foreach (var detail in detailsToDelete)
            {
                // CORRECTED LINE: Pass the BookingDetail entity directly to the Delete method.
                // This calls the 'Delete(TEntity entityToDelete)' overload in GenericRepository.
                _unitOfWork.BookingDetailRepository.Delete(detail);
            }

            // Recalculate and update the total price for the main reservation after detail changes
            // You might need to refetch updatedBookingDetails from the repository after adds/updates/deletes
            // or iterate over the `updatedBookingDetails` list if it accurately reflects the final state.
            // For accuracy, it's best to calculate from the current state of booking details in the context
            var finalBookingDetails = _unitOfWork.BookingDetailRepository
                                                    .Get(bd => bd.BookingReservationID == bookingReservation.BookingReservationID)
                                                    .ToList();
            bookingReservation.TotalPrice = finalBookingDetails.Sum(bd => bd.ActualPrice ?? 0);
            _unitOfWork.BookingReservationRepository.Update(bookingReservation); // Update again to save new TotalPrice

            _unitOfWork.Save(); // Save all accumulated changes
        }

        public void DeleteBookingReservation(int id)
        {
            // Deleting a reservation will cascade delete its details due to DB configuration and EF Core setup
            // This assumes cascade delete is configured in EF Core model or database.
            _unitOfWork.BookingReservationRepository.Delete(id);
            _unitOfWork.Save();
        }

        public IEnumerable<BookingReservation> GetBookingReservationsByCustomerId(int customerId)
        {
            return _unitOfWork.BookingReservationRepository.Get(
                br => br.CustomerID == customerId,
                orderBy: q => q.OrderByDescending(br => br.BookingDate),
                includeProperties: "BookingDetails.RoomInformation,Customer,BookingDetails.RoomInformation.RoomType"
            );
        }

        public IEnumerable<BookingReservation> GetBookingReservationsByPeriod(DateTime startDate, DateTime endDate)
        {
            return _unitOfWork.BookingReservationRepository.Get(
                br => br.BookingDate >= startDate && br.BookingDate <= endDate,
                orderBy: q => q.OrderByDescending(br => br.BookingDate), // Sort by BookingDate descending
                includeProperties: "Customer,BookingDetails.RoomInformation,BookingDetails.RoomInformation.RoomType" // Include related data for the report
            );
        }
    }
}
