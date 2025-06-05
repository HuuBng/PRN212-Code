using Core.Objects;

namespace Core.Repositories
{
    public interface IRoomTypeRepository
    {
        Task<RoomType> GetRoomTypeByIdAsync(int roomTypeId);
        Task<IEnumerable<RoomType>> GetAllRoomTypesAsync();
        Task AddRoomTypeAsync(RoomType roomType);
        Task UpdateRoomTypeAsync(RoomType roomType);
        Task DeleteRoomTypeAsync(int roomTypeId);
        Task<bool> RoomTypeExistsAsync(int roomTypeId);
    }
}
