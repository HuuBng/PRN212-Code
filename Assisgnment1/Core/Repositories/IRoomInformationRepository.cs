using Core.Objects;

namespace Core.Repositories
{
    public interface IRoomInformationRepository
    {
        Task<RoomInformation> GetRoomInformationByIdAsync(int roomId);
        Task<IEnumerable<RoomInformation>> GetAllRoomInformationsAsync();
        Task AddRoomInformationAsync(RoomInformation roomInformation);
        Task UpdateRoomInformationAsync(RoomInformation roomInformation);
        Task DeleteRoomInformationAsync(int roomId);
        Task<bool> RoomInformationExistsAsync(int roomId);
    }
}
