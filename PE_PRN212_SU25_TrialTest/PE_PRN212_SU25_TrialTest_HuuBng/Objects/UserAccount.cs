using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Objects
{
    [Table("UserAccount")]
    public class UserAccount
    {
        [Key]
        public int UserID { get; set; }

        public required string Password { get; set; }

        public required string Email { get; set; }

        public int Role { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
