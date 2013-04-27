using BGGApp.Common;
using BGGApp.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BGGApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            
            this.lastPlaysControl.ItemClicked += lastPlaysControl_gotoGamePage;
            this.lastPlaysControl.GotoPageClicked += lastPlaysControl_GotoPageClicked;

            this.collectionControl.ItemClicked += collectionControl_itemClicked;
            this.collectionControl.GotoPageClicked += collectionControl_GotoPageClicked;
            this.collectionControl.ChangeUserClicked += collectionControl_ChangeUserClicked;
            
            this.HubHotness.ItemClicked += HubHotness_itemClicked;
            this.HubHotness.GotoPageClicked += HubHotness_GotoPageClicked;

        }

        void lastPlaysControl_GotoPageClicked(object sender, EventArgs e)
        {
            this.Frame.Navigate(typeof(PlaysPage));
        }

        void HubHotness_GotoPageClicked(object sender, EventArgs e)
        {
            this.Frame.Navigate(typeof(HotnessPage));
        }

        void collectionControl_ChangeUserClicked(object sender, EventArgs e)
        {
            SettingsPane.Show();
        }

        void HubHotness_itemClicked(object sender, Common.ItemClickedEventArgs e)
        {
            this.Frame.Navigate(typeof(BoardGame), e.GameId);
        }

        void collectionControl_GotoPageClicked(object sender, EventArgs e)
        {
            this.Frame.Navigate(typeof(CollectionPage));
        }

        void collectionControl_itemClicked(object sender, Common.ItemClickedEventArgs e)
        {
            this.Frame.Navigate(typeof(BoardGame), e.GameId);
        }

        void lastPlaysControl_gotoGamePage(object sender, Common.ItemClickedEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(BoardGame),e.GameId);
        }

        
    }
}
