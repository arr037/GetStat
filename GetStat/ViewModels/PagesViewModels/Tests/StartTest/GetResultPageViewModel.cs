﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain.Models.Event;
using GetStat.Domain.Models.Test;
using GetStat.Domain.Services;
using GetStat.Models;
using GetStat.Pages.Authorization;
using GetStat.Pages.Main;
using GetStat.Services;

namespace GetStat.ViewModels.PagesViewModels.Tests.StartTest
{
    public class GetResultPageViewModel:BaseVM
    {
        private readonly EventBus eventBus;
        private readonly LoginResponseService _loginResponseService;
        private readonly PageService _pageService;
        public string AllCountQuestion { get; set; }
        public string CorrectCountQuestion { get; set; }
        public List<ResultQueston> ResultQuestons { get; set; }
        public string FullName { get; set; }
        public bool IsUserResult { get; set; } = false;
        public GetResultPageViewModel(EventBus eventBus,LoginResponseService loginResponseService,PageService pageService)
        {
            this.eventBus = eventBus;
            _loginResponseService = loginResponseService;
            _pageService = pageService;
            eventBus.Subscribe<OnResultTest>(LoadResultTests);
        }

        public ICommand BackPage=> new DelegateCommand(async() =>
        {
            if (IsUserResult)
            {
                await eventBus.Publish(new OnCloseTab());
                return;
            }

            if (_loginResponseService.LoginResponse == null)
            {
                _pageService.NavigateWithAnimation(new SignIn());
            }
            else
            {
                _pageService.NavigateWithAnimation(new MainPage());
            }
        });

        private Task LoadResultTests(OnResultTest arg)
        {
            ResultQuestons = arg.List.ResultQuestons;
            AllCountQuestion = arg.List.AllCountQuestion.ToString();
            CorrectCountQuestion = arg.List.CorrectCountQuestion.ToString();
            FullName = arg.Fullname;
            return Task.CompletedTask;
        }

    }
}
