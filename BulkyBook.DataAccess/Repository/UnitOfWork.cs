using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
            Category = new CategoryRepository(_db);
            CoverType = new CoverTypeRepository(_db);
            Product = new ProductRepository(_db);
            Company=new CompanyRepository(_db);
            ShoppingCart= new ShoppingCartRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
        }
        public CategoryRepository Category { get; set; }
        public CoverTypeRepository CoverType { get; set; }
        public ProductRepository Product { get; set; }
        public CompanyRepository Company { get; set; }
        public ShoppingCartRepository ShoppingCart { get; set; }
        public ApplicationUserRepository ApplicationUser { get; set; }
        public OrderDetailRepository OrderDetail { get; set; }
        public OrderHeaderRepository OrderHeader { get; set; }

        public void Save()
        {
            _db.SaveChanges();
        } 
    }
}
