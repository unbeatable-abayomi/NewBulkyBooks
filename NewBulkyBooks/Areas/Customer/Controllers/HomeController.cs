using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewBulkyBooks.DataAccess.Repository.IRepository;
using NewBulkyBooks.Models;
using NewBulkyBooks.Models.ViewModels;

namespace NewBulkyBooks.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;
		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public IActionResult Index()
		{
			IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"CoverType,Category");
			return View(productList);
		}
		[HttpGet]
		public IActionResult Details(int id)
		{
			var productFroMDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "CoverType,Category");
			ShoppingCart cartObj = new ShoppingCart()
			{
				Product = productFroMDb,
				ProductId = productFroMDb.Id
			};
			return View(cartObj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public IActionResult Details(ShoppingCart CartObject)
		{
			CartObject.Id = 0;
			if (ModelState.IsValid)
			{
				//then we add to the cart
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
				CartObject.ApplicationUserId = claim.Value;
				ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
	u => u.ApplicationUserId == CartObject.ApplicationUserId && u.ProductId == CartObject.ProductId, includeProperties: "Product"
	);
				if(cartFromDb == null)
				{
					//no record exists in database for that product for that user
					_unitOfWork.ShoppingCart.Add(CartObject);
				}
				else
				{
					cartFromDb.Count += CartObject.Count;
					_unitOfWork.ShoppingCart.Update(cartFromDb);

				}
				_unitOfWork.Save();
				return RedirectToAction(nameof(Index));
			}
			else
			{
				var productFroMDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == CartObject.ProductId, includeProperties: "CoverType,Category");
				ShoppingCart cartObj = new ShoppingCart()
				{
					Product = productFroMDb,
					ProductId = productFroMDb.Id
				};
				return View(cartObj);
			}

		}
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
