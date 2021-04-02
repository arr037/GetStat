using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models;
using GetStat.Domain.Models.Event;
using GetStat.Domain.Models.Menu;
using GetStat.Domain.Models.Tabs;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Pages;
using GetStat.Pages.Authorization;
using GetStat.Pages.Help;
using GetStat.Pages.Main.Test;
using GetStat.Reporting;
using GetStat.Services;
using GetStat.ViewModels.PagesViewModels.Tests;
using GetStat.ViewModels.PagesViewModels.Tests.StartTest;
using Microsoft.AspNetCore.SignalR.Client;
using NetCoreAudio;

namespace GetStat.ViewModels.PagesViewModels
{
    public class MainPageViewModel : BaseVM
    {
        
        private readonly LoginResponseService _loginResponseService;
        private readonly HubService _hubService;
        private readonly PageService _pageService;
        private readonly ModalService _modalService;
        private readonly EventBus _eventBus;
        private readonly MediaPlayerService _mediaPlayerService;
        public bool HasNewPush { get; set; }
        public ObservableCollection<BaseEventMessage> EventCollection { get; set; }
        public HubConnectionState ConnectionState { get; set; }
        public ObservableCollection<ItemText> MenuCollection { get; private set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ShortName { get; set; }
        public ITab SelectedTab { get; set; }
        public ObservableCollection<ITab> Tabs { get; set; }

        public MainPageViewModel(LoginResponseService loginResponseService,HubService hubService,
            PageService pageService,
            ModalService modalService,
            EventBus eventBus,MediaPlayerService mediaPlayerService)
        {
            _loginResponseService = loginResponseService;
            
            _hubService = hubService;
            _pageService = pageService;
            _modalService = modalService;
            _eventBus = eventBus;
            _mediaPlayerService = mediaPlayerService;
            _eventBus.Subscribe<OnTeacherResult>(TeacherResult);
            _eventBus.Subscribe<OnEditTest>(EditTest);
            _eventBus.Subscribe<OnCloseTab>(ClsTab);
            _eventBus.Subscribe<OnUserResult>(UserResult);
            _eventBus.Subscribe<OnPrintResultTest>(PrintResultTest);
            Name = loginResponseService.LoginResponse?.Name;
            Surname = loginResponseService.LoginResponse?.Surname;

            ShortName = Name?.FirstOrDefault().ToString();

            MenuCollection = new ObservableCollection<ItemText>
            {
                new ItemText()
                {
                    Name = "К Тесту",
                    IconImage = "\uf05b",
                    Page = new JoinWithCode()
                },
                new ItemText()
                {
                    Name = "Создать тест",
                    IconImage = "\uf067",
                    Page = new CreateTestPage()
                },
                new ItemText()
                {
                    Name = "Мои тесты",
                    IconImage = "\uf16c",
                    Page = new MyTestsPage()
                },
                new ItemText()
                {
                    Name = "Результаты",
                    IconImage = "\uf201",
                    Page = new GetResultPage()
                },
                new ItemText()
                {
                    Name = "Запросы",
                    IconImage = "\uf201",
                    Page = new RequestPage()
                },
            };
            Tabs = new ObservableCollection<ITab>();
            Tabs.CollectionChanged += Tabs_CollectionChanged;

            _ = _hubService.Connect();
            _hubService.OnConnectionStateChanged += (state) =>
            {
                ConnectionState = state;
            };

            EventCollection = new ObservableCollection<BaseEventMessage>();
            _hubService.OnNewPush += (name,txt) =>
            {
                EventCollection.Add(new BaseEventMessage
                {
                    FullName = name,
                    Text = txt
                });
                HasNewPush = true;
               _= mediaPlayerService.Play(true);
            };

            //_=  hubService.Connect(loginResponseService.LoginResponse?.Token);

        }

      

       

        private Task PrintResultTest(OnPrintResultTest arg)
        {
            var page = new SampleControl(arg.OrderFormHeader, arg.ResultTests);

            SelectedTab = Tabs.AddUnique(new Tab
            {
                Name = "ПредПросмотр печати",
                Page = page
            });

            return Task.CompletedTask;
        }

        private Task UserResult(OnUserResult arg)
        {
            var datacontext = Ioc.Resolve<GetResultPageViewModel>();
            datacontext.FullName = arg.FullName;
            datacontext.ResultQuestons = arg.ResultQuestons;
            datacontext.IsUserResult = true;
            datacontext.AllCountQuestion = arg.All.ToString();
            datacontext.CorrectCountQuestion = arg.Correct.ToString();

            var pg = new ResultTestPage
            {
                DataContext = datacontext
            };

            SelectedTab = Tabs.AddUnique(new Tab
            {
                Name = $"Ответы: {arg.FullName}",
                Page = pg
            });
            return Task.CompletedTask;
        }

        private Task ClsTab(OnCloseTab arg)
        {
            Tabs.Remove(SelectedTab);
            return Task.CompletedTask;
        }

        private Task EditTest(OnEditTest arg)
        {
            var datacontext = Ioc.Resolve<CreateTestViewModel>();
            datacontext.Questions = new ObservableCollection<Question>(arg.EditTest.Questions);
            datacontext.TestSettings = arg.EditTest.Settings;
            datacontext.TestType = TestType.Edit;
            datacontext.Test = arg.EditTest;

            var pg = new CreateTestPage
            {
                DataContext = datacontext
            };

            SelectedTab = Tabs.AddUnique(new Tab
            {
                Name = "Редактирование теста",
                Page = pg
            });
            return Task.CompletedTask;
        }

        private Task TeacherResult(OnTeacherResult arg)
        {
            var datacontext = Ioc.Resolve<TeacherResultViewModel>();
            datacontext.ResultTests = new ObservableCollection<ResultTest>(arg.ResultTests);

            var pg = new TeacherResultPage
            {
                DataContext = datacontext
            };

            SelectedTab = Tabs.AddUnique(new Tab
            {
                Name = "Ответы",
                Page = pg
            });
            return Task.CompletedTask;
        }

        public ICommand CloseTab => new DelegateCommand<ITab>(item =>
         {
             item.cancellationToken.Cancel();
             Tabs.Remove(item);
         });

        public ICommand LogOutCommand => new DelegateCommand(async () =>
        {
           await _mediaPlayerService.Play();
            //_ = _hubService.Disconnect();
            //LoginResponseService.Clear();
            //PageService.Navigate(new SignIn());
        });

        public ICommand AddItemToTabs => new DelegateCommand<ItemText>(async (item) =>
         {
             SelectedTab = Tabs.AddUnique(new Tab
             {
                 Name = item.Name,
                 Page = item.Page
             });

             if (SelectedTab?.Name == "Мои тесты")
                 await EventBus.Publish(new OnOpenMenu
                 {
                     MenuType = MenuType.MyTest,
                     cancellationToken = SelectedTab.cancellationToken
                 });

             if (SelectedTab?.Name == "Результаты")
                 await EventBus.Publish(new OnOpenMenu
                 {
                     MenuType = MenuType.ResultTest,
                     cancellationToken = SelectedTab.cancellationToken
                 });
         });

        public LoginResponseService LoginResponseService => _loginResponseService;

        public PageService PageService => _pageService;

        public ModalService ModalService => _modalService;

        public EventBus EventBus => _eventBus;
        public bool IsShow { get; set; } = false;
        public ICommand OpenPopup => new DelegateCommand(() =>
        {
            //var s= new MediaPlayerService();
            HasNewPush = false;
            IsShow = true;
        });

        private void Tabs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ITab tab;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    tab = (ITab)e.NewItems[0];
                    tab.CloseRequired += Tab_CloseRequired;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    tab = (ITab)e.OldItems[0];
                    tab.CloseRequired -= Tab_CloseRequired;
                    break;
            }
        }

        public ICommand SettingsCommand => new DelegateCommand(() =>
        {
            _pageService.NavigateWithAnimation(new HelperPage(),PageAnimation.SlideAndFadeInFromTop,PageAnimation.SlideAndFadeOutToBottom);
        });

        private void Tab_CloseRequired(object sender, EventArgs e)
        {
            var s = (ITab)sender;
            s.cancellationToken.Cancel();
            s.Page = null;
            Tabs.Remove(s);
            GC.Collect();
        }
    }

    public class OnOpenMenu : IEvent
    {
        public MenuType MenuType { get; set; }
        public CancellationTokenSource cancellationToken { get; set; }
    }

    public enum MenuType
    {
        MyTest,
        ResultTest
    }
}