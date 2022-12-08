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
    public class OrderDetailRepository:Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext db;

        public OrderDetailRepository(ApplicationDbContext db):base(db)
        {
            this.db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            db.OrderDetails.Update(orderDetail);
        }
    }
}
