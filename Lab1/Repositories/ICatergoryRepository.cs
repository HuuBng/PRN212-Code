using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface ICatergoryRepository
    {
        List<Category> GetCategories();
    }
}
