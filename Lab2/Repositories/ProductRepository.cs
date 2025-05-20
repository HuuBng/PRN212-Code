using System.Collections.Generic;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private ProductDAO pDAO;
        public ProductRepository()
        {
            pDAO = new ProductDAO();
        }
        public void DeleteProduct(Product p) => pDAO.DeleteProduct(p);
        public void SaveProduct(Product p) => pDAO.SaveProduct(p);
        public void UpdateProduct(Product p) => pDAO.UpdateProduct(p);
        public List<Product> GetProducts() => pDAO.GetProducts();
        public Product GetProductById(int id) => pDAO.GetProductById(id);
    }
}
