using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GetStat.Commands;
using GetStat.Domain.Models;
using GetStat.Domain.Services;
using GetStat.Models;

namespace GetStat.ViewModels.PagesViewModels.Tests
{
    public class BaseDic
    {
        public string Header { get; set; }
        public ObservableCollection<QueueTest> QueneTest { get; set; }   
    }

    public class RequestPageViewModel:BaseVM
    {
        private readonly HubService _hubService;
        public ObservableCollection<BaseDic> SortingQuenes{ get; set; }

        public RequestPageViewModel(HubService hubService)
        {
            _hubService = hubService;
            _ = _hubService.GetQueneUsers();
            SortingQuenes = new ObservableCollection<BaseDic>();
            hubService.OnNewQueune += HubService_OnNewQueune;
            hubService.OnCancelQueune += HubService_OnCancelQueune;
            hubService.OnAllQuene += HubService_OnAllQuene;

            //SortingQuenes.
        }

        private void HubService_OnAllQuene(List<QueueTest> obj)
        {
            foreach (var queueTest in obj)
            {
                var b = SortingQuenes.FirstOrDefault(x => x.Header == queueTest.TestName);

                if (b == null)
                {
                    SortingQuenes.Add(new BaseDic
                    {
                        Header = queueTest.TestName,
                        QueneTest = new ObservableCollection<QueueTest>
                        {
                            queueTest
                        }
                    });
                }
                else
                {
                    b.QueneTest.Add(queueTest);
                }
            }
        }

        private void HubService_OnCancelQueune(QueueTest obj)
        {
            var owner = SortingQuenes.FirstOrDefault(x => x.QueneTest.Any(x=>x.FullName==obj.FullName && x.TestId==obj.TestId));

            if (owner != null)
            {
                var b = owner.QueneTest.FirstOrDefault(x => x.FullName == obj.FullName);
                
                if(b!=null)
                    owner.QueneTest.Remove(b);


                if (owner.QueneTest.Count == 0)
                {
                    SortingQuenes.Remove(owner);
                }
            }
        }

        private void HubService_OnNewQueune(QueueTest obj)
        {
            var b = SortingQuenes.FirstOrDefault(x => x.Header == obj.TestName);

            if (b == null)
            {
                SortingQuenes.Add(new BaseDic
                {
                    Header = obj.TestName,
                    QueneTest = new ObservableCollection<QueueTest>
                    {
                        obj
                    }
                });
            }
            else
            {
                b.QueneTest.Add(obj);
            }
        }

        public ICommand AddCommand => new DelegateCommand<string>((str) =>
        {
            //var b=  SortingQuenes.FirstOrDefault(x => x.Header == str);

            //if (b == null)
            //{
            //    SortingQuenes.Add(new BaseDic
            //    {
            //        Header = str,
            //        QueneTest = new ObservableCollection<QueueTest>
            //        {
            //            new QueueTest
            //            {
            //                FullName = "b is null: "+ str 
            //            }
            //        }
            //    });
            //}
            //else
            //{
            //    b.QueneTest.Add(new QueueTest
            //    {
            //        FullName = "b is not null: "+ str
            //    });
            //}
        });

        public ICommand AcceptCommand => new DelegateCommand<QueueTest>(test =>
        {
            _ = _hubService.AllowOrDenyJoin(test.ConnectionId, true, test.TestId);
            _ = RemoveFromCollection(test);
        });

        public ICommand RejectCommand => new DelegateCommand<QueueTest>(test =>
        {
            _ = _hubService.AllowOrDenyJoin(test.ConnectionId, false, test.TestId);
           _=RemoveFromCollection(test);
        });

        private Task RemoveFromCollection(QueueTest test)
        {
            var owner = SortingQuenes.FirstOrDefault(x => x.QueneTest.Any(x => x.FullName == test.FullName && x.TestId == test.TestId));

            if (owner != null)
            {
                var b = owner.QueneTest.FirstOrDefault(x => x.FullName == test.FullName);

                if (b != null)
                    owner.QueneTest.Remove(b);


                if (owner.QueneTest.Count == 0)
                {
                    SortingQuenes.Remove(owner);
                }
            }
            return Task.CompletedTask;
        }

    }
}
