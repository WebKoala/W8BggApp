using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.Common
{
    public class ItemClickedEventArgs : System.EventArgs
    {
        public int GameId { get; set; }
    }

    public delegate void ItemClickedEventHandler(object sender, ItemClickedEventArgs e);

}
