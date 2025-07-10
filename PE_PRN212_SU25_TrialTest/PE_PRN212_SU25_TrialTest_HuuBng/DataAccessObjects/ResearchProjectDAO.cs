using Microsoft.EntityFrameworkCore;
using Objects;

namespace DataAccessObjects
{
    public class ResearchProjectDAO(ResearchDbContext dbContext)
    {
        public List<ResearchProject> GetAllResearchProject()
        {
            return dbContext.ResearchProject.Include(r => r.LeadResearcher).ToList();
        }

        public ResearchProject? GetResearchProjectById(int id)
        {
            return dbContext.ResearchProject.Include(r => r.LeadResearcher)
                    .FirstOrDefault(rp => rp.ProjectID == id);
        }

        public void CreateResearchProject(ResearchProject researchProject)
        {
            researchProject.ProjectID = dbContext.ResearchProject.Any() ?
                    dbContext.ResearchProject.Max(p => p.ProjectID) + 1 : 1;

            dbContext.ResearchProject.Add(researchProject);
            dbContext.SaveChanges();
        }

        public void UpdateResearchProject(ResearchProject researchProject)
        {
            dbContext.ResearchProject.Update(researchProject);
            dbContext.SaveChanges();
        }

        public void DeleteResearchProject(int id)
        {
            var researchProject = GetResearchProjectById(id);
            if (researchProject != null)
            {
                dbContext.ResearchProject.Remove(researchProject);
                dbContext.SaveChanges();
            }
        }

        public List<ResearchProject> Search(string keyword)
        {
            return dbContext.ResearchProject
                        .Include(r => r.LeadResearcher)
                        .Where(p => p.ProjectTitle.Contains(keyword) || p.ResearchField.Contains(keyword))
                        .ToList();
        }
    }
}
