using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace PropertyRegister.REAU.Web.Api.Controllers
{
    [Route("api/[controller]")]
    public class BaseApiController : Controller
    {
        protected string RequestOperationID =>
            HttpContext.Request.Headers["Operation-Id"].SingleOrDefault();
    }

    public class RequiredOperationHeaderAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var operationHeader = context.HttpContext.Request.Headers["Operation-Id"].SingleOrDefault();

            if (string.IsNullOrEmpty(operationHeader))
            {
                context.Result = new BadRequestResult();
            }
        }
    }
}
