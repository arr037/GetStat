using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GetStat.Domain.Models.Test;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Cms;

namespace GetStat.Domain.Services
{
    public class CodeHubService
    {
        private HubConnection connection;
        public event Action<string> OnConnected;
        public event Action<HubConnectionState> OnConnectionStateChanged;
        public event Action<string> ReceiveJoinTest; 
        public event Action<bool,Test> RecieveAllowOrDeny; 
        public CodeHubService()
        {
            connection = new HubConnectionBuilder().WithUrl("http://localhost:5000/code").AddMessagePackProtocol().Build();

            connection.On<string>("ReceiveJoinTest", (err) =>
            {
                ReceiveJoinTest?.Invoke(err);
            });

            connection.On<bool,Test>("RecieveAllowOrDeny", (flag,test) =>
            {
                RecieveAllowOrDeny?.Invoke(flag,test);
            });

        }

        public async Task JoinTest(string fullName,string code,string timeZoneInfo)
        {
            await connection.SendAsync("JoinInTest", fullName, code,timeZoneInfo);
        }

        public async Task CancelJoinTest()
        {
            await connection.SendAsync("CancelJoinTest");
        }

        public async Task Connect()
        {
            if (connection.State != HubConnectionState.Connected)
            {
                await connection.StartAsync();
                OnConnectionStateChanged?.Invoke(connection.State);
            }
           
        }

        public async Task Disconnect()
        {
            await connection.StopAsync();
            OnConnectionStateChanged?.Invoke(connection.State);
        }
    }
}
