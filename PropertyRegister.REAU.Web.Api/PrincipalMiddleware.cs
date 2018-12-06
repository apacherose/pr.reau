using CNSys.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Web.Api
{
    public class PrincipalMiddleware
    {
        private readonly RequestDelegate _next;
        private const string TestUserClientID = "1";

        public PrincipalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var user = httpContext.User;
            var testPrincipal = new TestPrincipal(user, TestUserClientID);

            httpContext.User = testPrincipal;
            System.Threading.Thread.CurrentPrincipal = testPrincipal;

            await _next(httpContext);
        }
    }

    public static class PrincipalMiddlewareExtensions
    {
        public static IApplicationBuilder UsePrincipalMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PrincipalMiddleware>();
        }
    }

    public class TestPrincipal : ClaimsPrincipal, IDataSourceUser
    {
        public TestPrincipal(IPrincipal principal, string clientID) : base(principal)
            => ClientID = clientID;        

        public string ClientID { get; }

        public string ProxyUserID => throw new NotImplementedException();
    }
}
