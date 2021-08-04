using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Formatter.Value;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Attributes;
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
	public class ODataTemplate<T> : ODataBaseController where T:class, IndexedModel {

		protected DataContext _db;

		public ODataTemplate(IConfiguration config, IOptions<AppSettings> appIdentitySettingsAccessor, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor) : 
			base(config, appIdentitySettingsAccessor, hostingEnvironment, httpContextAccessor) {
			_db = this.GetConnection();
		}

		public override void Dispose() {
			_db.Dispose();
		}

		private bool Exists(long key) {
			return TableForT().Any(p => p.ID == key);
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

		[HttpPost]
		[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
		public virtual async Task<IActionResult> Post([FromBody]T obj) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			TableForT().Add(obj);
			await _db.SaveChangesAsync();
			return Created(obj);
		}

		[HttpPatch]
		[EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
		public virtual async Task<IActionResult> Patch([FromODataUri] int key, [FromBody]Delta<T> delta) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			var entity = await TableForT().FindAsync(key);
			if (entity == null) {
				return NotFound();
			}

			delta.Patch(entity);

			try {
				await _db.SaveChangesAsync();
			} catch (DbUpdateConcurrencyException) {
				if (!Exists(key)) {
					return NotFound();
				} else {
					throw ;
				}
			}

			return Updated(entity);
		}

		[HttpDelete]
		public virtual async Task<IActionResult> Delete([FromODataUri] int key) {
			var entity = await TableForT().FindAsync(key);
			if (entity == null) {
				return NotFound();
			}

			TableForT().Remove(entity);
			await _db.SaveChangesAsync();
			return StatusCode((int)HttpStatusCode.NoContent);
		}

		#endregion
	}
}
