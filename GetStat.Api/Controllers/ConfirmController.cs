using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<Account> _userManager;

        public ConfirmController(AppDbContext dbContext,UserManager<Account> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async  Task<string> Check(string id,string token)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
                return "BadRequest";

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            if (account == null)
                return "Not Found";


            var checkCode = await _userManager.ConfirmEmailAsync(account,token.Base64ForUrlDecode());

            if (checkCode.Succeeded)
            {
                return "Okay";
            }
            else
            {
                return string.Join('\n', checkCode.Errors.Select(x=>x.Description));
            }
        }

        [HttpPost]
        public async Task<bool> CheckIsConfirmEmail(string id)
        {
            var user = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return false;

            return user.EmailConfirmed;
        }
    }
}