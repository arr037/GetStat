using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class ConfirmController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;
        private string UserId => User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

        public ConfirmController(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        [Route("api/ConfirmEmail")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<string> ConfirmEmail(string token,string id)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(id))
                return "BadRequest";

            var account = await _userManager.FindByIdAsync(id);
            if (account == null)
                return "Not Found";


            var checkCode = await _userManager.ConfirmEmailAsync(account, token.Base64ForUrlDecode());

            if (checkCode.Succeeded)
                return "<b>Your email is activated!</b>";
            return string.Join('\n', checkCode.Errors.Select(x => x.Description));
        }

        [HttpPost]
        [Route("api/IsConfirm")]
        public async Task<ApiResponse<bool>> IsConfirm()
        {
            var account = await _userManager.FindByIdAsync(UserId);
            if (account == null)
                return new ApiResponse<bool>
                {
                    Error = "Аккаунт не найден"
                };

            return new ApiResponse<bool>
            {
                Response = await _userManager.IsEmailConfirmedAsync(account)
            };
        }
    }
}