using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Api.Domain.Abstact;
using GetStat.Api.Services;
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
        //private readonly AppDbContext _dbContext;
        private readonly ITestService _testSerivce;
        private string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public TestController(ITestService testSerivce)
        {
            //_dbContext = dbContext;
            _testSerivce = testSerivce;
        }

        /// <summary>
        /// Создание теста
        /// </summary>
        /// <param name="test">Сущность</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<string>> CreateTest(Test test)
        {
            test.AccountId = UserId;

            foreach (var question in test.Questions)
            {
                var first = question.Answers.FirstOrDefault(x => x.IsSelected);

                if (first != null) question.CorrectAnswer = first.AnswerId;
            }
            
            var rs =  await _testSerivce.CreateTest(test);
           
            return new ApiResponse<string>
            {
                Response = rs
            };
        }

        /// <summary>
        /// Список тестов 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<List<Test>>> GetMyTests()
        {
            return new ApiResponse<List<Test>>
            {
                Response = await _testSerivce.GetTests(UserId)
        };
        }

        /// <summary>
        /// Вход на тест по коду
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
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

            if (param.Length != 2)
            {
                return  new ApiResponse<Test>
                {
                    Error = "Произошла ошибка"
                };
            }

            return await _testSerivce.JoinTest(param[0], param[1]);

        }



        /// <summary>
        /// Заверщение теста
        /// </summary>
        /// <param name="baseResult"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse<ResultTest>> EndTest(BaseResultQA baseResult)
        {
            return await _testSerivce.EndTest(baseResult, UserId);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTest([FromBody] int testId)
        {
            var res = await _testSerivce.DeleteTest(testId);

            if (res == EntityState.Detached || res== EntityState.Deleted)
                return Ok();
            return Problem();
        }

        [HttpPost]
        public async Task<ApiResponse<List<ResultTest>>> GetResult([FromBody] int testId)
        {
            return await _testSerivce.GetResult(testId);
        }

        [HttpPost]
        public async Task<ApiResponse<Test>> EditTest([FromBody] int testId)
        {
            return  await _testSerivce.EditTest(testId);
        }

        [HttpPost]
        public async Task<ApiResponse<int>> UpdateTest([FromBody]Test test)
        {
            
           foreach (var question in test.Questions)
           {
               if (question.CorrectAnswer!=-1)
                   continue;

               var first = question.Answers.FirstOrDefault(x => x.IsSelected);

               if (first != null) question.CorrectAnswer = first.AnswerId;
           }

           await _testSerivce.UpdateTest(test);

           return new ApiResponse<int>
           {
               Response = test.TestId
           };
        }

        [HttpPost]
        public async Task<ApiResponse<List<ResultTest>>> GetResultTest()
        {
            return  await _testSerivce.GetResultTest(UserId);
        }
    }
}