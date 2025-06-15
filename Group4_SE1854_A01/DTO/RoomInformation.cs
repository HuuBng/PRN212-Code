using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
    public class RoomInformation
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } // max length 50
        public string RoomDescription { get; set; } // max length 220
        public int RoomMaxCapacity { get; set; }
        public int RoomStatus { get; set; } // 1 Active, 2 Deleted
        public decimal RoomPricePerDate { get; set; }
        public int RoomTypeID { get; set; }

        public RoomType RoomType { get; set; } // navigation property for binding UI
    }
}
