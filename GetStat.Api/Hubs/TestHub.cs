    using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models;
using Microsoft.AspNetCore.SignalR;
    using Org.BouncyCastle.Math.EC.Rfc7748;

    namespace GetStat.Api.Hubs
{
    public class TestHub:Hub
    {
        private static ConcurrentDictionary<int,List<BaseTestHub>> _activeTests = new ConcurrentDictionary<int, List<BaseTestHub>>();
        public async  Task SendPermissionToEnterTheTest(string connectionId,int testId,string fullName)
        {
            List<BaseTestHub> aciveTest;
            if (_activeTests.TryGetValue(testId, out aciveTest))
            {
                if (aciveTest.All(x => x.ConnectionId != connectionId))
                {
                    aciveTest.Add(new BaseTestHub
                    {
                        ConnectionId = connectionId,
                        FullName = fullName
                    });
                }
            }
            else
            {
                _activeTests.TryAdd(testId, new List<BaseTestHub>
                {
                    new BaseTestHub
                    {
                        FullName = fullName,
                        ConnectionId = connectionId
                    }
                });
            }

            //Срабатывание учителю когда кто то добавился
            await Clients.Group("ttss" + testId.ToString()).SendAsync("JoinInTest",aciveTest,testId);
        }

        public async Task GetPermissionToEnterTheTest(int testId)
        {
            List<BaseTestHub> aciveTest;
            if (_activeTests.TryGetValue(testId, out aciveTest))
            {
                await Clients.Group("ttss"+testId).SendAsync("ReciveGetPermissionToEnterTheTest",aciveTest);
            }
        }


        public async Task SendAllowOrDenyJoinTest(BaseTestHub baseTestHub,bool allowordeny)
        {
            await Clients.Client(baseTestHub.ConnectionId).SendAsync("ReciveAllowOrDenyJoinTest", allowordeny);
        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var activeTestsValue in _activeTests.Values)
            {
                foreach (BaseTestHub baseTestHub in activeTestsValue)
                {
                    if (activeTestsValue.Any(x=>x.ConnectionId==Context.ConnectionId))
                    {
                        activeTestsValue.Remove(baseTestHub);
                    }
                }
            }
            
            return base.OnDisconnectedAsync(exception);
        }
    }
}
