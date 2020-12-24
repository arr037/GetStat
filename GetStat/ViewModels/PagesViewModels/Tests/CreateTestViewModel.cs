using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain.Models.Questions;
using GetStat.Models;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class CreateTestViewModel:BaseVM
    {
        public bool IsShowPreview { get; private set; } = false;
        public bool IsShowSettings { get; private set; } = false;
        public ObservableCollection<Question> Questions { get; set; }


        public CreateTestViewModel()
        {
            Questions = new ObservableCollection<Question>
            {
                new Question
                {
                    QuestionNum = 1,
                    Quest = "Вопрос 1",
                    Answers = new ObservableCollection<Answer>
                    {
                        new Answer
                        {
                            Ans = "C6376E4C-D331-4912-A289-8B591CBA5012"
                        },
                        new Answer
                        {
                            Ans = "C6376E4C-D331-4912-A289-8B591CBA5012"
                        },
                        new Answer
                        {
                            Ans = "C6376E4C-D331-4912-A289-8B591CBA5012"
                        },
                        new Answer
                        {
                            Ans = "C6376E4C-D331-4912-A289-8B591CBA5012"
                        }
                    }
                }
            };
        }

        public DelegateCommand<Question> AddNewAnswer => new DelegateCommand<Question>(question =>
        {
            question.Answers.Add(new Answer());
        });


        public DelegateCommand CreateQuestion=> new DelegateCommand(() =>
        {
            var q = new Question
            {
                QuestionNum = Questions.Count + 1,
                Answers = new ObservableCollection<Answer>(Enumerable.Range(0,4).Select(_=> new Answer()))
            };
            Questions.Add(q);
        });

        public DelegateCommand<Answer> DeleteAnswerCommand => new DelegateCommand<Answer>((item) =>
        {
            var b =Questions.FirstOrDefault(x => x.Answers.Contains(item));
            if (b == null) return;

            b.Answers.Remove(item);
        });

        public ICommand RemoveQuestion => new DelegateCommand<Question>(item =>
        {
            Questions.Remove(item);
        });


        public DelegateCommand PreviewCommand => new DelegateCommand(() =>
        {
            IsShowPreview = true;
        });

        public DelegateCommand SettingsCommand => new DelegateCommand(()=>
        {
            IsShowSettings = true;
        });
    }
}
