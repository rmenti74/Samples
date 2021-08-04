using Microsoft.Extensions.Configuration;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace API.Controllers {

	[ApiController]
	[Route("api/[controller]/[action]")]
	public class APIBaseController : Controller {

		protected readonly IConfiguration _Configuration;
		protected readonly AppSettings _AppSettings;
		protected readonly IWebHostEnvironment _HostingEnvironment;
		protected readonly IHttpContextAccessor _httpContextAccessor;
		protected readonly ILogger<APIBaseController> _logger;

		public APIBaseController(IConfiguration config) {
			_Configuration = config;
		}

		public APIBaseController(IConfiguration config, IOptions<AppSettings> appIdentitySettingsAccessor, IWebHostEnvironment hostingEnvironment
			, IHttpContextAccessor httpContextAccessor, ILogger<APIBaseController> logger) {
			_Configuration = config;
			_AppSettings = appIdentitySettingsAccessor != null ? appIdentitySettingsAccessor.Value : null;
			_HostingEnvironment = hostingEnvironment;
			_httpContextAccessor = httpContextAccessor;
			_logger = logger;
		}

		[ApiExplorerSettings(IgnoreApi = true)]

		public DataContext GetConnection() {
			DataContext oDataContext = new DataContext(this._Configuration.GetConnectionString("DefaultConnection"));
			return oDataContext;
		}

		/// <summary>
		/// Legge il token di autenticazione (Authorization) dall'header della richiesta.
		/// </summary>
		/// <returns>Token</returns>
		protected string GetToken() {
			try {
				KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> oToken = this._httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(p => p.Key == "Authorization");
				if (oToken.Key != null) {
					string szToken = oToken.Value.FirstOrDefault(p => p.ToLower().StartsWith("bearer "));
					if (!string.IsNullOrEmpty(szToken)) {
						szToken = szToken.Substring(szToken.IndexOf(" ") + 1);
						return szToken;
					}
				}
			} catch {

			}
			return null;
		}

	}
}
