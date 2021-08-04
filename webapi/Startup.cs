using Entities;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {

			// *************** CONFIGURAZIONE APPSSETTINGS.JSON/CORS POLICY ***************
			var config = new AppSettings();
			Configuration.Bind("AppSettings", config);      //  <--- This
			services.AddSingleton(config);

			services
			.AddCors(options => {
				options.AddPolicy("CorsPolicy",
					builder => builder
					.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					);
			});

			var identitySettingsSection = Configuration.GetSection("AppSettings");
			services.Configure<AppSettings>(identitySettingsSection);

			services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
			// *************** CONFIGURAZIONE APPSSETTINGS.JSON/CORS POLICY ***************

			services.AddControllers(options => {
				//{
				//    options.Conventions.Add(new MetadataApplicationModelConventionAttribute());
				//    options.Conventions.Add(new MetadataActionModelConvention());
			});

			services.AddControllers().AddNewtonsoftJson(x => {
				x.SerializerSettings.ContractResolver = new DefaultContractResolver();
				x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			});

			services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(20)
				.AddModel("odata", this.GetEdmModel())
				//.ConfigureRoute(route => route.EnableQualifiedOperationCall = false) // use this to configure the built route template
			);

			services.AddSwaggerGen(c =>
			{
				c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
			}

			// *************** CONFIGURAZIONE ODATA/SWAGGER ***************
			// If you want to use middle, enable the middleware.
			//app.UseODataOpenApi();

			// Add the OData Batch middleware to support OData $Batch
			app.UseODataQueryRequest();
			app.UseODataBatching();

			app.UseSwagger();
			app.UseSwaggerUI(c => {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "OData 8.x OpenAPI");
			});
			// *************** CONFIGURAZIONE ODATA/SWAGGER ***************

			//app.UseHttpsRedirection();

			app.UseRouting();

			// Test middelware
			app.Use(next => context => {
				var endpoint = context.GetEndpoint();
				if (endpoint == null) {
					return next(context);
				}

				return next(context);
			});

			// *************** CONFIGURAZIONE INTERNAZIONALIZZAZIONE ***************
			app.UseCors("CorsPolicy");

			var defaultDateCulture = "it-IT";
			var ci = new CultureInfo(defaultDateCulture);
			ci.NumberFormat.NumberDecimalSeparator = ".";
			ci.NumberFormat.CurrencyDecimalSeparator = ".";

			// Configure the Localization middleware
			app.UseRequestLocalization(new RequestLocalizationOptions {
				DefaultRequestCulture = new RequestCulture(ci),
				SupportedCultures = new List<CultureInfo>
				{
					ci,
				},
				SupportedUICultures = new List<CultureInfo>
				{
					ci,
				}
			});
			// *************** CONFIGURAZIONE INTERNAZIONALIZZAZIONE ***************

			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
				endpoints.MapControllers();
			});
		}

		private IEdmModel GetEdmModel() {
			var builder = new ODataConventionModelBuilder();

			return builder.GetEdmModel();
		}
	}
}
