using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;

namespace GetStat.Domain.Models.Event
{
    public class OnResultTest : IEvent
    {
        public ResultTest List { get; }

        public OnResultTest(ResultTest list)
        {
            List = list;
        }
    }
    public class OnStartTest : IEvent
    {
        public string QuestionName { get; }
        public string QuestionCount { get; }
        public TimeSpan Time { get; }
        public List<Question> Questions { get; }
        public string FullName { get; }
        public int TestId { get; }
        public OnStartTest(List<Question> questions, string questionName
            , string questionCount, TimeSpan time, string fullName, int testId)
        {
            Questions = questions;
            QuestionName = questionName;
            QuestionCount = questionCount;
            Time = time;
            FullName = fullName;
            TestId = testId;
        }
    }
    public class OnEditTest : IEvent
    {
        public Test.Test EditTest { get;  }

        public OnEditTest(Test.Test editTest)
        {
            EditTest = editTest;
        }
    }
    public class OnTeacherResult : IEvent
    {
        public List<ResultTest> ResultTests { get; }

        public OnTeacherResult(List<ResultTest> resultTests)
        {
            ResultTests = resultTests;
        }

    }

    public class OnCloseTab : IEvent
    {

    }
}
