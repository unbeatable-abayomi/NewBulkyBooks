﻿using Microsoft.AspNetCore.Authorization;
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
	[Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		public CoverTypeController(IUnitOfWork unitOfWork)
		{
            _unitOfWork = unitOfWork; 
		}

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
		{
            CoverType coverType = new CoverType();

			if (id == null)
			{
                return View(coverType);
			}
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
			if (coverType == null)
			{
                return NotFound();
			}
            return View(coverType);
		}


        [HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
		{
			if (ModelState.IsValid)
			{
				if (coverType.Id == 0)
				{
                    _unitOfWork.CoverType.Add(coverType);
				}
				else
				{
					_unitOfWork.CoverType.Update(coverType);
				}
				_unitOfWork.Save();
				return RedirectToAction(nameof(Index));
			}

            return View(coverType);
		}

		#region APICALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			var allObj = _unitOfWork.CoverType.GetAll();
			return Json(new { data = allObj });
		}

		[HttpDelete]

		public IActionResult Delete(int id)
		{
			var objFromDb = _unitOfWork.CoverType.Get(id);
			if(objFromDb == null)
			{
				return Json(new { success = false, message = "Error While Deleting" });
			}
			_unitOfWork.CoverType.Remove(objFromDb);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Successfull Deleted" });
		}
		#endregion
	}
}
