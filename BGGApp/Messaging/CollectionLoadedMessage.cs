using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.Messaging
{

    public class CollectionLoadingMessage
    {
        // Quick loading is loading the collection XML from the service, full loading is iterating through these results and loading full game information game-by-game
        public bool isQuick { get; set; }
    }

    public class CollectionLoadedMessage
    {
        public bool isQuick { get; set; }
    }

    public class UsernameChangedMessage
    {
    }

   
}
