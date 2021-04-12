using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Api.Domain.Abstract;
using GetStat.Api.Hubs;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Test;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Domain.Ef
{
    public class TestService:ITestService
    {
        private readonly AppDbContext _dbContext;

        public TestService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateTest(Test test)
        {
            var b = await _dbContext.Tests.AddAsync(test);
            var t = b.Entity;

            foreach (var question in test.Questions)
            {
                var first = question.Answers.FirstOrDefault(x => x.IsSelected);

                if (first != null) question.CorrectAnswer = first.AnswerId;
            }

            await _dbContext.SaveChangesAsync();
            return b.Entity.TestId;
        }

        public async Task<List<Test>> GetMyTests(string userId)
        {
            return await _dbContext.Tests.AsNoTracking()
                .Include(x => x.Settings)
                .Include(x => x.Questions)
                .ThenInclude(ar => ar.Answers)
                .Where(f => f.AccountId == userId).ToListAsync();
        }

        public async Task<Test> JoinTest(string code)
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

            return test;

        }

        public async Task<bool> IsPassedTest(int testId, string fullName)
        {
            return await _dbContext.ResultTests.AnyAsync(x =>
                x.TestId == testId && x.FullName.ToLower().Trim() == fullName.ToLower().Trim());
        }

        public Task<string> CheckTestSettingTime(Setting setting,string info)
        {
            var testDay = setting.StartDay.GmtToPacific(info);
            var currentTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(info);
            var s = TimeZoneInfo.ConvertTime(DateTime.Now, currentTimeZoneInfo);
            if (testDay.Date != s.Date)
                return Task.FromResult($"Дата начала теста: {setting.StartDay}");

            if (DateTime.Now.TimeOfDay >= setting.StartTime && DateTime.Now.TimeOfDay <= setting.EndTime)
            {
                return Task.FromResult(string.Empty);
            }

            return Task.FromResult($"Что то не так: startTime {setting.StartTime} ; endTime {setting.EndTime}");
        }



        public async Task<ResultTest> EndTest(BaseResultQA baseResult,string UserId)
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
                AccountId = !string.IsNullOrEmpty(UserId) ? UserId : null,
                TestName = baseResult.TestName
            });

            await _dbContext.SaveChangesAsync();

            return resultTest.Entity;
        }

        public async Task<bool> RemoveTest(int testId)
        {
            var res = _dbContext.Tests.Remove(new Test
            {
                TestId = testId
            });

            await _dbContext.SaveChangesAsync();

            return res.State == EntityState.Detached;
        }

        public async Task<List<ResultTest>> GetResult(int testId)
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
            return result;
        }

        public async Task<Test> EditTest(int testId)
        {
            var test = await _dbContext.Tests
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .Include(x => x.Settings)
                .FirstOrDefaultAsync(x => x.TestId == testId);

            if (test == null)
            {
                return null;
            }

            foreach (var firstOrDefault in from question in test.Questions where question.CorrectAnswer != -1 select question.Answers.FirstOrDefault(x => x.AnswerId == question.CorrectAnswer) into firstOrDefault where firstOrDefault != null select firstOrDefault)
            {
                firstOrDefault.IsSelected = true;
            }

            return test;
        }

        public async Task<int> UpdateTest(Test test)
        {
            var questions = _dbContext.Questions.Include(x => x.Answers)
                .Where(x => x.TestId == test.TestId);

            _dbContext.Questions.RemoveRange(questions);

            _dbContext.Tests.Update(test);

            foreach (var question in test.Questions)
            {
                var first = question.Answers.FirstOrDefault(x => x.IsSelected);

                if (first != null) question.CorrectAnswer = first.AnswerId;
            }

            await _dbContext.SaveChangesAsync();
            return test.TestId;
        }

        public async Task<List<ResultTest>> GetResultTest(string userId)
        {
            var res = await _dbContext.ResultTests
                .Include(x => x.ResultQuestons)
                .ThenInclude(a => a.ResultAnswers)
                .Where(x => x.AccountId == userId)
                .ToListAsync();
            return res;
        }

        public async Task<ResultTest> GetResultQuestions(int resultTestId)
        {
            var res = await _dbContext.ResultTests
                .Include(x => x.ResultQuestons)
                .ThenInclude(a => a.ResultAnswers).
                FirstOrDefaultAsync(x => x.ResultTestId == resultTestId);

            return res;
        }

        public async Task<Setting> GetTestHeader(int testId)
        {
            Setting settings = await _dbContext.Settings.FirstOrDefaultAsync(x => x.TestId == testId);
            return settings;
        }
    }
}
