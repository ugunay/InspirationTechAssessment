using InspirationTechAssessment.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InspirationTechAssessment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{id}/balance")]
        public async Task<ActionResult<decimal>> GetBalanceAsync(long id)
        {
            return Ok(await _accountService.GetBalanceAsync(id));
        }
    }
}