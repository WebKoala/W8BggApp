using BGGApp.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    [KnownType(typeof(BGGApp.DataModel.BoardGameDataItem))]
    [DataContractAttribute]
    public class BoardGameDataItem : BindableBase, IEquatable<BoardGameDataItem>
    {
        public const int CurrentDataVersion = 4;

        public BoardGameDataItem()
        {
            this.Publishers.CollectionChanged += Publishers_CollectionChanged;
            this.Designers.CollectionChanged += Designers_CollectionChanged;
            this.Artists.CollectionChanged += Artists_CollectionChanged;
            
        }

        private int _numplays = 0;
        [DataMember()]
        public int NumPlays
        {
            get { return this._numplays; }
            set
            {
                this.SetProperty(ref this._numplays, value);
                OnPropertyChanged("NumPlays");
                OnPropertyChanged("Played");
            }
        }

        public bool Played
        {
            get
            {
                return this.NumPlays > 0;
            }
        }

        private bool _owned = false;
        [DataMember()]
        public bool Owned
        {
            get { return this._owned; }
            set
            {
                this.SetProperty(ref this._owned, value);
                OnPropertyChanged("Owned");
            }
        }

        private bool _prevOwned = false;
        [DataMember()]
        public bool PreviouslyOwned
        {
            get { return this._prevOwned; }
            set
            {
                this.SetProperty(ref this._prevOwned, value);
                OnPropertyChanged("PreviouslyOwned");
            }
        }

        private bool _want = false;
        [DataMember()]
        public bool Want
        {
            get { return this._want; }
            set
            {
                this.SetProperty(ref this._want, value);
                OnPropertyChanged("Want");
            }
        }

        private bool _wanttoplay = false;
        [DataMember()]
        public bool WantToPlay
        {
            get { return this._wanttoplay; }
            set
            {
                this.SetProperty(ref this._wanttoplay, value);
                OnPropertyChanged("WantToPlay");
            }
        }

        private bool _wanttobuy = false;
        [DataMember()]
        public bool WantToBuy
        {
            get { return this._wanttobuy; }
            set
            {
                this.SetProperty(ref this._wanttobuy, value);
                OnPropertyChanged("WantToBuy");
            }
        }

        private bool _wishlist = false;
        [DataMember()]
        public bool Wishlist
        {
            get { return this._wishlist; }
            set
            {
                this.SetProperty(ref this._wishlist, value);
                OnPropertyChanged("Wishlist");
            }
        }

        private bool _fortrade = false;
        [DataMember()]
        public bool ForTrade
        {
            get { return this._fortrade; }
            set
            {
                this.SetProperty(ref this._fortrade, value);
                OnPropertyChanged("ForTrade");
            }
        }

        private bool _preordered = false;
        [DataMember()]
        public bool PreOrdered
        {
            get { return this._preordered; }
            set
            {
                this.SetProperty(ref this._preordered, value);
                OnPropertyChanged("PreOrdered");
            }
        }

        
        private decimal _rating = -1;
        [DataMember()]
        public decimal Rating
        {
            get { return this._rating; }
            set
            {
                this.SetProperty(ref this._rating, value);
            }
        }

        public string RatingDisplay
        {
            get
            {
                if (Rating < 0)
                    return "N/A";
                return Rating.ToString("0.00");
            }

        }


       
        /// <summary>
        /// Items that had a status (like wishlist) in the past, are still passed on by the API, but should imho be ignored.
        /// </summary>
        [IgnoreDataMember]
        public bool IsValidCollectionMember
        {
            get
            {
                if (Owned) return true;
                if (PreOrdered) return true;
                if (Played) return true;
                if (ForTrade) return true;
                if (PreviouslyOwned) return true;
                if (Want) return true;
                if (WantToBuy) return true;
                if (WantToPlay) return true;
                if (Wishlist) return true;

                return false;
            }
        }


        private string _name = string.Empty;
        [DataMember()]
        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        private string _userComment = string.Empty;
        [DataMember()]
        public string UserComment
        {
            get { return this._userComment; }
            set { this.SetProperty(ref this._userComment, value);
            OnPropertyChanged("HasComment");
            }
        }

        public bool HasComment
        {
            get
            {
                return !string.IsNullOrEmpty(UserComment);
            }
        }

        private string _thumbnail = string.Empty;
        [DataMember()]
        public string Thumbnail
        {
            get { return this._thumbnail; }
            set { this.SetProperty(ref this._thumbnail, value); }
        }

        private int _yearpublished = 0;
        [DataMember()]
        public int YearPublished
        {
            get { return this._yearpublished; }
            set { this.SetProperty(ref this._yearpublished, value); }
        }

        private string _description = string.Empty;
        [DataMember()]
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private string _image = string.Empty;
        [DataMember()]
        public string Image
        {
            get { return this._image; }
            set { this.SetProperty(ref this._image, value); }
        }

        private int _minPlayers = 0;
        [DataMember()]
        public int MinPlayers
        {
            get { return this._minPlayers; }
            set { this.SetProperty(ref this._minPlayers, value); }
        }

        private int _maxPlayers = 0;
        [DataMember()]
        public int MaxPlayers
        {
            get { return this._maxPlayers; }
            set { this.SetProperty(ref this._maxPlayers, value); }
        }

        private int _playingTime = 0;
        [DataMember()]
        public int PlayingTime
        {
            get { return this._playingTime; }
            set { this.SetProperty(ref this._playingTime, value); }
        }

        private int _totalComments = 0;
        [DataMember()]
        public int TotalComments
        {
            get { return this._totalComments; }
            set { this.SetProperty(ref this._totalComments, value); }
        }

        private int _rank = 0;
        [DataMember()]
        public int Rank
        {
            get { return this._rank; }
            set { 
                this.SetProperty(ref this._rank, value);
                OnPropertyChanged("RankDisplay");
            }
        }
        public string RankDisplay
        {
            get
            {
                if (Rank < 0)
                    return "N/A";
                return Rank.ToString();
            }
        }

        private bool _isExpansion = false;
        [DataMember()]
        public bool IsExpansion
        {
            get { return this._isExpansion; }
            set
            {
                this.SetProperty(ref this._isExpansion, value);
                OnPropertyChanged("IsExpansion");
            }
        }
       

        private decimal _averageRating = 0;
        [DataMember()]
        public decimal AverageRating
        {
            get { return this._averageRating; }
            set { this.SetProperty(ref this._averageRating, value); }
        }

        private decimal _BGGRating = 0;
        [DataMember()]
        public decimal BGGRating
        {
            get { return this._BGGRating; }
            set { this.SetProperty(ref this._BGGRating, value); }
        }
       
        public string VisitURL
        {
            get
            {
                return string.Format("http://www.boardgamegeek.com/boardgame/{0}", this.GameId);
            }
        }

        [DataMember()]
        public int GameId { get; set; }

        [DataMember()]
        public DateTime FetchDate { get; set; }

        [DataMember()]
        public int DataVersion = CurrentDataVersion;


        #region Publishers
        private ObservableCollection<string> _Publishers = new ObservableCollection<string>();

        [DataMember()]
        public ObservableCollection<string> Publishers
        {
            get { return _Publishers; }
            set { _Publishers = value; }
        }
        void Publishers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("PublishersDisplay");
        }
        public string PublishersDisplay
        {
            get
            {
                if (Publishers.Count > 4)
                {
                    return string.Join(Environment.NewLine, Publishers.Take(4)) + Environment.NewLine + "...";
                }
                return string.Join(Environment.NewLine, Publishers);
            }
        } 
        #endregion

        #region Artists
        private ObservableCollection<string> _Artists = new ObservableCollection<string>();
        [DataMember()]
        public ObservableCollection<string> Artists
        {
            get { return _Artists; }
            set { _Artists = value; }
        }
        void Artists_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("ArtistsDisplay");
        }
        public string ArtistsDisplay
        {
            get
            {
                
                if (Artists.Count > 4)
                {
                    return string.Join(Environment.NewLine, Artists.Take(4)) + Environment.NewLine + "...";
                }
                return string.Join(Environment.NewLine, Artists);
            }
        } 
        #endregion

        #region Designers
        private ObservableCollection<string> _Designers = new ObservableCollection<string>();

        [DataMember()]
        public ObservableCollection<string> Designers
        {
            get { return _Designers; }
            set { _Designers = value; }

        }
        void Designers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("DesignersDisplay");
        }

        public string DesignersDisplay
        {
            get
            {
                return string.Join(Environment.NewLine, Designers);
            }
        } 
        #endregion

        #region Comments
        private ObservableCollection<CommentDataItem> _Comments = new ObservableCollection<CommentDataItem>();
        [DataMember()]
        public ObservableCollection<CommentDataItem> Comments
        {
            get { return _Comments; }
            set { _Comments = value; }
        }
        
        #endregion

        private ObservableCollection<PlayerPollResultDataItem> _PlayerPollResults = new ObservableCollection<PlayerPollResultDataItem>();
        [DataMember()]
        public ObservableCollection<PlayerPollResultDataItem> PlayerPollResults
        {
            get { return _PlayerPollResults; }
            set { _PlayerPollResults = value; }
        }

        [DataMember()]
        public bool IsFullyLoaded { get; set; } // Indicates whether only collection information was loaded.
        
        [DataMember()]
        public bool IsCollectionItem { get; set; }

        public bool Equals(BoardGameDataItem other)
        {
            return other != null && GameId == other.GameId;
        }

       
    }
}
