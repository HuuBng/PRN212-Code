using BusinessObjects;
using DataAccessObjects.Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions; // Added for regex validation

namespace BusinessLogicLayer
{
    // Service for managing RoomInformation and RoomType related business logic
    public class RoomService
    {
        private UnitOfWork _unitOfWork;

        public RoomService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<RoomInformation> GetAllRooms()
        {
            // Include RoomType for displaying type name in UI
            return _unitOfWork.RoomInformationRepository.Get(includeProperties: "RoomType");
        }

        public RoomInformation GetRoomById(int id)
        {
            return _unitOfWork.RoomInformationRepository.GetById(id);
        }

        public void AddRoom(RoomInformation room)
        {
            if (string.IsNullOrEmpty(room.RoomNumber))
            {
                throw new ArgumentException("Room Number is required.");
            }
            // Check if RoomNumber contains letters
            if (Regex.IsMatch(room.RoomNumber, "[a-zA-Z]"))
            {
                throw new ArgumentException("Room Number cannot contain letters. Please use numbers only.");
            }
            // Check if RoomDetailDescription is empty or null
            if (string.IsNullOrWhiteSpace(room.RoomDetailDescription))
            {
                throw new ArgumentException("Room Detail Description is required.");
            }
            if (_unitOfWork.RoomInformationRepository.Get(r => r.RoomNumber == room.RoomNumber).Any())
            {
                throw new ArgumentException("Room with this number already exists.");
            }
            room.RoomStatus = 1; // Default status for new rooms
            _unitOfWork.RoomInformationRepository.Add(room);
            _unitOfWork.Save();
        }

        public void UpdateRoom(RoomInformation room)
        {
            if (string.IsNullOrEmpty(room.RoomNumber))
            {
                throw new ArgumentException("Room Number cannot be empty.");
            }
            // Check if RoomNumber contains letters
            if (Regex.IsMatch(room.RoomNumber, "[a-zA-Z]"))
            {
                throw new ArgumentException("Room Number cannot contain letters. Please use numbers only.");
            }
            // Check if RoomDetailDescription is empty or null
            if (string.IsNullOrWhiteSpace(room.RoomDetailDescription))
            {
                throw new ArgumentException("Room Detail Description is required.");
            }
            // Check for RoomNumber uniqueness if room number is changed
            var existingRoomWithNumber = _unitOfWork.RoomInformationRepository
                                                    .Get(r => r.RoomNumber == room.RoomNumber && r.RoomID != room.RoomID)
                                                    .FirstOrDefault();
            if (existingRoomWithNumber != null)
            {
                throw new ArgumentException("Another room already has this room number.");
            }

            // MODIFIED: Fetch the existing room from the database that is being tracked by the current DbContext
            var existingRoom = _unitOfWork.RoomInformationRepository.GetById(room.RoomID);
            if (existingRoom == null)
            {
                throw new ArgumentException($"Room with ID {room.RoomID} not found for update.");
            }

            // Update the properties of the *tracked* existingRoom entity with the values from the 'room' object (from the dialog)
            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.RoomDetailDescription = room.RoomDetailDescription;
            existingRoom.RoomMaxCapacity = room.RoomMaxCapacity;
            existingRoom.RoomTypeID = room.RoomTypeID;
            existingRoom.RoomStatus = room.RoomStatus;
            existingRoom.RoomPricePerDay = room.RoomPricePerDay;

            // The GenericRepository.Update method should ideally not be needed here if 'existingRoom' is already tracked
            // and its properties are being modified directly. EF Core will detect changes upon Save().
            // However, if your GenericRepository.Update(entity) attaches it or does other setup, keeping it is harmless.
            // For most standard GenericRepository implementations, directly modifying the tracked entity is enough.
            // _unitOfWork.RoomInformationRepository.Update(existingRoom); // This line is often redundant if the entity is already tracked

            _unitOfWork.Save(); // Save changes
        }

        public void DeleteRoom(int id)
        {
            // The assignment states: "The delete action will delete room information in the case this information is not belong to any renting transaction. If the room information is already stored in a booking detail, just change the status."
            var roomToDelete = _unitOfWork.RoomInformationRepository.GetById(id);
            if (roomToDelete == null) return;

            if (_unitOfWork.BookingDetailRepository.Get(bd => bd.RoomID == id).Any())
            {
                // Room is part of a booking detail, change status instead of deleting
                roomToDelete.RoomStatus = 0; // Assuming 0 means inactive/unavailable
                _unitOfWork.RoomInformationRepository.Update(roomToDelete);
            }
            else
            {
                // Room is not part of any booking, safe to delete
                _unitOfWork.RoomInformationRepository.Delete(id);
            }
            _unitOfWork.Save();
        }

        public IEnumerable<RoomInformation> SearchRooms(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllRooms();
            }
            searchTerm = searchTerm.ToLower();
            return _unitOfWork.RoomInformationRepository.Get(r =>
                (r.RoomNumber != null && r.RoomNumber.ToLower().Contains(searchTerm)) ||
                // Use null-coalescing operator (??) to treat null descriptions as empty strings for search
                ((r.RoomDetailDescription ?? "").ToLower().Contains(searchTerm)),
                includeProperties: "RoomType" // Include room type for search display
            );
        }

        // Room Type Management
        public IEnumerable<RoomType> GetAllRoomTypes()
        {
            return _unitOfWork.RoomTypeRepository.GetAll();
        }

        public RoomType GetRoomTypeById(int id)
        {
            return _unitOfWork.RoomTypeRepository.GetById(id);
        }

        public void AddRoomType(RoomType roomType)
        {
            if (string.IsNullOrEmpty(roomType.RoomTypeName))
            {
                throw new ArgumentException("Room Type Name is required.");
            }
            if (_unitOfWork.RoomTypeRepository.Get(rt => rt.RoomTypeName == roomType.RoomTypeName).Any())
            {
                throw new ArgumentException("Room Type with this name already exists.");
            }
            _unitOfWork.RoomTypeRepository.Add(roomType);
            _unitOfWork.Save();
        }

        public void UpdateRoomType(RoomType roomType)
        {
            if (string.IsNullOrEmpty(roomType.RoomTypeName))
            {
                throw new ArgumentException("Room Type Name cannot be empty.");
            }
            var existingRoomTypeWithName = _unitOfWork.RoomTypeRepository
                                                       .Get(rt => rt.RoomTypeName == roomType.RoomTypeName && rt.RoomTypeID != roomType.RoomTypeID)
                                                       .FirstOrDefault();
            if (existingRoomTypeWithName != null)
            {
                throw new ArgumentException("Another room type already has this name.");
            }

            // Fix: Retrieve the tracked entity and update its properties
            var trackedRoomType = _unitOfWork.RoomTypeRepository.GetById(roomType.RoomTypeID);
            if (trackedRoomType == null)
            {
                throw new ArgumentException($"Room Type with ID {roomType.RoomTypeID} not found.");
            }
            trackedRoomType.RoomTypeName = roomType.RoomTypeName;
            trackedRoomType.TypeDescription = roomType.TypeDescription;
            trackedRoomType.TypeNote = roomType.TypeNote;

            _unitOfWork.Save();
        }

        public void DeleteRoomType(int id)
        {
            // Check if any rooms are associated with this room type
            if (_unitOfWork.RoomInformationRepository.Get(ri => ri.RoomTypeID == id).Any())
            {
                throw new InvalidOperationException("Cannot delete room type because rooms are associated with it. Please reassign rooms first.");
            }
            _unitOfWork.RoomTypeRepository.Delete(id);
            _unitOfWork.Save();
        }
    }
}
