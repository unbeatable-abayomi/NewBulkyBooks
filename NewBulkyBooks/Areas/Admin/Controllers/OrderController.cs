﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewBulkyBooks.DataAccess.Repository.IRepository;
using NewBulkyBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBulkyBooks.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


		public IActionResult Index()
		{
			return View();
		}

		#region API CALLS
		[HttpGet]

		public IActionResult GetOrderList()
		{
			IEnumerable<OrderHeader> orderHeadersList;

			orderHeadersList = _unitOfWork.OrderHeader.GetAll(includeProperties:"ApplicationUser");

			return Json(new { data = orderHeadersList });
		}

		#endregion
	}
}
