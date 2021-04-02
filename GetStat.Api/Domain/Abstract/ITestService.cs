using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models.Test;

namespace GetStat.Api.Domain.Abstract
{
    public interface ITestService
    {
        Task<int> CreateTest(Test test);
        Task<List<Test>> GetMyTests(string userId);
        Task<Test> JoinTest(string code);
        Task<bool> IsPassedTest(int testId, string fullName);
        Task<string> CheckTestSettingTime(Setting setting,string info);
        Task<ResultTest> EndTest(BaseResultQA baseResult, string UserId);
        Task<bool> RemoveTest(int testId);
        Task<List<ResultTest>> GetResult(int testId);
        Task<Test> EditTest(int testId);
        Task<int> UpdateTest(Test test);
        Task<List<ResultTest>> GetResultTest(string userId);
        Task<ResultTest> GetResultQuestions(int resultTestId);
        Task<Setting> GetTestHeader(int testId);
    }
}
