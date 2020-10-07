using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NewBulkyBooks.DataAccess.Repository.IRepository;
using NewBulkyBooks.Models.ViewModels;
using NewBulkyBooks.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewBulkyBooks.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailSender _emailSender;
		private readonly UserManager<IdentityUser> _userManager;

		public ShoppingCartVM ShoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork,IEmailSender emailSender, UserManager<IdentityUser> userManger)
		{
			_unitOfWork = unitOfWork;
			_emailSender = emailSender;
			_userManager = userManger;
		}


		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;

			var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			ShoppingCartVM = new ShoppingCartVM()
			{
				OrderHeader = new Models.OrderHeader(),
				ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claims.Value, includeProperties:"Product")

			};

			ShoppingCartVM.OrderHeader.OrderTotal = 0;
			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claims.Value, includeProperties: "Company");

			foreach(var list in ShoppingCartVM.ListCart)
			{
				//list.Product = _unitOfWork.Product.GetFirstOrDefault();
				list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
				ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
				list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);
				if(list.Product.Description.Length > 100)
				{
					list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
				}
			}
				return View(ShoppingCartVM);
		}
	}
}
