using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Api.Domain.Abstract;
using GetStat.Domain.Annotations;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly AppDbContext _dbContext;
        private string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpPost]
        public async Task<ApiResponse<int>> CreateTest(Test test)
        {
            test.AccountId = UserId;

            return new ApiResponse<int>
            {
                Response = await _testService.CreateTest(test)
            };
        }

        [HttpPost]
        public async Task<ApiResponse<List<Test>>> GetMyTests()
        {
            return new ApiResponse<List<Test>>
            {
                Response = await _testService.GetMyTests(UserId)
            };
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse<Test>> JoinTest([FromBody] string[] param)
        {
            if (string.IsNullOrEmpty(param[0]))
            {
                return new ApiResponse<Test>
                {
                    Error = "Код не может быть пустым"
                };
            }

            var test = await _testService.JoinTest(param[0]);

            if (test == null)
                return new ApiResponse<Test>
                {
                    Error = "Тест по коду не найден"
                };

            if (await _testService.IsPassedTest(test.TestId,param[1]))
            {
                return new ApiResponse<Test>
                {
                    Error = "Вы уже прошли тест"
                };
            }

            var set = await _testService.CheckTestSettingTime(test.Settings,TimeZoneInfo.Utc.Id);

            if (string.IsNullOrEmpty(set))
                return new ApiResponse<Test>
                {
                    Response = test
                };

            return new ApiResponse<Test>
            {
                Error = set
            };

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse<ResultTest>> EndTest(BaseResultQA baseResult)
        {
            return new ApiResponse<ResultTest>
            {
                Response = await _testService.EndTest(baseResult,UserId)
            };
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTest([FromBody] int testId)
        {
            var res = await _testService.RemoveTest(testId);

            if (res)
                return Ok();
            return Problem();
        }

        [HttpPost]
        public async Task<ApiResponse<List<ResultTest>>> GetResult([FromBody] int testId)
        {
            var result = await _testService.GetResult(testId);
            return new ApiResponse<List<ResultTest>>
            {
                Response = result
            };
        }

        [HttpPost]
        public async Task<ApiResponse<Test>> EditTest([FromBody] int testId)
        {
            var test = await _testService.EditTest(testId);
            if (test == null)
                return new ApiResponse<Test>
                {
                    Error = "Тест не найден"
                };
            return new ApiResponse<Test>
            {
                Response = test
            };
        }

        [HttpPost]
        public async Task<ApiResponse<int>> UpdateTest([FromBody] Test test)
        {
            return new()
            {
                Response = await _testService.UpdateTest(test)
            };
        }

        [HttpPost]
        public async Task<ApiResponse<List<ResultTest>>> GetResultTest()
        {
            var res = await _testService.GetResultTest(UserId);

            return new ApiResponse<List<ResultTest>>
            {
                Response = res
            };
        }

        [HttpPost]
        public async Task<ApiResponse<ResultTest>> GetResultQuestions([FromBody] int resultTestId)
        {
            var res = await _testService.GetResultQuestions(resultTestId);

            if (res == null)
                return new ApiResponse<ResultTest>
                {
                    Error = "Произошла ошибка"
                };

            return new ApiResponse<ResultTest>
            {
                Response = res
            };
        }
        [HttpPost]
        public async Task<ApiResponse<Setting>> GetTestHeader([FromBody]int testId)
        {
            Setting settings = await _testService.GetTestHeader(testId);

            if (settings == null)
                return new ApiResponse<Setting>
                {
                    Error="Тест не найден"
                };

            return new ApiResponse<Setting>
            {
                Response = settings
            };
        }
    }
}