using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GetStat.Domain.Models;
using GetStat.Domain.Models.Test;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace GetStat.Domain.Services
{
    public class HubService
    {
        private HubConnection connection;
        public event Action<HubConnectionState> OnConnectionStateChanged;
        public event Action<bool, Test> RecieveAllowOrDeny;
        public event Action<string> ReceiveJoinTest;
        public event Action<QueueTest> OnNewQueune;
        public event Action<QueueTest> OnCancelQueune;
        public event Action<List<QueueTest>> OnAllQuene;
        public event Action<string,string> OnNewPush;
        public event Action<string> OnSetConnectionId;
        public event Action ReceiveCreateTest; 
        public HubService(LoginResponseService loginResponseService)
        {
            connection = new HubConnectionBuilder().WithUrl(Config.UrlAddress+"getstat", opt =>
            {
                opt.AccessTokenProvider = () => Task.FromResult(loginResponseService.LoginResponse?.Token);
            }).AddMessagePackProtocol().Build();



            #region Push Messages
            connection.On<List<QueueTest>>("ReceiveJoinTestUsers", (items) =>
            {
                OnAllQuene?.Invoke(items);
            });

            connection.On<string>("ReceiveSetConnectionId", str =>
            {
                OnSetConnectionId?.Invoke(str);
            });

            connection.On<QueueTest>("NewReceiveJoinTest", (item) =>
            {
                OnNewQueune?.Invoke(item);
                OnNewPush?.Invoke(item.FullName, "хочет пройти тест");
            });
            connection.On<QueueTest>("CancelReceiveJoinTest", (item) =>
            {
                OnCancelQueune?.Invoke(item);
                OnNewPush?.Invoke(item.FullName, "отменил свой запрос");
            });

            #endregion

            #region JoinTest

            connection.On<string>("ReceiveJoinTest", (err) =>
            {
                ReceiveJoinTest?.Invoke(err);
            });

            connection.On<bool, Test>("RecieveAllowOrDeny", (flag, test) =>
            {
                RecieveAllowOrDeny?.Invoke(flag, test);
            });


            #endregion


            #region MainPageReceives

            connection.On("ReceiveCreateTest", () =>
            {
                ReceiveCreateTest?.Invoke();
            });

            #endregion
        }

        #region MainPageCommand

        public async Task CreateTest(Test test)
        {
            await connection.SendAsync("CreateTest", test);
        }
        

        #endregion

        #region QueneUserCommands

        public async Task JoinTest(string fullName, string code, string timeZoneInfo)
        {
            await connection.SendAsync("JoinInTest", fullName, code, timeZoneInfo);
        }

        public async Task CancelJoinTest()
        {
            await connection.SendAsync("CancelJoinTest");
        }



        public async Task AllowOrDenyJoin(string connectionId,bool flag,int testId)
        {
            await connection.SendAsync("AllowOrDenyJoinTest", connectionId,flag,testId);
        }



        public async Task GetQueneUsers()
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("GetJoinTestUsers");
            }
        }

        #endregion

        #region Connec/Disconect/SetConnId

        public async Task SetConnectionIdTest()
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("SetTestConnectionId", TimeZoneInfo.Utc.Id);
            }
        }

        public async Task Connect()
        {
            if (connection.State != HubConnectionState.Connected)
            {
                await connection.StartAsync();
                _ = SetConnectionIdTest();
                OnConnectionStateChanged?.Invoke(connection.State);
            }
        }

        public async Task Disconnect()
        {
            await connection.StopAsync();
            OnConnectionStateChanged?.Invoke(connection.State);
        }

        #endregion

    }
}
