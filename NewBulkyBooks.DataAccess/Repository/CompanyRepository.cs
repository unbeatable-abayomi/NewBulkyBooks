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
    public class CompanyRepository : Repository<Company>,ICompanyRepository
    {
		private readonly ApplicationDbContext _db;
		public CompanyRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(Company company)
		{
		//	var objFromDd = _db.Companies.FirstOrDefault(i => i.Id == company.Id);

			
				_db.Update(company);



		}
	}
}
