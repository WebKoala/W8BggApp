using BGGAPI.Interface;
using BGGAPI.Items;
using BGGApp.Helpers;
using BGGApp.Messaging;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    public class BGGDataSource
    {
        CancellationTokenSource cts;

        //TODO: Settings panel and persistence
        private const int HUB_PREVIEW = 40;


        private static BGGDataSource _Singleton = new BGGDataSource();
        public static BGGDataSource Singleton
        {
            get
            {
                return _Singleton;
            }
        }

        private string _Username;

        private BGGDataSource()
        {
            _Username = AppSettings.Singleton.UserNameSetting;

            LoadAll();
            Messenger.Default.Register<UsernameChangedMessage>(this, UsernameChanged);
        }

        IBGGApiClient Client = ServiceLocator.Current.GetInstance<IBGGApiClient>();

        /// <summary>
        /// Loads the collection, plays and hotness lists.
        /// </summary>
        public async void LoadAll()
        {
            await BoardGameStorage.Restore();
            LoadCollection();
            LoadPlays();
            LoadHotness();
            LoadUserInfo();
        }

        /// <summary>
        /// Called when the username is changed and collection and plays should be updated.
        /// </summary>
        /// <param name="username"></param>
        public void UsernameChanged(UsernameChangedMessage msg)
        {
            _Username = AppSettings.Singleton.UserNameSetting;
            BoardGameStorage.ClearCache();
            LoadCollection();
            LoadPlays();
            LoadUserInfo();
        }

        public async Task LoadAllComments(BoardGameDataItem item)
        {
            IEnumerable<Comment> comments = await Client.LoadAllComments(item.GameId,item.TotalComments);
            item.Comments.Clear();
            foreach (Comment comment in comments.OrderByDescending(x => x.Text.Length).Take(500))
            {
                item.Comments.Add(new CommentDataItem()
                {
                    Rating = comment.Rating,
                    Text = comment.Text,
                    Username = comment.Username
                });
            }

        }
        
        private void LoadCollectionFromCache()
        {

            foreach (BoardGameDataItem game in BoardGameStorage.CollectionGames)
            {
                UserCollection.Add(game);

            }
            UpdateCollectionForHub();
        }

        public async void LoadCollection()
        {
            UserCollection.Clear();


            if (string.IsNullOrEmpty(_Username))
            {
                return;
            }

            LoadCollectionFromCache();

            Messenger.Default.Send<CollectionLoadingMessage>(new CollectionLoadingMessage() { isQuick = true });

            IEnumerable<Boardgame> items = await Client.LoadCollection(_Username);
            foreach (Boardgame item in items)
            {
                BoardGameDataItem cdi = BoardGameStorage.LoadGame(item.GameId);
                if (cdi == null)// Add to main store
                {
                    cdi = new BoardGameDataItem();
                    cdi.IsFullyLoaded = false; // Indicate Loadgame should get individual results from API
                    BoardGameStorage.AddGame(cdi);
                }

                cdi.Name = item.Name;
                cdi.NumPlays = item.NumPlays;
                cdi.Thumbnail = item.Thumbnail;
                cdi.YearPublished = item.YearPublished;
                cdi.GameId = item.GameId;
                cdi.ForTrade = item.ForTrade;
                cdi.Owned = item.Owned;
                cdi.PreOrdered = item.PreOrdered;
                cdi.Want = item.Want;
                cdi.PreviouslyOwned = item.PreviousOwned;
                cdi.WantToBuy = item.WantToBuy;
                cdi.WantToPlay = item.WantToPlay;
                cdi.Wishlist = item.WishList;
                cdi.Rating = item.Rating;
                cdi.AverageRating = item.AverageRating;
                cdi.BGGRating = item.BGGRating;
                cdi.Image = item.Image;
                cdi.MaxPlayers = item.MaxPlayers;
                cdi.MinPlayers = item.MinPlayers;
                cdi.PlayingTime = item.PlayingTime;
                cdi.IsExpansion = item.IsExpansion;
                cdi.Rank = item.Rank;
                cdi.UserComment = item.UserComment;

                if (cdi.IsValidCollectionMember)
                {
                    cdi.IsCollectionItem = true;
                    if (UserCollection.Contains(cdi))
                        UserCollection.Remove(cdi);

                    UserCollection.Add(cdi);
                }
            }

            UpdateCollectionForHub();

            Messenger.Default.Send<CollectionLoadedMessage>(new CollectionLoadedMessage() { isQuick = true });

            LoadCollectionFully();
        }
        public async void LoadHotness()
        {
            HotnessList.Clear();

            IEnumerable<Boardgame> items = await Client.LoadHotness();
            foreach (Boardgame item in items)
            {
                HotnessList.Add(new BoardGameDataItem()
                {
                    Name = item.Name,
                    Thumbnail = item.Thumbnail,
                    YearPublished = item.YearPublished,
                    GameId = item.GameId,
                });
            }
        }
        private async void LoadPlays()
        {
            UserPlaysHub.Clear();
            UserPlays.Clear();
            if (string.IsNullOrEmpty(_Username))
                return;

            int MaxPlaysHub = 10;
            IEnumerable<PlayItem> items = await Client.LoadLastPlays(_Username);
            //int ItemsToGet = Math.Min(items.Count(), 50);
            int ItemsToGet = items.Count();
            for (int i = 0; i < ItemsToGet; i++)
            {
                PlayItemDataItem play =new PlayItemDataItem()
                {
                    Name = items.ElementAt(i).Name,
                    NumPlays = items.ElementAt(i).NumPlays,
                    GameId = items.ElementAt(i).GameId,
                    PlayDate = items.ElementAt(i).PlayDate,
                    
                };
                UserPlays.Add(play);
                if (i < MaxPlaysHub)
                    UserPlaysHub.Add(play);
            }

            try
            {
                // Separately load games through cache
                foreach (PlayItemDataItem pi in UserPlays)
                {
                    BoardGameDataItem game = await LoadGame(pi.GameId);
                    pi.Thumbnail = game.Thumbnail;
                }
            }
            catch (Exception) { }
        }
        private async void LoadUserInfo()
        {
            BGGUser bggUser = await Client.LoadUserDetails(_Username);
            BGGUserDataItem.Singleton.Avatar = bggUser.Avatar;
            BGGUserDataItem.Singleton.Username = bggUser.Username;
        }

        public async Task<bool> LogPlay(PlayItemDataItem play)
        {
            if (string.IsNullOrEmpty(_Username))
                return false;

            string password = AppSettings.Singleton.UserPasswordSetting;

            if (string.IsNullOrEmpty(_Username))
                return false;

            return await Client.LogPlay(_Username, password, play.GameId, play.PlayDate, play.NumPlays, play.Comments, play.Length);
        }

        bool _searchUpdateIsRunning = false;
        public async void Search(string query)
        {
            CancelSearch(); // cancel if running
            while (_searchUpdateIsRunning)
            {
                await Task.Delay(100);
            }
            DoSearch(query);
        }

        private async void DoSearch(string query)
        {
            _searchUpdateIsRunning = true;
            cts = new CancellationTokenSource();

            FoundResults.Clear();

            IEnumerable<SearchResult> items = await Client.Search(query);
            foreach (SearchResult sr in items)
            {
                FoundResults.Add(new SearchResultDataItem()
                {
                    Name = sr.Name,
                    GameId = sr.GameId
                });
            }

            // Separately load games through cache
            foreach (SearchResultDataItem srdi in FoundResults)
            {
                if (cts != null && cts.IsCancellationRequested)
                    break;

                BoardGameDataItem game = await LoadGame(srdi.GameId);
                if (game != null)
                {
                    srdi.Thumbnail = game.Thumbnail;
                }
            }

            cts = null;
            _searchUpdateIsRunning = false;
        }

        private void CancelSearch()
        {
            if (cts != null)
                cts.Cancel();
        }

        private ObservableCollection<BoardGameDataItem> _UserCollection = new ObservableCollection<BoardGameDataItem>();
        private ObservableCollection<BoardGameDataItem> _UserCollectionHub = new ObservableCollection<BoardGameDataItem>();
        private ObservableCollection<BoardGameDataItem> _HotnessList = new ObservableCollection<BoardGameDataItem>();
        private ObservableCollection<PlayItemDataItem> _UserPlaysHub = new ObservableCollection<PlayItemDataItem>();
        private ObservableCollection<PlayItemDataItem> _UserPlays = new ObservableCollection<PlayItemDataItem>();
        private ObservableCollection<SearchResultDataItem> _FoundResults = new ObservableCollection<SearchResultDataItem>();

        public ObservableCollection<BoardGameDataItem> UserCollection
        {
            get { return _UserCollection; }
        }

        public ObservableCollection<BoardGameDataItem> HotnessList
        {
            get { return _HotnessList; }
        }

        public ObservableCollection<BoardGameDataItem> UserCollectionHub
        {
            get { return _UserCollectionHub; }
        }
        public ObservableCollection<PlayItemDataItem> UserPlaysHub
        {
            get { return _UserPlaysHub; }
        }
        public ObservableCollection<PlayItemDataItem> UserPlays
        {
            get { return _UserPlays; }
        }
        public ObservableCollection<SearchResultDataItem> FoundResults
        {
            get { return _FoundResults; }
        }

        private void UpdateCollectionForHub()
        {
            UserCollectionHub.Clear();

            List<BoardGameDataItem> OwnedItems = UserCollection.Distinct().Where(x => x.Owned).ToList();
            int amountToTake = Math.Min(HUB_PREVIEW, OwnedItems.Count());

            Random rnd = new Random();
            if (OwnedItems.Count != 0)
            for (int i = 0; i < amountToTake; i++)
            {
                BoardGameDataItem item = OwnedItems[rnd.Next(0, OwnedItems.Count)];
                while (UserCollectionHub.Any(x => x.GameId == item.GameId))
                    item = OwnedItems[rnd.Next(0, OwnedItems.Count)];

                UserCollectionHub.Add(item);
            }
        }

        /// <summary>
        /// Loads a game, first from cache, if not found from the BGG Service
        /// </summary>
        /// <param name="GameId"></param>
        /// <param name="ForceReload"></param>
        /// <returns></returns>
        public async Task<BoardGameDataItem> LoadGame(int GameId, bool ForceReload = false)
        {
            BoardGameDataItem bgdi = BoardGameStorage.LoadGame(GameId);

            // If Force reload, the item should be removed from the storage and fetched anew.
            if (bgdi != null)
            {
                if (ForceReload)
                {
                    BoardGameStorage.RemoveGame(bgdi);
                    bgdi = null;
                }
                else if (bgdi.DataVersion < BoardGameDataItem.CurrentDataVersion)
                {
                    BoardGameStorage.RemoveGame(bgdi);
                    bgdi = null;
                }

            }

            if (bgdi == null || !bgdi.IsFullyLoaded)
            {
                Boardgame game = await Client.LoadGame(GameId);

                if (game != null)
                {
                    #region Filling values
                    if (bgdi == null)
                    {
                        bgdi = new BoardGameDataItem();
                        BoardGameStorage.AddGame(bgdi);
                    }
                    else
                    {
                        bgdi.DisableUpdate = true;
                    }
                    
                    bgdi.Description = WebUtility.HtmlDecode(game.Description);
                    bgdi.GameId = game.GameId;
                    bgdi.Image = game.Image;
                    bgdi.MaxPlayers = game.MaxPlayers;
                    bgdi.MinPlayers = game.MinPlayers;
                    bgdi.Name = game.Name;
                    bgdi.Thumbnail = game.Thumbnail;
                    bgdi.YearPublished = game.YearPublished;
                    bgdi.PlayingTime = game.PlayingTime;
                    bgdi.AverageRating = game.AverageRating;
                    bgdi.Rank = game.Rank;
                    bgdi.BGGRating = game.BGGRating;
                    bgdi.FetchDate = DateTime.Now;
                    bgdi.IsExpansion = game.IsExpansion;
                    bgdi.TotalComments = game.TotalComments;
                    bgdi.IsFullyLoaded = true;

                    foreach (string publisher in game.Publishers)
                        bgdi.Publishers.Add(publisher);
                    foreach (string designer in game.Designers)
                        bgdi.Designers.Add(designer);
                    foreach (string artist in game.Artists)
                        bgdi.Artists.Add(artist);

                    foreach (Comment comment in game.Comments.OrderByDescending(x => x.Text.Length))
                    {
                        bgdi.Comments.Add(new CommentDataItem()
                        {
                            Rating = comment.Rating,
                            Text = comment.Text,
                            Username = comment.Username
                        });
                    }

                    foreach (PlayerPollResult result in game.PlayerPollResults.OrderBy(x => x.NumPlayers + (x.NumPlayersIsAndHigher ? 1 : 0))) // add one to 4+ , making it 5 and the highest
                    {
                        bgdi.PlayerPollResults.Add(new PlayerPollResultDataItem()
                        {
                            Best = result.Best,
                            NumPlayers = result.NumPlayers,
                            NumPlayersIsAndHigher = result.NumPlayersIsAndHigher,
                            NotRecommended = result.NotRecommended,
                            Recommended = result.Recommended
                        });
                    }

                    //if (BoardGameStorage.LoadGame(GameId) == null) // a different thread could have inserted the game by now!


                    bgdi.DisableUpdate = false;
                    #endregion
                }
                else
                    return null;
            }
            return bgdi;
        }

        /// <summary>
        /// Loads full game information from the collection.
        /// </summary>
        public void LoadCollectionFully()
        {
            try
            {
                Messenger.Default.Send<CollectionLoadingMessage>(new CollectionLoadingMessage());


                Task.Factory.StartNew(
                    () =>
                    Parallel.ForEach<BoardGameDataItem>(UserCollection,
                        game =>
                        {
                            game = LoadGame(game.GameId).Result;
                        })).ContinueWith(
                        (x) => Messenger.Default.Send<CollectionLoadedMessage>(new CollectionLoadedMessage()));
            }
            catch (Exception) { }
        }

    }
}
