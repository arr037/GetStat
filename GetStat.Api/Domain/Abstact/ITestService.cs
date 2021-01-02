using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Base;
using GetStat.Domain.Models.Test;
using Microsoft.EntityFrameworkCore;

namespace GetStat.Api.Domain.Abstact
{
    public interface ITestService
    {
        Task<List<Test>> GetTests(string userId);
        Task<ApiResponse<Test>> JoinTest(string code, string fullName);
        Task<ApiResponse<ResultTest>> EndTest(BaseResultQA baseResult,string userId);
        Task<ApiResponse<List<ResultTest>>> GetResult(int testId);
        Task<string> CreateTest(Test entity);
        Task UpdateTest(Test entity);
        Task<ApiResponse<Test>> EditTest(int testId);
        Task<EntityState> DeleteTest(int id);
        Task<ApiResponse<List<ResultTest>>> GetResultTest(string userId);
    }
}
