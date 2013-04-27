using BGGApp.Common;
using BGGApp.DataModel;
using BGGApp.DataModel.Filters;
using BGGApp.Helpers;
using BGGApp.Messaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace BGGApp.ViewModel
{
   
    public class CollectionViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private SynchronizationContext SynchronizationContext;

        public CollectionViewModel()
        {
            this.SynchronizationContext = SynchronizationContext.Current;
            if (this.SynchronizationContext == null)
            {
                throw new ArgumentNullException("No synchronization context was specified and no default synchronization context was found.");
            }

            Messenger.Default.Register<CollectionLoadingMessage>(this, CollectionLoading);
            Messenger.Default.Register<CollectionLoadedMessage>(this, CollectionLoaded);
            Messenger.Default.Register<UsernameChangedMessage>(this, UsernameChanged);

            CollectionIsQuickLoading = IsUsernameFilled;
            CollectionIsLoading = true;

            ReloadCollectionCommand = new RelayCommand(ReloadCollection);
            ReloadDataCommand = new RelayCommand(ReloadData);

            CurrentTextFilter.FilterTextChanged += CurrentTextFilter_FilterTextChanged;

            this.Collection.CollectionChanged += Collection_CollectionChanged;
        }

        void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("NumberOfGamesOwned");
        }

        

        private void UsernameChanged(UsernameChangedMessage msg)
        {
            RaisePropertyChanged("UsernameDisplay");
            RaisePropertyChanged("IsUsernameFilled");
        }


        private void CollectionLoading(CollectionLoadingMessage msg)
        {
            if (msg.isQuick)
            {
                this.SynchronizationContext.Post((state) => CollectionIsQuickLoading = true, msg);
            }
            else
                this.SynchronizationContext.Post((state) => CollectionIsLoading = true, msg);
        }

        private void CollectionLoaded(CollectionLoadedMessage msg)
        {
            if (msg.isQuick)
            {
                this.SynchronizationContext.Post((state) => CollectionIsQuickLoading = false, msg);
            }
            else
                this.SynchronizationContext.Post((state) => { RepopulateFilteredCollection(); CollectionIsLoading = false; }, msg);
        }


        /// <summary>
        /// Reloads all data (hotness, collection, plays)
        /// </summary>
        public RelayCommand ReloadDataCommand { get; private set; }
        private void ReloadData()
        {
            BGGDataSource.Singleton.LoadAll();
        }

        public RelayCommand ReloadCollectionCommand { get; private set; }
        private void ReloadCollection()
        {
            BGGDataSource.Singleton.LoadCollection();
        }



        public ObservableCollection<BoardGameDataItem> Collection
        {
            get
            {
                return BGGDataSource.Singleton.UserCollection;
            }
        }
        public ObservableCollection<BoardGameDataItem> CollectionForHub
        {
            get
            {
                return BGGDataSource.Singleton.UserCollectionHub;
            }
        }
        
        public ObservableCollection<PlayItemDataItem> UserPlaysHub
        {
            get
            {
                return BGGDataSource.Singleton.UserPlaysHub;
            }
        }

        public ObservableCollection<PlayItemDataItem> UserPlays
        {
            get
            {
                return BGGDataSource.Singleton.UserPlays;
            }
        }


        public ObservableCollection<BoardGameDataItem> HotnessList
        {
            get
            {
                return BGGDataSource.Singleton.HotnessList;
            }
        }
        public ObservableCollection<SearchResultDataItem> FoundResults
        {
            get
            {
                return BGGDataSource.Singleton.FoundResults;
            }
        }

        public ObservableCollection<BoardGameDataItem> _FilteredCollection = new ObservableCollection<BoardGameDataItem>();
        public ObservableCollection<BoardGameDataItem> FilteredCollection
        {
            get
            {
                return _FilteredCollection;
            }
        }


        private StatusFilter _CurrentStatusFilter { get; set; }
        public StatusFilter CurrentStatusFilter
        {
            get
            {
                if (_CurrentStatusFilter == null)
                    _CurrentStatusFilter = StatusFilters.First(x => x.StatusToFilterOn == GameCollectionStatus.Owned);
                return _CurrentStatusFilter;
            }
            set
            {
                if (_CurrentStatusFilter != value)
                {
                    _CurrentStatusFilter = value;
                    RaisePropertyChanged("CurrentStatusFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private PlayerFilter _CurrentPlayerFilter { get; set; }
        public PlayerFilter CurrentPlayerFilter
        {
            get
            {
                if (_CurrentPlayerFilter == null)
                    _CurrentPlayerFilter = PlayerFilters.First();
                return _CurrentPlayerFilter;
            }
            set
            {
                if (_CurrentPlayerFilter != value)
                {
                    _CurrentPlayerFilter = value;
                    RaisePropertyChanged("CurrentPlayerFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private PlayTimeFilter _CurrentPlayTimeFilter { get; set; }
        public PlayTimeFilter CurrentPlayTimeFilter
        {
            get
            {
                 if (_CurrentPlayTimeFilter == null)
                     _CurrentPlayTimeFilter = PlayTimeFilters.First();

                return _CurrentPlayTimeFilter;
            }
            set
            {
                if (_CurrentPlayTimeFilter != value)
                {
                    _CurrentPlayTimeFilter = value;
                    RaisePropertyChanged("CurrentPlayTimeFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private ExpansionFilter _CurrentExpansionFilter { get; set; }
        public ExpansionFilter CurrentExpansionFilter
        {
            get
            {
                if (_CurrentExpansionFilter == null)
                    _CurrentExpansionFilter = ExpansionFilters.First();

                return _CurrentExpansionFilter;
            }
            set
            {
                if (_CurrentExpansionFilter != value)
                {
                    _CurrentExpansionFilter = value;
                    RaisePropertyChanged("CurrentExpansionFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        void CurrentTextFilter_FilterTextChanged(object sender, EventArgs e)
        {
            RepopulateFilteredCollection();
        }

        private BoardgameSorter _CurrentCollectionSorter { get; set; }
        public BoardgameSorter CurrentCollectionSorter
        {
            get
            {
                if (_CurrentCollectionSorter == null)
                    CurrentCollectionSorter = CollectionSorters.First();

                return _CurrentCollectionSorter;
            }
            set
            {
                if (_CurrentCollectionSorter != value)
                {
                    _CurrentCollectionSorter = value;
                    RaisePropertyChanged("CurrentCollectionSorter");
                    RepopulateFilteredCollection();
                }
            }
        }

        private TextFilter _CurrentTextFilter = new TextFilter();
        public TextFilter CurrentTextFilter
        {
            get
            {
                if (_CurrentTextFilter == null)
                    _CurrentTextFilter = new TextFilter();

                return _CurrentTextFilter;
            }
            set
            {
                if (_CurrentTextFilter != value)
                {
                    _CurrentTextFilter = value;
                    RaisePropertyChanged("CurrentTextFilter");
                    RepopulateFilteredCollection();
                }
            }
        }

        public int NumberOfGamesOwned
        {
            get
            {
                return BGGDataSource.Singleton.UserCollection.Where(x => x.Owned).Count();
            }

        }

        public int NumberItemsDisplayed
        {
            get
            {
                return this.FilteredCollection.Count;
            }

        }

        public List<PlayerFilter> PlayerFilters
        {
            get
            {
                return PlayerFilter.DefaultFilters;
            }
        }
        public List<PlayTimeFilter> PlayTimeFilters
        {
            get
            {
                return PlayTimeFilter.DefaultFilters;
            }
        }
        public List<StatusFilter> StatusFilters
        {
            get
            {
                return StatusFilter.DefaultFilters;
            }
        }
        public List<ExpansionFilter> ExpansionFilters
        {
            get
            {
                return ExpansionFilter.DefaultFilters;
            }
        }
        public List<BoardgameSorter> CollectionSorters
        {
            get
            {
                return BoardgameSorter.DefaultSorters;
            }
        }
      
        private void RepopulateFilteredCollection()
        {
            List<BoardGameDataItem> filtered = new List<BoardGameDataItem>();
            foreach (BoardGameDataItem ci in Collection)
            {
                bool ShowMe = true;
                if (CurrentPlayerFilter != null)
                    ShowMe = CurrentPlayerFilter.Matches(ci);

                if (ShowMe && CurrentPlayTimeFilter != null)
                    ShowMe = CurrentPlayTimeFilter.Matches(ci);

                if (ShowMe && CurrentStatusFilter != null)
                    ShowMe = CurrentStatusFilter.Matches(ci);

                if (ShowMe && CurrentExpansionFilter != null)
                    ShowMe = CurrentExpansionFilter.Matches(ci);

                if (ShowMe && CurrentTextFilter != null)
                    ShowMe = CurrentTextFilter.Matches(ci);

                if(ShowMe)
                    filtered.Add(ci);
            }

            IOrderedEnumerable<BoardGameDataItem> sortedList = CurrentCollectionSorter.Sort(filtered);

            FilteredCollection.Clear();
            foreach (BoardGameDataItem item in sortedList)
                FilteredCollection.Add(item);

            RaisePropertyChanged("NumberItemsDisplayed");
        }

        private bool _CollectionIsLoading { get; set; }
        public bool CollectionIsLoading
        {
            get
            {
                return _CollectionIsLoading;
            }
            set
            {
                if (value != _CollectionIsLoading)
                {
                    _CollectionIsLoading = value;
                    RaisePropertyChanged("CollectionIsLoading");
                    RaisePropertyChanged("CanRefresh");
                }
                    
            }
        }

        private bool _CollectionIsQuickLoading { get; set; }
        public bool CollectionIsQuickLoading
        {
            get
            {
                return _CollectionIsQuickLoading;
            }
            set
            {
                if (value != _CollectionIsQuickLoading)
                {
                    _CollectionIsQuickLoading = value;
                    RaisePropertyChanged("CollectionIsQuickLoading");
                    RaisePropertyChanged("CanRefresh");
                }

            }
        }

        public bool CanRefresh
        {
            get
            {
                return !CollectionIsLoading && !CollectionIsQuickLoading;
            }
        }

        #region Property: "SearchTerm"
        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                   
                    RaisePropertyChanged("SearchTerm");
                    RaisePropertyChanged("SearchTermDisplay");
                    BGGDataSource.Singleton.Search(_searchTerm);
                }
            }
        }
        public string SearchTermDisplay
        {
            get
            {
                return '\u201c' + _searchTerm + '\u201d';
            }
        }

        #endregion

        public bool IsUsernameFilled
        {
            get
            {
                if (IsInDesignMode)
                    return false;
                // Todo: Make better implementation of vaultmanager
                return !string.IsNullOrEmpty(AppSettings.Singleton.UserNameSetting) && AppSettings.Singleton.UserNameSetting.ToLower() != "boardgamegeek";
            }
        }

        #region Property: "Username"
        
        public string UsernameDisplay
        {
            get
            {
                return AppSettings.Singleton.UserNameSetting;
            }
        }

        #endregion
    }
}
