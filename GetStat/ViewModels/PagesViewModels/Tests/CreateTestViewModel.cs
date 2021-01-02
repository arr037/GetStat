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
using GetStat.Domain.Models.Event;
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
        private readonly EventBus _eventBus;
        private readonly SignalRTestService _testService;
        internal Test Test { get; set; }
        public bool IsShowSettings { get; private set; } = false;
        public ObservableCollection<Question> Questions { get; set; }
        public TimeSpan StartTime { get; set; }
        public TestType TestType { get; internal set; } = TestType.Create;
        public Setting TestSettings { get; set; } = new Setting
        {
            StartDay = DateTime.Now
        };

        public CreateTestViewModel(ModalService modalService,
            LoginResponseService loginResponseService,
            EventBus eventBus,
            SignalRTestService testService)
        {
            _modalService = modalService;
            _loginResponseService = loginResponseService;
            _eventBus = eventBus;
            _testService = testService;
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

           if (TestType==TestType.Create)
           {
               var newTest = new Test
               {
                   Questions = Questions.ToList(),
                   Settings = TestSettings
               };
               
               await _testService.CreateTest(newTest,_loginResponseService.LoginResponse.Token);

               //var response = await WebRequests.PostAsync<ApiResponse<int>>
               //("https://localhost:5001/api/test/CreateTest", newTest,
               //    bearerToken:_loginResponseService.LoginResponse.Token);

               //var res = response.DisplayErrorIfFailedAsync();

               //if (!res.SuccessFul)
               //{
               //    _modalService.ShowModalWindow("Ошибка",res.Message);
               //    return;
               //}
               //Questions.Clear();
               //TestSettings = new Setting();
               //_modalService.ShowModalWindow("Новый тест", "Тест успешно добавлен в базу.\nПосмотреть можете в разделе Мои Тесты");
               //GC.Collect();
           }
           else
           {
               Test.Settings = TestSettings;
               Test.Questions = Questions.ToList();
               var response = await WebRequests.PostAsync<ApiResponse<int>>
               ("https://localhost:5001/api/test/UpdateTest", Test,
                   bearerToken: _loginResponseService.LoginResponse.Token);

               var res = response.DisplayErrorIfFailedAsync();

               if (!res.SuccessFul)
               {
                   _modalService.ShowModalWindow("Ошибка", res.Message);
                   return;
               }

               Questions.Clear();
               TestSettings = new Setting();
               _modalService.ShowModalWindow("Тест", "Тест успешно обновлен");
               await _eventBus.Publish(new OnCloseTab());
               GC.Collect();

           }
        });
        public DelegateCommand CreateQuestion=> new DelegateCommand(() =>
        {
            TestSettings ??= new Setting
            {
                StartDay = DateTime.Now
            };

            var q = new Question
            {
                Answers = new ObservableCollection<Answer>(Enumerable.Range(0, 4)
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
            var error = NotCorrectQuestion();
            if (!error)
                return;


            var saveFileDialog = new SaveFileDialog
            {
                FileName = "test - " + TestSettings.TestName,
                DefaultExt = ".json",
                Filter = "Json File (.json)|*.json"
            };

            // Default file name
            // Default file extension
            // Filter files by extension
            if (saveFileDialog.ShowDialog() == true)
            {
                var test = new Test
                {
                    Settings = TestSettings,
                    Questions = Questions.ToList()
                };
                File.WriteAllTextAsync(saveFileDialog.FileName, JsonConvert.SerializeObject(test, Formatting.Indented));
            }
        });
        public ICommand ImportQuestions => new DelegateCommand(async () =>
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".json", Filter = "Json File (.json)|*.json"
            };

            // Default file extension
            // Filter files by extension
            if (openFileDialog.ShowDialog() == true)
            {
                var des = JsonConvert.DeserializeObject<Test>(await File.ReadAllTextAsync(openFileDialog.FileName));
                Questions = new ObservableCollection<Question>(des.Questions);
                TestSettings = des.Settings;
            }
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

    public enum TestType
    {
        Create,
        Edit
    }
}
