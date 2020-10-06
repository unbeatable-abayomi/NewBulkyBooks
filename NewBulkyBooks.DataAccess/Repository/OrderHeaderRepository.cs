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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _db;
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(OrderHeader orderHeader)
		{
			_db.Update(orderHeader);
			//var objFromDb = _db.Categories.FirstOrDefault(i => i.Id == category.Id);
			//if(objFromDb != null)
			//{
			//	objFromDb.Name = category.Name;
			//}
		}
	}
}
