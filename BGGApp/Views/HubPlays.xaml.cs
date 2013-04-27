using BGGApp.Common;
using BGGApp.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BGGApp
{
    public sealed partial class HubPlays : UserControl
    {
        public HubPlays()
        {
            this.InitializeComponent();
            //ScrollViewer annoying = PlaysListView.GetFirstDescendantOfType<ScrollViewer>();
            //if (annoying != null)
            //    annoying.IsVerticalRailEnabled = false;
        }

       
        
        public event ItemClickedEventHandler ItemClicked;
        public event EventHandler GotoPageClicked;

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var gameId = ((PlayItemDataItem)e.ClickedItem).GameId;
            if (ItemClicked != null)
                ItemClicked(this, new ItemClickedEventArgs() { GameId = gameId });
        }

        void GotoMainPageClicked(object sender, RoutedEventArgs e)
        {
            if (GotoPageClicked != null)
                GotoPageClicked(this, null);
        }

        
    }
}
