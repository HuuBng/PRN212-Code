using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Objects
{
    [Table("Researcher")]
    public class Researcher
    {
        [Key]
        public int ResearcherID { get; set; }
        public required string FullName { get; set; }
        public required string Affiliation { get; set; }
        public required string Email { get; set; }
        public required string Expertise { get; set; }

    }
}
