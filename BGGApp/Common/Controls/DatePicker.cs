using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BGGApp.Controls
{
    public class SmartDateTextbox : TextBox, INotifyPropertyChanged
    {
        public readonly static DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(DateTime), typeof(SmartDateTextbox), new PropertyMetadata(""));

        public SmartDateTextbox()
            : base()
        {
            Date = DateTime.Today;
        }

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { 
                SetValue(DateProperty, value);
                NotifyPropertyChanged("DayOfWeek");
                NotifyPropertyChanged("DateIsValid");
            }
        }

        public string DayOfWeek
        {
            get
            {
                if (Date == DateTime.MinValue)
                    return string.Empty;

                return Date.ToString("dddd", new CultureInfo("en-us"));
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Text = Date.ToString("yyyy-MM-dd");
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            ValidateAndFix();
        }

        protected override void OnKeyUp(Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (Date != DateTime.MinValue)
            {
                if (e.Key == Windows.System.VirtualKey.Up || e.Key == Windows.System.VirtualKey.Right)
                {
                    Date = Date.AddDays(1);
                    this.Text = Date.ToString("yyyy-MM-dd");
                }
                else if (e.Key == Windows.System.VirtualKey.Down || e.Key == Windows.System.VirtualKey.Left)
                {
                    Date = Date.AddDays(-1);
                    this.Text = Date.ToString("yyyy-MM-dd");
                }
            }
            else
                base.OnKeyUp(e);
        }

        private void ValidateAndFix()
        {
            DateTime date = DateTime.Today;
            Text = Regex.Replace(Text, "[^0-9]", "");
            if (Regex.IsMatch(Text,"^[0-9]{4}$"))
            {
                date = Fix4Position(Text);
            }
            else if (Regex.IsMatch(Text, "^[0-9]{6}$"))
            {
                date = Fix6Position(Text);
            }
            else if (Regex.IsMatch(Text, "^[0-9]{8}$"))
            {
                date = Fix8Position(Text);
            }
            else
            {
                SetInvalid();
                return;
            }

            Date = date;
            this.Text = Date.ToString("yyyy-MM-dd");
        }

        private DateTime Fix8Position(string input)
        {
            string start = input.Substring(0, 4);
            int year = GetYear(start);
            if (year == -1)
            {
                SetInvalid();
                return DateTime.MinValue;
            }

            return Fix4Position(input.Substring(4), year);
        }

        private DateTime Fix6Position(string input)
        {
            string start = input.Substring(0, 2);
            int year = GetYear(start);
            if (year == -1)
            {
                SetInvalid();
                return DateTime.MinValue;
            }

            return Fix4Position(input.Substring(2), year);
        }

        private DateTime Fix4Position(string input, int year = -1)
        {
            if (year == -1)
                year = DateTime.Now.Year;

            string start = input.Substring(0, 2);
            
            int month = GetMonth(start);
            if (month == -1)
            {
                SetInvalid();
                return DateTime.MinValue;
            }

            string end = input.Substring(2, 2);
            int day = GetDay(end,year,month);
            if( day == -1)
            {
                SetInvalid();
                return DateTime.MinValue; 
            }

            return new DateTime(year, month, day);
        }

        private int GetDay(string day, int year, int month)
        {
            if (day.Length != 2)
                return -1;

            if (day.StartsWith("0"))
                day = day.Substring(1);

            int iDay;
            if (!int.TryParse(day, out iDay))
                return -1;

            if (iDay > DateTime.DaysInMonth(year,month))
                return -1;

            return iDay;
        }

        private int GetMonth(string month)
        {
            if(month.Length != 2)
                return -1;

            int iMonth;
            if (!int.TryParse(month, out iMonth))
                return -1;
        
            if (iMonth > 12)
                return -1;

            return iMonth;
        }

        private int GetYear(string year)
        {
            if (year.Length != 2 && year.Length != 4)
                return -1;

            int iYear;
            if (!int.TryParse(year, out iYear))
                return -1;

            if (iYear < 50)
                iYear += 2000;
            else if (iYear < 100)
                iYear += 1900;

            return iYear;
        }

        private void SetInvalid()
        {
            Date = DateTime.MinValue;
        }

        public bool DateIsValid
        {
            get
            {
                return Date != DateTime.MinValue;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // NotifyPropertyChanged will raise the PropertyChanged event passing the
        // source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }  
    }
}
