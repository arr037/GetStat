using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GetStat.Domain.Models;
using GetStat.Domain.Models.Test;
using Microsoft.AspNetCore.SignalR.Client;

namespace GetStat.Services
{
    public class SignalRTestService
    {
        private readonly HubConnection _connection;
        public string Token { get; set; }
        public event Action<string> AddedTest; 
        public SignalRTestService()
        {
            _connection = new HubConnectionBuilder().WithUrl("https://localhost:5001/testHub").Build();

            _connection.Closed += Connection_Closed;

            _connection.On<string>("AddedNewTest", s => AddedTest?.Invoke(s));

        }

        public async Task CreateTest(Test test,string token= null)
        {
            Token = token;
            await _connection.SendAsync("AddNewTest", test);
        }

        public async void Connect()
        {
            await Task.Run(async () =>
            {
                await _connection.StartAsync().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        GiveQuestion(task.Exception);
                    }
                });
            });
        }

        private Task Connection_Closed(Exception arg)
        {
            GiveQuestion(arg);
            
            return Task.CompletedTask;
        }

        
        public async Task SendPermissionToEnterTheTest(int testId, string fullName)
        {
            await _connection.SendAsync("SendPermissionToEnterTheTest", _connection.ConnectionId, testId, fullName);
        }


        public async Task SendAllowOrDenyJoinTest(BaseTestHub baseTest,bool allowDeny)
        {
            await _connection.SendAsync("SendAllowOrDenyJoinTest", baseTest,allowDeny);
        }

        private void GiveQuestion(Exception arg)
        {
            if (MessageBox.Show(arg.Message + "\n\nПопытаться повторно подключиться?", "Error",
                MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                Connect();
            }
        }
    }
}
