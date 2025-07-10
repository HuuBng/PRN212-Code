using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Objects
{
    [Table("ResearchProject")]
    public class ResearchProject
    {
        public ResearchProject()
        {
        }
        [Key]
        public int ProjectID { get; set; }
        public required string ProjectTitle { get; set; }
        public required string ResearchField { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeadResearcherID { get; set; }
        public decimal Budget { get; set; }


        public Researcher? LeadResearcher { get; set; }
    }
}
