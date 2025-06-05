using System.ComponentModel.DataAnnotations;

namespace Core.Objects
{
    public partial class RoomType
    {
        public RoomType(string RoomTypeName, string? TypeDescription, string? TypeNote)
        {
            this.RoomTypeName = RoomTypeName;
            this.TypeDescription = TypeDescription;
            this.TypeNote = TypeNote;
        }

        public RoomType()
        {
        }

        [Required(ErrorMessage = "Room Type ID is required.")]
        public int RoomTypeID { get; set; }

        [Required(ErrorMessage = "Room type name is required.")]
        [StringLength(250, ErrorMessage = "Room type name cannot exceed 250 characters.")]
        public string RoomTypeName { get; set; }

        [StringLength(250, ErrorMessage = "Type description cannot exceed 250 characters.")]
        public string? TypeDescription { get; set; }

        [StringLength(250, ErrorMessage = "Type note cannot exceed 250 characters.")]
        public string? TypeNote { get; set; }
    }
}
