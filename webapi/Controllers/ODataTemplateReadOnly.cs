using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers {

	[AllowAnonymous]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ODataTemplateReadOnly<T> : ODataBaseController where T:class, IndexedModel {

		protected DataContext _db;

		public ODataTemplateReadOnly(IConfiguration config, IOptions<AppSettings> appIdentitySettingsAccessor, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor) : 
			base(config, appIdentitySettingsAccessor, hostingEnvironment, httpContextAccessor) {
			_db = this.GetConnection();
		}

		public override void Dispose() {
			_db.Dispose();
		}

		private DbSet<T> TableForT() {
			return _db.Set<T>();
		}

		#region CRUD

		[HttpGet]
		[EnableQuery(MaxAnyAllExpressionDepth = 3)]
		public virtual IQueryable<T> Get() {
			return TableForT();
		}

		[HttpGet]
		[EnableQuery(MaxAnyAllExpressionDepth = 3)]
		public SingleResult<T> Get([FromODataUri] int key) {
			IQueryable<T> result = Get().Where(p => p.ID == key);
			return SingleResult.Create(result);
		}

		#endregion
	}
}
