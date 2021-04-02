using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Reporting;
using System;
using System.Collections.Generic;

namespace GetStat.Domain.Models.Event
{
    public class OnCloseTab : IEvent { }
    public class OnTeacherResult : IEvent
    {
        public List<ResultTest> ResultTests { get; }

        public OnTeacherResult(List<ResultTest> resultTests)
        {
            ResultTests = resultTests;
        }

    }

    public class OnEditTest : IEvent
    {
        public Test.Test EditTest { get; }

        public OnEditTest(Test.Test editTest)
        {
            EditTest = editTest;
        }
    }

    public class OnResultTest : IEvent
    {
        public ResultTest List { get; }
        public string Fullname { get; }
        public OnResultTest(ResultTest list,string fullname)
        {
            List = list;
            Fullname = fullname;
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

    public class OnUserResult:IEvent
    {
        public OnUserResult(List<ResultQueston> resultQuestons,string fullName,int correct,int all)
        {
            ResultQuestons = resultQuestons;
            FullName = fullName;
            Correct = correct;
            All = all;
        }

        public List<ResultQueston> ResultQuestons { get; }
        public string FullName { get; }
        public int Correct { get; }
        public int All { get; }
    }

    public class OnPrintResultTest:IEvent
    {
        public IReadOnlyList<ResultTest> ResultTests { get; }
        public OrderFormHeader OrderFormHeader { get; }

        public OnPrintResultTest(OrderFormHeader orderFormHeader,IReadOnlyList<ResultTest> resultTests)
        {
            OrderFormHeader = orderFormHeader;
            ResultTests = resultTests;
        }

    }

    public class OnCancelRequestToHub : IEvent
    {

    }
}