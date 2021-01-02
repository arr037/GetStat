using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Api.Domain;
using GetStat.Api.Domain.Abstact;
using GetStat.Api.Hubs;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Test;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Services
{
    public class TestSerivce:ITestService
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<TestHub> _hubContext;

        public TestSerivce(AppDbContext dbContext,IHubContext<TestHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        public async Task<List<Test>> GetTests(string userId)
        {
            return await _dbContext.Tests
                .Include(x => x.Settings)
                .Include(x => x.Questions)
                .Where(f => f.AccountId == userId).ToListAsync();
        }

        
        public async Task<ApiResponse<Test>> JoinTest(string code,string fullName)
        {
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
                                          == code.ToLower());


            if (test == null)
                return new ApiResponse<Test>
                {
                    Error = "Тест по коду не найден"
                };

            if (await _dbContext.ResultTests.AnyAsync(x =>
                x.TestId == test.TestId && x.FullName.ToLower().Trim() == fullName.ToLower().Trim()))
            {
                return new ApiResponse<Test>
                {
                    Error = "Вы уже прошли тест"
                };
            }

            var setting = test.Settings;

            if (setting.StartDay.Date.ToFileTimeUtc() != DateTime.UtcNow.Date.ToFileTimeUtc() &&
                !(DateTime.Now.TimeOfDay >= setting.StartTime && DateTime.Now.TimeOfDay <= setting.EndTime))
                return new ApiResponse<Test>
                {
                    Error = $"Произошла ошибка"
                };

           
            return new ApiResponse<Test>
            {
                Response = test
            };
                
        }

        public async Task<ApiResponse<List<ResultTest>>> GetResult(int testId)
        {
            var result = await _dbContext.ResultTests.Select(x => new ResultTest
                {
                    TestId = x.TestId,
                    FullName = x.FullName,
                    AllCountQuestion = x.AllCountQuestion,
                    CorrectCountQuestion = x.CorrectCountQuestion
                })
                .Where(x => x.TestId == testId).ToListAsync();

            return new ApiResponse<List<ResultTest>>
            {
                Response = result
            };
        }

        public async Task<ApiResponse<ResultTest>> EndTest(BaseResultQA baseResult,string userId)
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
                AccountId = !string.IsNullOrEmpty(userId) ? userId: null,
                TestName = baseResult.TestName
            });

            await _dbContext.SaveChangesAsync();

            return new ApiResponse<ResultTest>
            {
                Response = resultTest.Entity
            };
        }

        public async Task<string> CreateTest(Test entity)
        {
            var en = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return en.Entity.Settings.TestName;
        }

        public async Task UpdateTest(Test entity)
        {
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<ApiResponse<Test>> EditTest(int testId)
        {
            var test = await _dbContext.Tests
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .Include(x => x.Settings)
                .FirstOrDefaultAsync(x => x.TestId == testId);


            if (test == null)
                return new ApiResponse<Test>
                {
                    Error = "Тест в базе не найден"
                };

            foreach (var firstOrDefault in from question in test.Questions where question.CorrectAnswer != -1 select question.Answers.FirstOrDefault(x => x.AnswerId == question.CorrectAnswer) into firstOrDefault where firstOrDefault != null select firstOrDefault)
            {
                firstOrDefault.IsSelected = true;
            }


            return new ApiResponse<Test>
            {
                Response = test
            };
        }

        public async Task<EntityState> DeleteTest(int id)
        {
            var res =  _dbContext.Tests.Remove(new Test()
            {
                TestId = id
            });
            await _dbContext.SaveChangesAsync();
            return res.State;
        }

        public async Task<ApiResponse<List<ResultTest>>> GetResultTest(string userId)
        {
            var res = await _dbContext.ResultTests
                .Include(x => x.ResultQuestons)
                .ThenInclude(a => a.ResultAnswers)
                .Where(x => x.AccountId == userId)
                .ToListAsync();

            return new ApiResponse<List<ResultTest>>
            {
                Response = res
            };
        }
    }
}
