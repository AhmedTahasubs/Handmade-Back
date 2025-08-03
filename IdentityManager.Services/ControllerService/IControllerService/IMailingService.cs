using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityManager.Services.ControllerService.IControllerService
{
	public interface IMailingService
	{
		Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null);
	}
}
