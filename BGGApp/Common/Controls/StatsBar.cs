using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace BGGApp.Controls
{
    public sealed class StatsBar : Control
    {
        public StatsBar()
        {
            this.DefaultStyleKey = typeof(StatsBar);
        }

        public readonly static DependencyProperty PercentageProperty = DependencyProperty.Register("Percentage", typeof(Decimal), typeof(StatsBar), new PropertyMetadata(""));

        public decimal Percentage
        {
            get { return (decimal)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        public string PercentageDisplay
        {
            get
            {
                return string.Format("{0:0}%", Percentage);
            }
        }

        public readonly static DependencyProperty TotalWidthProperty = DependencyProperty.Register("TotalWidth", typeof(Double), typeof(StatsBar), new PropertyMetadata(""));
        public Double TotalWidth
        {
            get { return (Double)GetValue(TotalWidthProperty); }
            set { SetValue(TotalWidthProperty, value); }
        }



        public Double MyWidth
        {
            get
            {
                if (Percentage == 0)
                    return 0;
                return (Double)Decimal.Divide((decimal)TotalWidth, 100 / Percentage);
            }
        }
    }
}
