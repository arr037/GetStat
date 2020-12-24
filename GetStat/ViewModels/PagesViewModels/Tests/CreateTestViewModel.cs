using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Dna;
using GetStat.Commands;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class CreateTestViewModel:BaseVM
    {
        private readonly ModalService _modalService;
        private readonly LoginResponseService _loginResponseService;
        public bool IsShowPreview { get; private set; } = false;
        public bool IsShowSettings { get; private set; } = false;
        public ObservableCollection<Question> Questions { get; set; }
        public TimeSpan StartTime { get; set; }

        public Setting TestSettings { get; set; } = new Setting
        {
            StartDay = DateTime.Now
        };

        public CreateTestViewModel(ModalService modalService,
            LoginResponseService loginResponseService)
        {
            _modalService = modalService;
            _loginResponseService = loginResponseService;
            Questions = new ObservableCollection<Question>();
        }

      
        public DelegateCommand<Question> AddNewAnswer => new DelegateCommand<Question>(question =>
        {
            question.Answers.Add(new Answer());
        });

        public ICommand SaveCommand=> new DelegateCommand(async () =>
        {
           var error= NotCorrectQuestion();
           if(!error)
               return;

           var newTest = new Test
            {
                Questions = Questions.ToList(),
                Settings = TestSettings
            };

            var response = await WebRequests.PostAsync<ApiResponse<int>>
            ("https://localhost:5001/api/test/CreateTest", newTest,
                bearerToken:_loginResponseService.LoginResponse.Token);

            var res = response.DisplayErrorIfFailedAsync();

            if (!res.SuccessFul)
            {
                _modalService.ShowModalWindow("Ошибка","Произошла ошибка при добавлении теста");
                return;
            }
            Questions.Clear();
            _modalService.ShowModalWindow("Новый тест", "Тест успешно добавлен в базу.\nПосмотреть можете в разделе Мои Тесты");
            
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //if (saveFileDialog.ShowDialog() == true)
            //{
            //    saveFileDialog.AddExtension = true;
            //    saveFileDialog.FileName = "json"; // Default file name
            //    saveFileDialog.DefaultExt = ".json"; // Default file extension
            //    saveFileDialog.Filter = "Text documents (.json)|*.json"; // Filter files by extension

            //    var json = JsonConvert.SerializeObject(newTest);
            //    File.WriteAllText(saveFileDialog.FileName, json);
            //}

        });
        public DelegateCommand CreateQuestion=> new DelegateCommand(() =>
        {
            var q = new Question
            {
                Answers = new List<Answer>(Enumerable.Range(0, 4)
                    .Select(_ => new Answer())),
                

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

        public ICommand ExportQuestions => new DelegateCommand(() =>
        {
            var b = StartTime;
            //var b = NotCorrectQuestion();
            //if (b.Any())
            //{
            //    var bs = string.Join(",", b.Select(x => x.QuestionNum));
            //    _modalService.ShowModalWindow("Ошибка",$"В {bs} вопросах не выбран правильный ответ!\nИсправьте и повторите попытку");
            //}
            //MessageBox.Show(b.First().Quest);

            //string json = JsonConvert.SerializeObject(Questions, Formatting.Indented);
        });

        public DelegateCommand SettingsCommand => new DelegateCommand(()=>
        {
            IsShowSettings = true;
        });

        public ICommand CloseSettingsCommand => new DelegateCommand(() =>
        {
            IsShowSettings = false;
        });

        private bool NotCorrectQuestion()
        {
            var errors = new List<string>();
            
            if (Questions.Count == 0)
            {
                errors.Add("Создайте первый вопрос!\nПовторите попытку...");
               
            }

            var quest = Questions.Where(question => question.Answers.All(x => x.IsSelected == false)).ToList();

            if (quest.Any())
            {
                var bs = string.Join(",", quest.Select(x => Questions.IndexOf(x)));
                errors.Add($"В {bs} вопросах не выбран правильный ответ!\nИсправьте и повторите попытку");

            }

            if (string.IsNullOrWhiteSpace(TestSettings.TestName))
            {
                errors.Add("Введите название теста");
            }

            if (Questions.Count < Convert.ToInt32(TestSettings.MaxQuestion))
            {
                errors.Add("Максимальное количество вопросов не должно превышать количество вопрсов в базе");
            }

            if (TestSettings.StartDay.Date.ToFileTimeUtc() <= DateTime.Now.Date.ToFileTimeUtc())
            {
                errors.Add("Введите дату превышаюший сегодняшний день");
            }

            if (errors.Count > 0)
            {
                _modalService.ShowModalWindow("Ошибка",string.Join(@"\n",errors));
                return false;
            }

            return true;
        }

    }
}
