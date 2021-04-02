using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Api.Domain.Abstract;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using GetStat.Domain.Models.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Hubs
{
    public class GetStatHub:Hub
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<CodeHub> _codeHubContext;
        private readonly ITestService _testService;
        private string UserId => Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        public GetStatHub(AppDbContext context,IHubContext<CodeHub> codeHubContext,ITestService testService)
        {
            _context = context;
            _codeHubContext = codeHubContext;
            _testService = testService;
        }

        public async Task JoinInTest(string fullname, string code)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(x => x.Code == code);

            if (setting != null)
            {
                if (await _testService.IsPassedTest(setting.TestId, fullname))
                {
                    await Clients.Caller.SendAsync("ReceiveJoinTest", "Вы уже прошли тест");
                    return;
                }

                var set = await _testService.CheckTestSettingTime(setting,TimeZoneInfo.Local.Id);
                if (!string.IsNullOrEmpty(set))
                {
                    await Clients.Caller.SendAsync("ReceiveJoinTest", set);
                    return;
                }

                var qTest = new QueueTest
                {
                    FullName = fullname,
                    TestId = setting.TestId,
                    ConnectionId = Context.ConnectionId
                };
                await _context.QueueTests.AddAsync(qTest);
                await _context.SaveChangesAsync();
                var test = await _context.Tests.FirstOrDefaultAsync(x => x.TestId == setting.TestId);
                await Clients.Client(test.ConnectionId).SendAsync("NewReceiveJoinTest", qTest);
                await Clients.Caller.SendAsync("ReceiveJoinTest", "");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveJoinTest", "Тест не найден");
            }

        }

        public async Task CancelJoinTest()
        {
            var s = await _context.QueueTests.FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);

            if (s != null)
            {
                _context.QueueTests.Remove(s);
                await _context.SaveChangesAsync();
                var test = await _context.Tests.FirstOrDefaultAsync(x => x.TestId == s.TestId);
                await Clients.Client(test.ConnectionId).SendAsync("CancelReceiveJoinTest", s);
            }
        }

        [Authorize]
        public async Task GetJoinTestUsers()
        {
            var userTestsId = await _context.Tests.Where(x => x.AccountId == UserId).Select(x => x.TestId).ToListAsync();
            var tests = await _context.QueueTests.Where(x => userTestsId.Contains(x.TestId)).ToListAsync();
            await Clients.Caller.SendAsync("ReceiveJoinTestUsers",tests);
        }

        public async Task AllowOrDenyJoinTest(string connectionId,bool flag,int testId)
        {
            if (flag)
            {
                var test = await _context.Tests.AsNoTracking()
                    .Include(x => x.Settings)
                    .Include(a => a.Questions)
                    .ThenInclude(x => x.Answers)
                    .Select(x => new Test
                    {
                        Settings = x.Settings,
                        Questions = x.Questions.Select(a => new Question
                        {
                            Answers = a.Answers.Select(a => new Answer
                            {
                                AnswerId = a.AnswerId,
                                Ans = a.Ans
                            }).ToRandom(),
                            Quest = a.Quest,
                            QuestionId = a.QuestionId
                        }).ToRandom(Convert.ToInt32(x.Settings.MaxQuestion)),
                        TestId = x.TestId
                    })
                    .FirstOrDefaultAsync(a => a.TestId == testId);

                _= _codeHubContext.Clients.Client(connectionId).SendAsync("RecieveAllowOrDeny", true, test);
                _= Clients.Client(connectionId).SendAsync("RecieveAllowOrDeny", true, test);
            }
            else
            {
                _= _codeHubContext.Clients.Client(connectionId).SendAsync("RecieveAllowOrDeny", false, null);
                _= _codeHubContext.Clients.Client(connectionId).SendAsync("RecieveAllowOrDeny", false, null);
            }

            await RemoveFromDb(connectionId);
        }


        public async Task SetTestConnectionId(string timeZone)
        {
            var userTestsId = await _context.Tests.Include(x=>x.Settings)
                .Where(x => x.AccountId == UserId).Select(x => x.TestId).ToListAsync();
            
            var tests = await _context.Tests.Include(x=>x.Settings).Where(x => userTestsId.Contains(x.TestId)).ToListAsync();
            
            
            
            if (tests.Count != 0)
            {
                foreach (Test test in tests)
                {
                   var check = await _testService.CheckTestSettingTime(test.Settings, timeZone);

                   if (string.IsNullOrEmpty(check))
                   {
                       test.ConnectionId = Context.ConnectionId;
                   }
                }
                await _context.SaveChangesAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var tests = await _context.Tests.Where(x => x.ConnectionId == Context.ConnectionId).ToListAsync();

            foreach (var test in tests)
            {
                test.ConnectionId = string.Empty;
            }

            await _context.SaveChangesAsync();
            await base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            return base.OnConnectedAsync();
        }


        private async Task RemoveFromDb(string connectionId)
        {
            var s = await _context.QueueTests.FirstOrDefaultAsync(x => x.ConnectionId ==connectionId);

            if (s != null)
            {
                _context.QueueTests.Remove(s);
                await _context.SaveChangesAsync();
            }
        }
    }
}
