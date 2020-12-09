using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dna;
using GetStat.Domain.Models.Test;
using GetStat.Models;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class MyTestViewModel:BaseVM
    {
        public List<Test> Tests { get; set; }

        public MyTestViewModel()
        {
            
            //Tests = res.Result.ServerResponse;
            LoadTests();
        }

        private async void LoadTests()
        {
            await Task.Run(async () =>
            {
                var res = await WebRequests.PostAsync<List<Test>>
                ("https://localhost:5001/api/test/GetMyTests",
                    bearerToken:
                    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidXJzZWtlIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIwODIxOGQ0Yy0zZTg1LTRjZTQtYTEzYS01YWY4NGRlNmJhYmEiLCJuYmYiOiIxNjA3MTAyNzI1IiwiZXhwIjoiMTYwNzEwOTkyNSJ9.4ZI23N3t_eGtAJ3VqFHl-xR9uuTcxFa1SRRj9nh61Ew"
                );
                Tests = res.ServerResponse;
            });
        }
    }
}
