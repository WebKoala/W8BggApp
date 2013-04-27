using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGAPI.Items
{
    public class Boardgame
    {
        public string Name { get; set; }
        public int YearPublished { get; set; }

        public string Thumbnail { get; set; }
        public string Image { get; set; }
        public int NumPlays { get; set; }

        public int GameId { get; set; }

        public bool Owned { get; set; }
        public bool PreviousOwned { get; set; }
        public bool ForTrade { get; set; }
        public bool Want { get; set; }
        public bool WantToPlay { get; set; }
        public bool WantToBuy { get; set; }
        public bool WishList { get; set; }
        public bool PreOrdered { get; set; }

        public decimal Rating { get; set; }

        public string Description { get; set; }
        public string UserComment { get; set; }

        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }

        public int PlayingTime { get; set; }

        public bool IsExpansion { get; set; }

        public decimal BGGRating { get; set; }
        public decimal AverageRating { get; set; }
        public int Rank { get; set; }

        public List<string> Designers { get; set; }
        public List<string> Publishers { get; set; }
        public List<string> Artists { get; set; }

        public List<Comment> Comments { get; set; }
        public List<PlayerPollResult> PlayerPollResults { get; set; }


    }
}
