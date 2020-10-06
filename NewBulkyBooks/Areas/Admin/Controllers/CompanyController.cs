using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		public CompanyController(IUnitOfWork unitOfWork)
		{
            _unitOfWork = unitOfWork;
		}


        [HttpGet]
        public IActionResult Index()
		{
            return View();
		}



        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();

            if(id == null)
			{
                return View(company);
			}

            company = _unitOfWork.Company.Get(id.GetValueOrDefault());

            if(company == null)
			{
                return NotFound();
			}
            return View(company);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
			if (ModelState.IsValid)
			{
				if (company.Id == 0)
				{
                    _unitOfWork.Company.Add(company);
				}
				else
				{
                    _unitOfWork.Company.Update(company);
				}
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
			}
            return View(company);

        }



		#region APICALLS

        [HttpGet]
        public IActionResult GetAll()
		{
            var objFromDb = _unitOfWork.Company.GetAll();
            return Json(new { data = objFromDb });
		}




        [HttpDelete]

        public IActionResult Delete(int id)
		{
            var objFromDb = _unitOfWork.Company.Get(id);
            if(objFromDb == null)
			{
                return Json(new { success = false, message = "Error While Deleye" });
			}

            _unitOfWork.Company.Remove(objFromDb);
            _unitOfWork.Save();
		  return Json(new { success = true, message = "Successfully Deleted " });
      
            
		}
		#endregion
	}
}
