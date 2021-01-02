    using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
    using System.Security.Claims;
    using System.Text;
using System.Threading.Tasks;
    using GetStat.Api.Domain;
    using GetStat.Api.Domain.Abstact;
    using GetStat.Api.Services;
    using GetStat.Domain.Models;
    using GetStat.Domain.Models.Test;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Org.BouncyCastle.Math.EC.Rfc7748;

    namespace GetStat.Api.Hubs
{
    public class TestHub:Hub
    {
        private readonly ITestService _service;

        public TestHub(ITestService service)
        {
            _service = service;
        }
        [Authorize]
        public async Task AddNewTest(Test test)
        {
            test.AccountId = Context.User.Claims.FirstOrDefault(x=>x.Type ==ClaimTypes.NameIdentifier)?.Value;

            foreach (var question in test.Questions)
            {
                var first = question.Answers.FirstOrDefault(x => x.IsSelected);

                if (first != null) question.CorrectAnswer = first.AnswerId;
            }

            var res = await _service.CreateTest(test);
            await Clients.Caller.SendAsync("AddedNewTest",res);
        }


    }
}
