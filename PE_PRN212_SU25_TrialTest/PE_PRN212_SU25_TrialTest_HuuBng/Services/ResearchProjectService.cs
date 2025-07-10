using DataAccessObjects;
using Objects;

namespace Services
{
    public class ResearchProjectService
    {
        private readonly ResearchProjectDAO researchProjectDAO;
        private readonly ResearcherDAO researcherDAO;

        public ResearchProjectService()
        {
            researchProjectDAO = new ResearchProjectDAO(new ResearchDbContext());
            researcherDAO = new ResearcherDAO(new ResearchDbContext());
        }

        public List<Researcher> GetResearchers()
        {
            return researcherDAO.GetAllReseachers();
        }

        public Researcher? GetResearcher(int id)
        {
            return researcherDAO.GetResearcherById(id);
        }

        public List<ResearchProject> GetResearchProjects()
        {
            return researchProjectDAO.GetAllResearchProject();
        }

        public ResearchProject? GetResearchProject(int id)
        {
            return researchProjectDAO.GetResearchProjectById(id);
        }

        public void DeleteResearchProject(int id)
        {
            researchProjectDAO.DeleteResearchProject(id);
        }

        public void CreateResearchProject(ResearchProject researchProject)
        {
            researchProjectDAO.CreateResearchProject(researchProject);
        }

        public void UpdateResearchProject(ResearchProject researchProject)
        {
            researchProjectDAO.UpdateResearchProject(researchProject);
        }

        public List<ResearchProject> SearchProjects(string keyword)
        {
            return researchProjectDAO.Search(keyword);
        }
    }
}
