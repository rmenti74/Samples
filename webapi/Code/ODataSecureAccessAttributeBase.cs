using API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace API.Code {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ODataSecureAccessAttributeBase : AuthorizeAttribute, IAuthorizationFilter {

        public static class AppSettingsConfig {
            public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
               .Build();
        }

        private IConfiguration _config = null;

        public ODataSecureAccessAttributeBase() {
            this._config = AppSettingsConfig.Configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            if (!this.IsAuthorize(context.HttpContext.Request)) {
                context.Result = new ForbidResult();
                return;
            }
        }

        public bool IsAuthorize(HttpRequest _oHttpRequestMessage) {
            string szToken = null;
            KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> oToken = _oHttpRequestMessage.Headers.FirstOrDefault(p => p.Key == "Authorization");
            if (oToken.Key != null) {
                szToken = oToken.Value.FirstOrDefault(p => p.ToLower().StartsWith("bearer "));
                if (!string.IsNullOrEmpty(szToken)) {
                    szToken = szToken.Substring(szToken.IndexOf(" ") + 1);
                    PublicController oPublicController = new PublicController(this._config, null, null, null, null);
                    var task = oPublicController.IsValidTokenInternal(szToken);
                    task.Wait();
                    if (task.Result) {
                        return true;
                    }
                }
            }
            return false;
        }
    }

}
