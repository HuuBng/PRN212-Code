
using Data.DTO;

public class RoomRepository
{
    private static RoomRepository _instance;
    private static readonly object _lock = new object();

    private List<RoomType> _roomTypes = new List<RoomType>();
    private List<RoomInformation> _rooms = new List<RoomInformation>();

    public RoomRepository()
    {
        // Khởi tạo dữ liệu mẫu
        _roomTypes.Add(new RoomType { RoomTypeID = 1, RoomTypeName = "Single", TypeDescription = "Single room", TypeNote = "Basic" });
        _roomTypes.Add(new RoomType { RoomTypeID = 2, RoomTypeName = "Double", TypeDescription = "Double room", TypeNote = "Standard" });
        _roomTypes.Add(new RoomType { RoomTypeID = 3, RoomTypeName = "Suite", TypeDescription = "Suite room", TypeNote = "Luxury" });

        _rooms.Add(new RoomInformation
        {
            RoomID = 1,
            RoomNumber = "101",
            RoomDescription = "Near elevator",
            RoomMaxCapacity = 2,
            RoomPricePerDate = 100m,
            RoomStatus = 1,
            RoomTypeID = 2,
            RoomType = _roomTypes.First(rt => rt.RoomTypeID == 2)
        });

        _rooms.Add(new RoomInformation
        {
            RoomID = 2,
            RoomNumber = "102",
            RoomDescription = "Beach View",
            RoomMaxCapacity = 2,
            RoomPricePerDate = 150m,
            RoomStatus = 1,
            RoomTypeID = 3,
            RoomType = _roomTypes.First(rt => rt.RoomTypeID == 2)
        });
    }

    public static RoomRepository Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new RoomRepository();
                }
            }
            return _instance;
        }
    }

    // Các phương thức như GetRooms, AddRoom, UpdateRoom, DeleteRoom ...
    public List<RoomType> GetRoomTypes() => _roomTypes;

    public List<RoomInformation> GetRooms() => _rooms.Where(r => r.RoomStatus == 1).ToList();

    public void AddRoom(RoomInformation room)
    {
        room.RoomID = _rooms.Any() ? _rooms.Max(r => r.RoomID) + 1 : 1;
        room.RoomStatus = 1;
        room.RoomType = _roomTypes.FirstOrDefault(rt => rt.RoomTypeID == room.RoomTypeID);
        _rooms.Add(room);
    }

    public void UpdateRoom(RoomInformation room)
    {
        var existing = _rooms.FirstOrDefault(r => r.RoomID == room.RoomID);
        if (existing != null)
        {
            existing.RoomNumber = room.RoomNumber;
            existing.RoomDescription = room.RoomDescription;
            existing.RoomMaxCapacity = room.RoomMaxCapacity;
            existing.RoomPricePerDate = room.RoomPricePerDate;
            existing.RoomTypeID = room.RoomTypeID;
            existing.RoomType = _roomTypes.FirstOrDefault(rt => rt.RoomTypeID == room.RoomTypeID);
            existing.RoomStatus = room.RoomStatus;
        }
    }

    public void DeleteRoom(int roomId)
    {
        var room = _rooms.FirstOrDefault(r => r.RoomID == roomId);
        if (room != null)
            room.RoomStatus = 2; // đánh dấu đã xóa
    }
    public List<RoomInformation> GetAllRooms()
    {
        return _rooms;
    }

}
