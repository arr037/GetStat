using System.ComponentModel;
using System.Runtime.CompilerServices;
using GetStat.Annotations;

namespace GetStat.Models
{
    public class BaseVM : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}