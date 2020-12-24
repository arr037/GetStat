using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GetStat.Annotations;
using GetStat.Models;

namespace GetStat.Controls
{
    public partial class TimeControl : UserControl, INotifyPropertyChanged
    {
        public TimeControl()
        {
            InitializeComponent();
            Hours = new ObservableCollection<string>
            {
                "00",
                "01",
                "02",
                "03",
                "04",
                "05",
                "06",
                "07",
                "08",
                "09",
                "10",
                "11",
                "12",
                "13",
                "14",
                "15",
                "16",
                "17",
                "18",
                "19",
                "20",
                "21",
                "22",
                "23"
            };
            Minutes = new ObservableCollection<string>
            {
                "00",
                "05",
                "10",
                "15",
                "20",
                "25",
                "30",
                "35",
                "40",
                "45",
                "50",
                "55"
            };
            hour.ItemsSource = Hours;
            min.ItemsSource = Minutes;
        }


        private void OnTimeChanged()
        {
            var control = this;
            control.Value = new TimeSpan(0, Convert.ToInt32(hour.SelectedItem), Convert.ToInt32(min.SelectedItem), 0, 0);
        }


        public TimeSpan Value
        {
            get => (TimeSpan)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeControl),
                new FrameworkPropertyMetadata(DateTime.Now.TimeOfDay, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ObservableCollection<string> Hours { get; }
        public ObservableCollection<string> Minutes { get; }

        private String _hourText = "";

        public String HourText
        {
            get { return _hourText; }

            set
            {
                if (_hourText == value)
                {
                    return;
                }

                if (Hours.FirstOrDefault(x => x == value) == null && !string.IsNullOrEmpty(value))
                {
                    //to get the Editable TextBox from the combobox
                    var textBox = hour.Template.FindName("PART_EditableTextBox", hour) as TextBox;
                    textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                    textBox.CaretIndex = textBox.Text.Length;
                    return;
                }

                _hourText = value;
                OnPropertyChanged(nameof(Hours));
                OnTimeChanged();
            }

        }


        private String _minutText = "";

        public String MinutText
        {
            get { return _minutText; }

            set
            {
                if (_minutText == value)
                {
                    return;
                }

                if (Minutes.FirstOrDefault(x => x == value) == null && !string.IsNullOrEmpty(value))
                {
                    //to get the Editable TextBox from the combobox
                    var textBox = min.Template.FindName("PART_EditableTextBox", min) as TextBox;
                    textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                    textBox.CaretIndex = textBox.Text.Length;
                    return;
                }

                _minutText = value;
                OnPropertyChanged(nameof(Minutes));
                OnTimeChanged();
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
