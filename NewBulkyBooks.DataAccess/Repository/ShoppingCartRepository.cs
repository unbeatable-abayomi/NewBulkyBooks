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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private readonly ApplicationDbContext _db;
		public ShoppingCartRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(ShoppingCart shoppingCart)
		{
			_db.Update(shoppingCart);
			//var objFromDb = _db.Categories.FirstOrDefault(i => i.Id == category.Id);
			//if(objFromDb != null)
			//{
			//	objFromDb.Name = category.Name;
			//}
		}
	}
}
