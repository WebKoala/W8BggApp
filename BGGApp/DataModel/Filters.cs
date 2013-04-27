using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel.Filters
{
    public abstract class GameFilter
    {
        public string DisplayName { get; set; }
        public int Amount { get; set; }
    }

    public class ExpansionFilter : GameFilter
    {

        public enum ExpansionStatus
        {
            All,
            BaseGame,
            Expansion
        }

        public ExpansionStatus RequestedStatus;

        public bool Matches(BoardGameDataItem game)
        {
            bool match = true;
            if (game == null)
                match = false;

            switch (RequestedStatus)
            {
                case ExpansionStatus.All:
                    match = true;
                    break;
                case ExpansionStatus.BaseGame:
                    match = game.IsExpansion == false;
                    break;
                case ExpansionStatus.Expansion:
                    match = game.IsExpansion;
                    break;
                default:
                    match = false;
                    break;
            }
            return match;
        }

        private static List<ExpansionFilter> _DefaultFilters;
        public static List<ExpansionFilter> DefaultFilters
        {
            get
            {
                if (_DefaultFilters == null)
                {
                    _DefaultFilters = new List<ExpansionFilter>();
                    _DefaultFilters.Add(new ExpansionFilter() { RequestedStatus = ExpansionStatus.All, DisplayName = "All" });
                    _DefaultFilters.Add(new ExpansionFilter() { RequestedStatus = ExpansionStatus.BaseGame, DisplayName = "Base-games" });
                    _DefaultFilters.Add(new ExpansionFilter() { RequestedStatus = ExpansionStatus.Expansion, DisplayName = "Expansions" });
                    
                }

                return _DefaultFilters;
            }

        }
    }

    public class PlayerFilter : GameFilter
    {

        public  bool Matches(BoardGameDataItem game)
        {
            bool match = true;
            if (game == null)
                match = false;
            else if (Amount == 0)
            {
                match = true; //match all;
            }
            else if (game.MinPlayers > Amount)
                match = false;
            else if (game.MaxPlayers < Amount)
                match = false;

            return match;
        }

        private static List<PlayerFilter> _DefaultFilters;
        public static List<PlayerFilter> DefaultFilters
        {
            get
            {
                if (_DefaultFilters == null)
                {
                    _DefaultFilters = new List<PlayerFilter>();
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 0, DisplayName = "All player numbers" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 1, DisplayName = "One player" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 2, DisplayName = "Two players" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 3, DisplayName = "Three players" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 4, DisplayName = "Four players" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 5, DisplayName = "Five players" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 6, DisplayName = "Six players" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 7, DisplayName = "Seven players" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 8, DisplayName = "Eight players" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 9, DisplayName = "Nine players" });
                    _DefaultFilters.Add(new PlayerFilter() { Amount = 10, DisplayName = "Ten players" });
                }

                return _DefaultFilters;
            }

        }
    }

    public class PlayTimeFilter : GameFilter
    {
        public bool Matches(BoardGameDataItem game)
        {
            bool match = true;
            if (game == null)
                match = false;
            else if (Amount == 0)
            {
                match = true; //match all;
            }
            else if (game.PlayingTime == 0)
            {
                match = false;
            }
            else if (game.PlayingTime > Amount)
                match = false;

            return match;
        }

        private static List<PlayTimeFilter> _DefaultFilters;
        public static List<PlayTimeFilter> DefaultFilters
        {
            get
            {
                if (_DefaultFilters == null)
                {
                    _DefaultFilters = new List<PlayTimeFilter>();
                    _DefaultFilters.Add(new PlayTimeFilter() { Amount = 0, DisplayName = "All playing times" });
                    _DefaultFilters.Add(new PlayTimeFilter() { Amount = 15, DisplayName = "< 15 minutes" });
                    _DefaultFilters.Add(new PlayTimeFilter() { Amount = 30, DisplayName = "< 30 minutes" });
                    _DefaultFilters.Add(new PlayTimeFilter() { Amount = 60, DisplayName = "< 1 hour" });
                    _DefaultFilters.Add(new PlayTimeFilter() { Amount = 90, DisplayName = "< 1,5 hours" });
                    _DefaultFilters.Add(new PlayTimeFilter() { Amount = 120, DisplayName = "< 2 hours" });
                    _DefaultFilters.Add(new PlayTimeFilter() { Amount = 180, DisplayName = "< 3 hours" });
                    _DefaultFilters.Add(new PlayTimeFilter() { Amount = 240, DisplayName = "< 4 hours" });
                }

                return _DefaultFilters;
            }

        }
    }

    public class StatusFilter : GameFilter
    {
        public GameCollectionStatus StatusToFilterOn;

        public bool Matches(BoardGameDataItem BoardGame)
        {
            switch (StatusToFilterOn)
            {
                case GameCollectionStatus.All:
                    return true;

                case GameCollectionStatus.Owned:
                    return BoardGame.Owned;
                    
                case GameCollectionStatus.ForTrade:
                    return BoardGame.ForTrade;
                    
                case GameCollectionStatus.PreOrdered:
                    return BoardGame.PreOrdered;
                    
                case GameCollectionStatus.PreviouslyOwned:
                    return BoardGame.PreviouslyOwned;
                    
                case GameCollectionStatus.Want:
                    return BoardGame.Want;
                    
                case GameCollectionStatus.WantToBuy:
                    return BoardGame.WantToBuy;
                
                case GameCollectionStatus.WantToPlay:
                    return BoardGame.WantToPlay;
                    
                case GameCollectionStatus.WishList:
                    return BoardGame.Wishlist;
                case GameCollectionStatus.Played:
                    return BoardGame.Played;
                case GameCollectionStatus.OwnedUnplayed:
                    return !BoardGame.Played && BoardGame.Owned;
                    
            }
            return false;
        }

        private static List<StatusFilter> _DefaultFilters;
        public static List<StatusFilter> DefaultFilters
        {
            get
            {
                if (_DefaultFilters == null)
                {
                    _DefaultFilters = new List<StatusFilter>();
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Show all", StatusToFilterOn = GameCollectionStatus.All });
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Owned", StatusToFilterOn = GameCollectionStatus.Owned });
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Played", StatusToFilterOn = GameCollectionStatus.Played});
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Owned, Unplayed", StatusToFilterOn = GameCollectionStatus.OwnedUnplayed });
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Want to play", StatusToFilterOn = GameCollectionStatus.WantToPlay });
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Want to buy", StatusToFilterOn = GameCollectionStatus.WantToBuy });
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Want", StatusToFilterOn = GameCollectionStatus.Want});
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Wishlist", StatusToFilterOn = GameCollectionStatus.WishList });
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "For Trade", StatusToFilterOn = GameCollectionStatus.ForTrade});
                    _DefaultFilters.Add(new StatusFilter() { DisplayName = "Previously owned", StatusToFilterOn = GameCollectionStatus.PreviouslyOwned });
                }

                return _DefaultFilters;
            }
        }
    }

    public class TextFilter : GameFilter
    {
        public event EventHandler FilterTextChanged;

        private string _filterText;
        public string FilterText 
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    if (FilterTextChanged != null)
                        FilterTextChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool Matches(BoardGameDataItem BoardGame)
        {
            if(string.IsNullOrEmpty(FilterText))
                return true;

            if(BoardGame == null)
                return false;
            if(string.IsNullOrEmpty(BoardGame.Name))
                return false;

            return BoardGame.Name.ToLower().Contains(FilterText.ToLower());
        }
    }

    public enum GameCollectionStatus
    {
        All,
        Owned,
        PreviouslyOwned,
        ForTrade,
        Want,
        WantToBuy,
        WantToPlay,
        WishList,
        PreOrdered,
        Played,
        OwnedUnplayed
    }

    public class BoardgameSorter
    {
        public delegate IOrderedEnumerable<BoardGameDataItem> Sorter(IEnumerable<BoardGameDataItem> collection);

        public Sorter SortFunction { get; set; }
        public string DisplayName { get; set; }

        public IOrderedEnumerable<BoardGameDataItem> Sort(IEnumerable<BoardGameDataItem> collection)
        {
            if (SortFunction != null)
            {
                return SortFunction(collection);
            }
            return collection.OrderBy(x => x.Name);
        }

        private static List<BoardgameSorter> _DefaultSorters;
        public static List<BoardgameSorter> DefaultSorters
        {
            get
            {
                if (_DefaultSorters == null)
                {
                    _DefaultSorters = new List<BoardgameSorter>();

                    _DefaultSorters.Add(new BoardgameSorter()
                    {
                        DisplayName = "Sort by name - asc",
                        SortFunction = (collection) => collection.OrderBy(x => x.Name)
                    });

                    _DefaultSorters.Add(new BoardgameSorter()
                    {
                        DisplayName = "Sort by name - desc",
                        SortFunction = (collection) => collection.OrderByDescending(x => x.Name)
                    });

                    _DefaultSorters.Add(new BoardgameSorter()
                    {
                        DisplayName = "Sort by rating - asc",
                        SortFunction = (collection) => collection.OrderBy(x => x.AverageRating)
                    });

                    _DefaultSorters.Add(new BoardgameSorter()
                    {
                        DisplayName = "Sort by rating - desc",
                        SortFunction = (collection) => collection.OrderByDescending(x => x.AverageRating)
                    });

                    _DefaultSorters.Add(new BoardgameSorter()
                    {
                        DisplayName = "Sort by rank - asc",
                        SortFunction = (collection) => collection.OrderBy(x => x.Rank + (x.Rank < 0 ? 100000 : 0))
                        // Sneaky, maybe improve later..
                    });

                    _DefaultSorters.Add(new BoardgameSorter()
                    {
                        DisplayName = "Sort by rank - desc",
                        SortFunction = (collection) => collection.OrderByDescending(x => x.Rank + (x.Rank < 0 ? 100000 : 0))
                    });

                    _DefaultSorters.Add(new BoardgameSorter()
                    {
                        DisplayName = "Sort by year - asc",
                        SortFunction = (collection) => collection.OrderBy(x => x.YearPublished)
                    });

                    _DefaultSorters.Add(new BoardgameSorter()
                    {
                        DisplayName = "Sort by year - desc",
                        SortFunction = (collection) => collection.OrderByDescending(x => x.YearPublished)
                    });

                }
                return _DefaultSorters;
            }
        }
    }
}
