using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _model;

        public ProductRepository(ApplicationDbContext model ):base(model)
        {
            _model = model;
        }
        public void Update(Product model)
        {
            var objFromDb = _model.Products.FirstOrDefault(x => x.Id == model.Id);
            if(objFromDb!=null)
            {
                objFromDb.Title = model.Title;
                objFromDb.ISBN = model.ISBN;
                objFromDb.Price = model.Price;
                objFromDb.Price50 = model.Price50;
                objFromDb.Price100 = model.Price100;
                objFromDb.ListPrice = model.ListPrice;
                objFromDb.Description = model.Description;
                objFromDb.CategoryId = model.CategoryId;
                objFromDb.CoverTypeId = model.CoverTypeId;
                objFromDb.Author = model.Author;
                if(model.ImageUrl!=null)
                {
                    objFromDb.ImageUrl = model.ImageUrl;
                }
            }


            _model.Update(objFromDb);
        }
    }
}
