using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
	public class Payment
	{
		[Key]
		public int Id { get; set; }

		[Column(TypeName = "decimal(18,2)")]
		public decimal Amount { get; set; }

		public DateTime PaymentDate { get; set; } = DateTime.Now;

		public string PaymentMethod { get; set; } = null!;

		public string TransactionId { get; set; } = null!;

		public string Status { get; set; } = "Pending";

		[ForeignKey("User")]
		public string UserId { get; set; } = null!;
		public ApplicationUser? User { get; set; }

		[ForeignKey("Order")]
		public int OrderId { get; set; }
		public CustomerOrder? Order { get; set; }
	}
}
