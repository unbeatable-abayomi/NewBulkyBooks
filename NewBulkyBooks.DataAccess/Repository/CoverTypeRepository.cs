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
	public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
	{
		private readonly ApplicationDbContext _db;
		public CoverTypeRepository(ApplicationDbContext db): base(db)
		{
			_db = db;
		}
		public void Update(CoverType coverType)
		{
			var objFromDb = _db.CoverTypes.FirstOrDefault(i => i.Id == coverType.Id);
			if (objFromDb != null)
			{
				objFromDb.Name = coverType.Name;
			}
		}
	}
}
