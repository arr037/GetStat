using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GetStat.Api.Domain;
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
        private readonly AppDbContext _dbContext;
        private string UserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public TestController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<ApiResponse<int>> CreateTest(Test test)
        {
            test.AccountId = UserId;

            var b = await _dbContext.Tests.AddAsync(test);
            var t = b.Entity;

            foreach (var question in test.Questions)
            {
                var first = question.Answers.FirstOrDefault(x => x.IsSelected);

                if (first != null) question.CorrectAnswer = first.AnswerId;
            }

            await _dbContext.SaveChangesAsync();
            return new ApiResponse<int>
            {
                Response = b.Entity.TestId
            };
        }

        [HttpPost]
        public async Task<ApiResponse<List<Test>>> GetMyTests()
        {
            return new ApiResponse<List<Test>>
            {
                Response = await _dbContext.Tests.AsNoTracking()
                .Include(x => x.Settings)
                .Include(x => x.Questions)
                .ThenInclude(ar => ar.Answers)
                .Where(f => f.AccountId == UserId).ToListAsync()
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

            var test = await _dbContext.Tests.AsNoTracking()
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
                .FirstOrDefaultAsync(a => a.Settings.Code.ToLower()
                                          == param[0].ToLower());


            if (test == null)
                return new ApiResponse<Test>
                {
                    Error = "Тест по коду не найден"
                };

            if (await _dbContext.ResultTests.AnyAsync(x =>
                x.TestId == test.TestId && x.FullName.ToLower().Trim() == param[1].ToLower().Trim()))
            {
                return new ApiResponse<Test>
                {
                    Error = "Вы уже прошли тест"
                };
            }

            var setting = test.Settings;

            if (setting.StartDay.Date.ToFileTimeUtc() != DateTime.UtcNow.Date.ToFileTimeUtc())
                return new ApiResponse<Test>
                {
                    Error = $"Дата начала теста: {setting.StartDay}"
                };

            if (DateTime.Now.TimeOfDay >= setting.StartTime && DateTime.Now.TimeOfDay <= setting.EndTime)
            {
                return new ApiResponse<Test>
                {
                    Response = test
                };
            }

            return new ApiResponse<Test>
            {
                Error = $"Что то не так: startTime {setting.StartTime} ; endTime {setting.EndTime}"
            };
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ApiResponse<ResultTest>> EndTest(BaseResultQA baseResult)
        {
            var res = 0;
            var answers = new List<ResultQueston>();

            foreach (var qa in baseResult.ResultQas)
            {
                var question = await _dbContext.Questions.Include(y => y.Answers)
                    .FirstOrDefaultAsync(x => x.QuestionId == qa.QuestionId);

                if (question == null)
                {
                    res++;
                    continue;
                }

                var resultQuestion = new ResultQueston
                {
                    Question = question.Quest,
                    ResultAnswers = new List<ResultAnswer>(
                        question.Answers.Select(x => new ResultAnswer
                        {
                            Answer = x.Ans,
                            IsCorrect = x.AnswerId == question.CorrectAnswer,
                            IsUserCorrect = qa.AnswerId == x.AnswerId
                        }))
                };


                if (question.CorrectAnswer == qa.AnswerId)
                {
                    res++;
                }

                answers.Add(resultQuestion);
            }

            var resultTest = await _dbContext.ResultTests.AddAsync(new ResultTest
            {
                FullName = baseResult.FullName,
                AllCountQuestion = baseResult.ResultQas.Count,
                CorrectCountQuestion = res,
                ResultQuestons = answers,
                TestId = baseResult.TestId,
                AccountId = !string.IsNullOrEmpty(UserId)?UserId:null,
                TestName = baseResult.TestName
            });

            await _dbContext.SaveChangesAsync();

            return new ApiResponse<ResultTest>
            {
                Response = resultTest.Entity
            };
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTest([FromBody] int testId)
        {
            var res = _dbContext.Tests.Remove(new Test
            {
                TestId = testId
            });

            await _dbContext.SaveChangesAsync();

            if (res.State == EntityState.Detached)
                return Ok();
            return Problem();
        }

        [HttpPost]
        public async Task<ApiResponse<List<ResultTest>>> GetResult([FromBody] int testId)
        {
            var result = await _dbContext.ResultTests.Select(x => new ResultTest
                {
                    TestId = x.TestId,
                    FullName = x.FullName,
                    AllCountQuestion = x.AllCountQuestion,
                    CorrectCountQuestion = x.CorrectCountQuestion,
                    ResultTestId = x.ResultTestId
                })
                .Where(x => x.TestId == testId).ToListAsync();
            return new ApiResponse<List<ResultTest>>
            {
                Response = result
            };
        }

        [HttpPost]
        public async Task<ApiResponse<Test>> EditTest([FromBody] int testId)
        {
            var test = await _dbContext.Tests
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .Include(x => x.Settings)
                .FirstOrDefaultAsync(x => x.TestId == testId);

            if (test == null)
            {
                return new ApiResponse<Test>
                {
                    Error = "Тест в базе не найден"
                };
            }

            foreach (var firstOrDefault in from question in test.Questions where question.CorrectAnswer != -1 select question.Answers.FirstOrDefault(x => x.AnswerId == question.CorrectAnswer) into firstOrDefault where firstOrDefault != null select firstOrDefault)
            {
                firstOrDefault.IsSelected = true;
            }


            return new ApiResponse<Test>
            {
                Response = test
            };
        }

        [HttpPost]
        public async Task<ApiResponse<int>> UpdateTest([FromBody]Test test)
        {

            var questions = _dbContext.Questions.Include(x=>x.Answers)
                .Where(x => x.TestId == test.TestId);

            _dbContext.Questions.RemoveRange(questions);

            _dbContext.Tests.Update(test);

            foreach (var question in test.Questions)
            {
                var first = question.Answers.FirstOrDefault(x => x.IsSelected);

                if (first != null) question.CorrectAnswer = first.AnswerId;
            }

            await _dbContext.SaveChangesAsync();

            return new ApiResponse<int>
            {
                Response = test.TestId
            };
        }

        [HttpPost]
        public async Task<ApiResponse<List<ResultTest>>> GetResultTest()
        {
            var res = await _dbContext.ResultTests
                .Include(x=>x.ResultQuestons)
                    .ThenInclude(a=>a.ResultAnswers)
                .Where(x => x.AccountId == UserId)
                .ToListAsync();

            return new ApiResponse<List<ResultTest>>
            {
                Response = res
            };
        }

        [HttpPost]
        public async Task<ApiResponse<ResultTest>>GetResultQuestions([FromBody]int resultTestId)
        {
            var res = await _dbContext.ResultTests
                .Include(x=>x.ResultQuestons)
                .ThenInclude(a=>a.ResultAnswers).
                FirstOrDefaultAsync(x => x.ResultTestId == resultTestId);

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
    }
}