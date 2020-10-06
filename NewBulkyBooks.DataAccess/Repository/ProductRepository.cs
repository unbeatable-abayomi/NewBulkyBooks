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
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private readonly ApplicationDbContext _db;
		public ProductRepository(ApplicationDbContext db): base(db)
		{
			_db = db;
		}
		public void Update(Product product)
		{
			var objFromDb = _db.Products.FirstOrDefault(i => i.Id == product.Id);

			if (objFromDb != null)
			{
				if (objFromDb.ImageUrl != null)
				{
					objFromDb.ImageUrl = product.ImageUrl;
				};
				objFromDb.ISBN = product.ISBN;
				objFromDb.Price = product.Price;
				objFromDb.Price100 = product.Price;
				objFromDb.Price50 = product.Price50;
				objFromDb.ListPrice = product.ListPrice;
				objFromDb.Title = product.Title;
				objFromDb.Description = product.Description;
				objFromDb.CategoryId = product.CategoryId;
				objFromDb.Author = product.Author;
				objFromDb.CoverTypeId = product.CoverTypeId;


			};
		}
	}
}
