﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewBulkyBooks.DataAccess.Repository.IRepository;
using NewBulkyBooks.Models;
using NewBulkyBooks.Models.ViewModels;
using NewBulkyBooks.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NewBulkyBooks.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderDetailsVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


		public IActionResult Index()
		{
			return View();
		}


		
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult StartProcessing(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
			orderHeader.OrderStatus = SD.StatusInProcess;
			_unitOfWork.Save();
			return RedirectToAction("Index");
		}

		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult ShipOrder()
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusShipped;

			orderHeader.ShippingDate = DateTime.Now;
			_unitOfWork.Save();
			return RedirectToAction("Index");
		}

		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult CancelOrder(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
			if (orderHeader.PaymentStatus == SD.StatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
					Reason = RefundReasons.RequestedByCustomer,
					Charge = orderHeader.TransactionID
				};
				var service = new RefundService();
				Refund refund = service.Create(options);
				orderHeader.OrderStatus = SD.StatusRefunded;
				orderHeader.PaymentStatus = SD.StatusRefunded;
			}
			else
			{
				orderHeader.OrderStatus = SD.StatusCancelled;
				orderHeader.PaymentStatus = SD.StatusRefunded;
			}
			orderHeader.OrderStatus = SD.StatusInProcess;
			_unitOfWork.Save();
			return RedirectToAction("Index");
		}
		public IActionResult Details(int id)
		{
			OrderVM = new OrderDetailsVM()
			{
				OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
				OrderDetails = _unitOfWork.OrderDetails.GetAll(O => O.OrderId == id, includeProperties: "Product")
			};
			return View(OrderVM);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Details")]
		public IActionResult Details(string stripeToken)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
			if (stripeToken != null)
			{
			
				//process the payment
				var options = new ChargeCreateOptions
				{
					Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
					Currency = "usd",
					Description = "OrderID" + orderHeader.Id,
					Source = stripeToken
				};
				var service = new ChargeService();
				Charge charge = service.Create(options);

				if (charge.BalanceTransactionId == null)
				{
					orderHeader.PaymentStatus = SD.PaymentStatusRejected;
				}
				else
				{
					orderHeader.PaymentStatus = charge.BalanceTransactionId;
				}
				if (charge.Status.ToLower() == "succeeded")
				{
					orderHeader.PaymentStatus = SD.PaymentStatusApproved;
					orderHeader.PaymentDate = DateTime.Now;
				}
				_unitOfWork.Save();
				
			}
			return RedirectToAction("Details", "Order", new { id = orderHeader.Id });
			//else
			//{

			//}
			//return View();
		}

		#region API CALLS
		[HttpGet]

		public IActionResult GetOrderList(string status)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;

			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			IEnumerable<OrderHeader> orderHeadersList;
			if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
				orderHeadersList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
			}
			else
			{
				orderHeadersList = _unitOfWork.OrderHeader.GetAll(
					u => u.ApplicationUserId ==  claim.Value,
					includeProperties: "ApplicationUser");
			}

			switch (status)
			{
				case "pending":
					orderHeadersList = orderHeadersList.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
					break;
				case "inprocess":
					orderHeadersList = orderHeadersList.Where(o => o.OrderStatus == SD.StatusApproved || o.OrderStatus == SD.StatusInProcess ||o.OrderStatus == SD.StatusPending);
					break;
				case "completed":
					orderHeadersList = orderHeadersList.Where(o => o.OrderStatus == SD.StatusShipped);
					break;
				case "rejected":
					orderHeadersList = orderHeadersList.Where(o => o.OrderStatus == SD.StatusCancelled || o.OrderStatus == SD.StatusRefunded || o.OrderStatus == SD.PaymentStatusRejected);
					break;
				default:
					//all = "active text-white";
					break;



			}

			return Json(new { data = orderHeadersList });
		}

		#endregion
	}
}
