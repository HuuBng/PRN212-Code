using BusinessObjects;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class ProductDAO
    {
        private static List<Product> listProducts;
       

        public List<Product> GetProducts()
        {
            var listProducts = new List<Product>();
            try
            {
                using var db = new MyStoreContext();
                listProducts = db.Products.ToList();

            }
            catch (Exception e) { 
            
            
            }
            return listProducts;
        }

        public void SaveProduct(Product p)
        {
            try
            {
                using var context = new MyStoreContext();
                context.Products.Add(p);
                context.SaveChanges();

            }
            catch (Exception e)
            { 
            throw new Exception(e.Message);
            
            }
        }

        public void UpdateProduct(Product p)
        {
            try
            {
                using var context = new MyStoreContext();
                context.Entry<Product>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();

            }
            catch (Exception e)
            {
                throw new Exception (e.Message);
            }
        }

        public void DeleteProduct(Product p)
        {
            try
            {
                using var context = new MyStoreContext();
                var p1 = context.Products.SingleOrDefault(c => c.ProductId == p.ProductId);
                context.Products.Remove(p1);
                context.SaveChanges();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public Product GetProductById(int id)
        {
            using var db = new MyStoreContext();
            return db.Products.FirstOrDefault(c => c.ProductId.Equals(id));
           

        }


    }
}

