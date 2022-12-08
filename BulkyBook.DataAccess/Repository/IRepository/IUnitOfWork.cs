using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
       public CategoryRepository Category { get; set; }
        public CoverTypeRepository CoverType { get; set; }
        public ProductRepository Product { get; set; }
        public CompanyRepository Company { get; set; }
        public ShoppingCartRepository ShoppingCart { get; set; }
        public ApplicationUserRepository ApplicationUser { get; set; }
        public OrderDetailRepository OrderDetail { get; set; }
        public OrderHeaderRepository OrderHeader { get; set; }
        public void Save();
    }
}
 