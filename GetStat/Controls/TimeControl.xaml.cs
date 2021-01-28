using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GetStat.Annotations;
using GetStat.Models;

namespace GetStat.Controls
{
    public partial class TimeControl : UserControl, INotifyPropertyChanged
    {
        public TimeControl()
        {
            InitializeComponent();
            Hours = new ObservableCollection<string>(Enumerable.Range(0,24).Select(x=>x.ToString("D2")));
            Minutes = new ObservableCollection<string>(Enumerable.Range(0, 60).Select(x => x.ToString("D2")));
            hour.ItemsSource = Hours;
            min.ItemsSource = Minutes;
        }



        public TimeSpan Value
        {
            get => (TimeSpan)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeControl),
                new FrameworkPropertyMetadata(DateTime.Now.TimeOfDay, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,new PropertyChangedCallback(OnTimeChanged)));

        private static void OnTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is TimeControl control)) return;

            var val = (TimeSpan)e.NewValue;
            control.hour.SelectedItem = val.Hours.ToString("D2");
            control.min.SelectedItem = val.Minutes.ToString("D2");

        }


        public ObservableCollection<string> Hours { get; }
        public ObservableCollection<string> Minutes { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void hour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = sender as ComboBox;

            Value = new TimeSpan(0, Convert.ToInt32(hour.SelectedItem), Convert.ToInt32(min.SelectedItem), 0);
        }
    }
}
