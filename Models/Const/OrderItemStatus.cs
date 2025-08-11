using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Const
{
	public static class OrderItemStatus
	{
		public static string Pending { get; set; } = "Pending";
		public static string Processing { get; set; } = "Processing";
		public static string Shipped { get; set; } = "Shipped";
		public static string Delivered { get; set; } = "Delivered";
		public static string Cancelled { get; set; } = "Cancelled";
	}
}
