using Objects;

namespace DataAccessObjects
{
    public class ResearcherDAO(ResearchDbContext dbContext)
    {
        public List<Researcher> GetAllReseachers()
        {
            return dbContext.Researcher.ToList();
        }

        public Researcher? GetResearcherById(int id)
        {
            return dbContext.Researcher.FirstOrDefault(r => r.ResearcherID == id);
        }
    }
}
