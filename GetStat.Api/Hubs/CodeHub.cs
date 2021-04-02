using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Api.Domain.Abstract;
using GetStat.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Hubs
{
    public class CodeHub:Hub
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<GetStatHub> _getstatHubContext;
        private readonly ITestService _testService;

        public CodeHub(AppDbContext context,IHubContext<GetStatHub> getstatHubContext,ITestService testService)
        {
            _context = context;
            _getstatHubContext = getstatHubContext;
            _testService = testService;
        }

        public async Task JoinInTest(string fullname,string code,string timeZoneInfo)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(x => x.Code == code);

            if (setting != null)
            {
                if (await _testService.IsPassedTest(setting.TestId, fullname))
                {
                    await Clients.Caller.SendAsync("ReceiveJoinTest", "Вы уже прошли тест");
                    return;
                }

                var set = await _testService.CheckTestSettingTime(setting,timeZoneInfo);
                if (!string.IsNullOrEmpty(set))
                {
                    await Clients.Caller.SendAsync("ReceiveJoinTest", set);
                    return;
                }

                var qTest = new QueueTest
                {
                    FullName = fullname,
                    TestId = setting.TestId,
                    TestName = setting.TestName,
                    ConnectionId = Context.ConnectionId
                };
                await _context.QueueTests.AddAsync(qTest);
                await _context.SaveChangesAsync();
                var test = await _context.Tests.FirstOrDefaultAsync(x => x.TestId == setting.TestId);
                await _getstatHubContext.Clients.Client(test.ConnectionId).SendAsync("NewReceiveJoinTest",qTest);
                await Clients.Caller.SendAsync("ReceiveJoinTest","");
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
                await _getstatHubContext.Clients.Client(test.ConnectionId).SendAsync("CancelReceiveJoinTest", s);
            }
        }
        public async Task StartedTest(int testId,string fullName)
        {
            var test = await _context.Tests.FirstOrDefaultAsync(x => x.TestId == testId);
            if (test != null)
            {
                var conId = test.ConnectionId;
                await Clients.Client(conId).SendAsync("ReceiveStartedTest",fullName);
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var s =await  _context.QueueTests.FirstOrDefaultAsync(x => x.ConnectionId == Context.ConnectionId);

            if (s != null)
            {
                _context.QueueTests.Remove(s);
                await _context.SaveChangesAsync();
                var test = await _context.Tests.FirstOrDefaultAsync(x => x.TestId == s.TestId);
                await _getstatHubContext.Clients.Client(test.ConnectionId).SendAsync("CancelReceiveJoinTest", s);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
