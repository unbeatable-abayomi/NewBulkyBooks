
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBulkyBooks.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; }
		public string StreetAddress { get; set; }
		public string City { get; set; }
		public string State { get; set; }

		[Display(Name = "Comapany name")]
		public int? CompanyId { get; set; }


		[ForeignKey("CompanyId")]
		public Company Company { get; set; }
		public string PostalCode { get; set; }

		[NotMapped]  //means it's not added/pushed to the database
		public string Role { get; set; }

	}
}
