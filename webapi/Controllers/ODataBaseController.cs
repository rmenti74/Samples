using Entities;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers {

	public class ODataBaseController : ODataController, IDisposable {

		protected readonly IConfiguration _Configuration;
		protected readonly AppSettings _AppSettings;
		protected readonly IWebHostEnvironment _HostingEnvironment;
		protected readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="appIdentitySettingsAccessor"></param>
		/// <param name="hostingEnvironment"></param>
		/// <param name="httpContextAccessor"></param>
		public ODataBaseController(IConfiguration config, IOptions<AppSettings> appIdentitySettingsAccessor, IWebHostEnvironment hostingEnvironment
			, IHttpContextAccessor httpContextAccessor) {
			_Configuration = config;
			_AppSettings = appIdentitySettingsAccessor.Value;
			_HostingEnvironment = hostingEnvironment;
			_httpContextAccessor = httpContextAccessor;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public DataContext GetConnection() {
			DataContext oDataContext = new DataContext(this._Configuration.GetConnectionString("DefaultConnection"));
			return oDataContext;
		}

		public virtual void Dispose() {
		}
		
	}
}