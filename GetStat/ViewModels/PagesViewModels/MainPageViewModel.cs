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
using GetStat.Models;
using GetStat.Pages;
using GetStat.Pages.Authorization;
using GetStat.Pages.Main.Pages;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels
{
    public class MainPageViewModel:BaseVM
    {



        private readonly AuthorizationService _authorizationService;
        private readonly PageService _pageService;
        private readonly ModalService _modalService;
        public ObservableCollection<ItemText> MenuCollection { get; private set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public string ShortName { get; set; }
        public Page CurrentPage { get; set; }
        public ITab SelectedTab { get; set; }
        public ObservableCollection<ITab> Tabs { get; set; }

        public MainPageViewModel(AuthorizationService authorizationService,PageService pageService,ModalService modalService)
        {
            _authorizationService = authorizationService;
            _pageService = pageService;
            _modalService = modalService;

            Name = authorizationService.LoginResponse?.Name;
            Surname = authorizationService.LoginResponse?.Surname;
            MiddleName = authorizationService.LoginResponse?.MiddleName;

            ShortName = Name?.FirstOrDefault() + Surname?.FirstOrDefault().ToString();

            MenuCollection = new ObservableCollection<ItemText>
            {
                new ItemText()
                {
                    Name = "Создать тест",
                    IconImage = "\uf067",
                    Page = new CreateTestPage()
                },
                new ItemText()
                {
                    Name = "Редактировать тест",
                    IconImage = "\uf040"
                },
                new ItemText()
                {
                    Name = "Удалить тест",
                    IconImage = "\uf1f8"
                },
                new ItemText()
                {
                    Name = "Мои тесты",
                    IconImage = "\uf16c"
                },
                new ItemText()
                {
                    Name = "Запросы",
                    IconImage = "\uf1e9"
                },
                new ItemText()
                {
                    Name = "Результаты",
                    IconImage = "\uf201"
                },
                new ItemText()
                {
                    Name = "Претензии",
                    IconImage = "\uf296"
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
              _authorizationService.LogOut();
             _pageService.Navigate(new SignIn());
         });

        public ICommand AddItemToTabs=> new DelegateCommand<ItemText>((item) =>
        {
            SelectedTab = Tabs.AddUnique(new Tab
            {
                Name = item.Name,
                Page = item.Page
            });
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
}
