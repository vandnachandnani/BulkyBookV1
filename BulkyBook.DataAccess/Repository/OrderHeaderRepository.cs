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
    public class OrderHeaderRepository: Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            db.OrderHeaders.Update(orderHeader);
        }


        public void UpdateStatus(int Id, string OrderStatus, string? PaymentStatus = null)
        {
            var orderfromDb = db.OrderHeaders.FirstOrDefault(x => x.Id == Id);
            if(orderfromDb!=null)
            {
                orderfromDb.OrderStatus = OrderStatus;  
                if(!string.IsNullOrEmpty(PaymentStatus))
                {
                    orderfromDb.PaymentStatus = PaymentStatus;  
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentItentId)
        {
            var orderFromDb = db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            orderFromDb.PaymentDate = DateTime.Now;
            orderFromDb.SessionId = sessionId;
            orderFromDb.PaymentIntentId = paymentItentId;
        }
    }
}
