using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BGGApp.Controls
{
    /// <summary>
    /// From: http://www.vxcompany.info/2012/07/29/updatesourcetrigger-in-winrt/
    /// </summary>
    public class BindableTextBox : TextBox
    {
        public string BindableText
        {
            get { return (string)GetValue(BindableTextProperty); }
            set { SetValue(BindableTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindableText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindableTextProperty =
            DependencyProperty.Register("BindableText", typeof(string), typeof(BindableTextBox), new PropertyMetadata("", OnBindableTextChanged));

        private static void OnBindableTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            ((BindableTextBox)sender).OnBindableTextChanged((string)eventArgs.OldValue, (string)eventArgs.NewValue);
        }

        public BindableTextBox()
        {
            TextChanged += BindableTextBox_TextChanged;
        }

        private void OnBindableTextChanged(string oldValue, string newValue)
        {
            Text = newValue != null ? newValue : string.Empty; // null is not allowed as value!
        }

        private void BindableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindableText = Text;
        }
    }
}
