using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace GetStat.Services
{
    public class SignalRTestService
    {
        private readonly HubConnection connection;
        public SignalRTestService()
        {
            connection = new HubConnectionBuilder().WithUrl("https://localhost:5001/testHub").Build();
            Connect();
        }

        public async void Connect()
        {
            await Task.Run(async () =>
            {
                await connection.StartAsync().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        MessageBox.Show(task.Exception.Message);
                    }
                });
            });
        }

        public async Task SendPermissionToEnterTheTest(int testId, string fullName)
        {
            await connection.SendAsync("SendPermissionToEnterTheTest", connection.ConnectionId, testId, fullName);
        }

    }
}
