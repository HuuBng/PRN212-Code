using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public partial class Category
    {

        public Category()
        {
            Products = new HashSet<Product>();
        }

        public Category(int catID, String catName)
        {
            this.CategoryId = catID;
            this.CategoryName = catName;

        }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public override string ToString()
        {
            return CategoryName ?? "Unknow";

        }


    }
}
