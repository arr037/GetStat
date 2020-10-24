using System.Linq;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;

        public ConfirmController(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<string> Check(string id, string token)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
                return "BadRequest";

            var account = await _userManager.FindByIdAsync(id);
            if (account == null)
                return "Not Found";


            var checkCode = await _userManager.ConfirmEmailAsync(account, token.Base64ForUrlDecode());

            if (checkCode.Succeeded)
                return "Okay";
            return string.Join('\n', checkCode.Errors.Select(x => x.Description));
        }
    }
}