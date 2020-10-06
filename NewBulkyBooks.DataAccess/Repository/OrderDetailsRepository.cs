using NewBulkyBooks.DataAccess.Data;
using NewBulkyBooks.DataAccess.Repository.IRepository;
using NewBulkyBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBulkyBooks.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
	{
		private readonly ApplicationDbContext _db;
		public OrderDetailsRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(OrderDetails orderDetails)
		{
			_db.Update(orderDetails);
				//var objFromDb = _db.Categories.FirstOrDefault(i => i.Id == category.Id);
			//if(objFromDb != null)
			//{
			//	objFromDb.Name = category.Name;
			//}
		}
	}
}
