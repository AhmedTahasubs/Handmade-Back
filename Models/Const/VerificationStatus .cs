using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Const
{
	public static class VerificationStatus
	{
		public const string Unverified = "Unverified";
		public const string Verified = "Verified";
		public const string Rejected = "Rejected";
		public const string Pending = "Pending";

		public static readonly string[] AllStatuses =
		{
			Unverified, Verified, Rejected, Pending
		};

		public static bool TryParse(string input, out string matchedStatus)
		{
			matchedStatus = AllStatuses
				.FirstOrDefault(s =>
					string.Equals(s, input, StringComparison.OrdinalIgnoreCase));

			return matchedStatus != null;
		}
	}
}
