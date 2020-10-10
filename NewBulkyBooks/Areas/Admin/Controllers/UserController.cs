using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewBulkyBooks.DataAccess.Data;
using NewBulkyBooks.DataAccess.Repository.IRepository;
using NewBulkyBooks.Models;
using NewBulkyBooks.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBulkyBooks.Areas.Admin.Controllers
{

	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

	public class UserController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ApplicationDbContext _db;


		public UserController(IUnitOfWork unitOfWork, ApplicationDbContext db)
		{
			_unitOfWork = unitOfWork;
			_db = db;

		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		#region
		[HttpGet]
		public IActionResult GetAll()
		{
			//var listOfUsers = _unitOfWork.ApplicationUser.GetAll(includeProperties:"Company");
			var listOfUsers = _db.ApplicationUsers.Include(i => i.Company).ToList();
			var roles = _db.Roles.ToList(); 
			var userRole = _db.UserRoles.ToList();
			foreach (var user in listOfUsers)
			{
				var roleId = userRole.FirstOrDefault(i => i.UserId == user.Id).RoleId;
				user.Role = roles.FirstOrDefault(i => i.Id == roleId).Name;
				if (user.Company == null)
				{
					user.Company = new Company()
					{
							Name = ""
					};
				}
			}

			return Json(new {data = listOfUsers });
		}







		[HttpPost]
		public IActionResult LockUnlock([FromBody] string id)
		{
			//var objFromDb = _unitOfWork.ApplicationUser.Get(id);
			var objFromDb = _db.ApplicationUsers.FirstOrDefault(i => i.Id == id);

			if (objFromDb == null)
			{
				return Json(new { success = false, message = "Error while Locking/Unlocing" });
			}


			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
			{
				//USER is currently Locked we will unlock them
				objFromDb.LockoutEnd = DateTime.Now;
			}
			else
			{
				objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
			}
			return Json(new { success = true, message = "Operation Succesfull" });
		}
			
			
			
			
			#endregion
	}
}
