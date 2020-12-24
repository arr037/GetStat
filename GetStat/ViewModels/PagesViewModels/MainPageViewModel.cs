using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Dna;
using GetStat.Commands;
using GetStat.Domain.Base;
using GetStat.Domain.Extetrions;
using GetStat.Domain.Models.Menu;
using GetStat.Domain.Models.Tabs;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Pages;
using GetStat.Pages.Authorization;
using GetStat.Pages.Main.Test;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels
{
    public class MainPageViewModel:BaseVM
    {
        private readonly LoginResponseService _loginResponseService;
        private readonly PageService _pageService;
        private readonly ModalService _modalService;
        private readonly EventBus _eventBus;
        public ObservableCollection<ItemText> MenuCollection { get; private set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ShortName { get; set; }
        public ITab SelectedTab { get; set; }
        public ObservableCollection<ITab> Tabs { get; set; }

        public MainPageViewModel(LoginResponseService loginResponseService,
            PageService pageService,
            ModalService modalService,
            EventBus eventBus)
        {
            _loginResponseService = loginResponseService;
            _pageService = pageService;
            _modalService = modalService;
            _eventBus = eventBus;

            Name = loginResponseService.LoginResponse?.Name;
            Surname = loginResponseService.LoginResponse?.Surname;

            ShortName = Name?.FirstOrDefault().ToString();

            MenuCollection = new ObservableCollection<ItemText>
            {
                new ItemText()
                {
                    Name = "К Тесту",
                    IconImage = "\uf067",
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
                    Name = "Претензии",
                    IconImage = "\uf296",
                    Page = new ClaimsPage()
                },

            };
            Tabs = new ObservableCollection<ITab>();
            Tabs.CollectionChanged += Tabs_CollectionChanged;
        }


        public ICommand CloseTab=> new DelegateCommand<ITab>(item =>
        {
            Tabs.Remove(item);
        });
        public ICommand LogOutCommand => new DelegateCommand( () =>
         {
              _loginResponseService.Clear();
             _pageService.Navigate(new SignIn());
         });
        public ICommand AddItemToTabs=> new DelegateCommand<ItemText>(async (item) =>
        {
            SelectedTab = Tabs.AddUnique(new Tab
            {
                Name = item.Name,
                Page = item.Page
            });
            if (SelectedTab.Name == "Мои тесты")
                await _eventBus.Publish(new OnOpenMenu());
            
            
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
        private void Tab_CloseRequired(object sender, EventArgs e)
        {
            Tabs.Remove((ITab)sender);
        }
    }

    public class OnOpenMenu : IEvent
    {

    }
}
