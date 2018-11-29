using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyRegister.REAU.Applications;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Web.Api.Controllers
{
    public class ApplicationsController : BaseApiController
    {
        private readonly IApplicationAcceptanceService ApplicationService;

        public ApplicationsController(IApplicationAcceptanceService applicationService)
        {
            ApplicationService = applicationService;
        }

        [HttpGet]
        //[RequiredOperationHeader]
        public IActionResult Get()
        {
            return Ok(new { id = 5, oid = RequestOperationID });
        }

        [HttpPost]
        [RequiredOperationHeader]
        public async Task<IActionResult> AcceptApplication(IFormFile file)
        {
            var result = await ApplicationService.AcceptApplicationAsync(RequestOperationID, file.OpenReadStream());

            return Ok(result);
        }
    }
}
