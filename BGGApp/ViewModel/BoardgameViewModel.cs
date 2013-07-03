using BGGApp.DataModel;
using BGGApp.Helpers;
using BGGApp.Messaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;
using System.Linq;

namespace BGGApp.ViewModel
{
    public class BoardGameViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private SynchronizationContext SynchronizationContext;

        public BoardGameViewModel()
        {
            this.SynchronizationContext = SynchronizationContext.Current;
            this.GotoSiteCommand = new RelayCommand(GotoSite);
            this.ForceReloadCommand = new RelayCommand(ForceReload);
            this.LogPlayCommand = new RelayCommand(LogPlay);
            this.ShowPlayCommand = new RelayCommand(ShowPlay);
            this.HidePlayCommand = new RelayCommand(HidePlay);
            this.LoadAllCommentsCommand = new RelayCommand(LoadAllComments);

            ResetCurrentPlay();

            Messenger.Default.Register<UsernameChangedMessage>(this, UsernameChanged);
        }

        /// <summary>
        /// Called when the username is changed and collection and plays should be updated.
        /// </summary>
        /// <param name="username"></param>
        public void UsernameChanged(UsernameChangedMessage msg)
        {
            RaisePropertyChanged("CanChangeStatus");
        }

        public RelayCommand GotoSiteCommand { get; private set; }
        private async void GotoSite()
        {
            if (this.CurrentGame != null)
            {
                Uri Url = new Uri(this.CurrentGame.VisitURL);
                var result = await Windows.System.Launcher.LaunchUriAsync(Url);
            }
        }

        public RelayCommand ForceReloadCommand { get; private set; }
        private async void ForceReload()
        {
            if (CurrentGame != null)
            {
                IsLoading = true;
                CurrentGame = await BGGDataSource.Singleton.LoadGame(CurrentGame.GameId, true);
                IsLoading = false;
            }
        }

        public RelayCommand ShowPlayCommand { get; private set; }
        private void ShowPlay()
        {
            if (CurrentGame == null)
                return;

            ShowPlayDialog = true;

        }

        public RelayCommand HidePlayCommand { get; private set; }
        private void HidePlay()
        {
            ResetCurrentPlay();
            ShowPlayDialog = false;
        }

        public RelayCommand LoadAllCommentsCommand { get; private set; }
        private async void LoadAllComments()
        {
            if (CurrentGame != null)
            {
                IsLoading = true;
                await BGGDataSource.Singleton.LoadAllComments(CurrentGame);
                IsLoading = false;
            }
        }

        public RelayCommand LogPlayCommand { get; private set; }
        private async void LogPlay()
        {
            StatusMessage = string.Empty;

            if (CurrentGame == null)
                return;

            StatusUpdating = true;
            bool success = await BGGDataSource.Singleton.LogPlay(CurrentPlay);
            StatusUpdating = false;

            if (success)
            {
                BGGDataSource.Singleton.UserPlaysHub.Insert(0, CurrentPlay);
                if (CurrentCollectionItem != null)
                    CurrentCollectionItem.NumPlays += CurrentPlay.NumPlays;

                ResetCurrentPlay();
                ShowPlayDialog = false;
            }
            else
            {
                StatusMessage = "Error logging play, check your account settings";
            }

        }

        #region Property: "StatusMessage"
        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    RaisePropertyChanged("StatusMessage");
                }
            }
        }
        #endregion

        #region Property: "StatusUpdating"
        private bool _statusUpdating;
        public bool StatusUpdating
        {
            get { return _statusUpdating; }
            set
            {
                if (_statusUpdating != value)
                {
                    _statusUpdating = value;
                    RaisePropertyChanged("StatusUpdating");
                }
            }
        }
        #endregion

        #region Property: "CanChangeStatus"

        public bool CanChangeStatus
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AppSettings.Singleton.UserPasswordSetting);
            }

        }
        #endregion

        private BoardGameDataItem _CurrentGame;
        public BoardGameDataItem CurrentGame
        {
            get
            {
                if (IsInDesignMode)
                {
                    // Load fake data for Blend
                    return DesignData.GetGame();
                }
                return _CurrentGame;
            }
            set
            {
                _CurrentGame = value;
                RaisePropertyChanged("CurrentGame");

                // Update collection item
                BoardGameDataItem colItem = null;
                if(_CurrentGame != null)
                {
                    colItem = BGGDataSource.Singleton.UserCollection.FirstOrDefault(x => x.GameId == CurrentGame.GameId);
                }
                if (colItem == null)
                {
                    // Very well possible for example if game from hotness list.
                    colItem = new BoardGameDataItem();
                }
                CurrentCollectionItem = colItem;
            }
        }

        private BoardGameDataItem _CurrentCollectionItem;
        public BoardGameDataItem CurrentCollectionItem
        {
            get
            {
                if (IsInDesignMode)
                {
                    // Load fake data for Blend
                    return DesignData.GetCollectionItem();
                }
                return _CurrentCollectionItem;
            }
            private set
            {
                if (value != _CurrentCollectionItem)
                {
                    _CurrentCollectionItem = value;
                    RaisePropertyChanged("CurrentCollectionItem");
                }
            }
        }

        private PlayItemDataItem _CurrentPlay;
        public PlayItemDataItem CurrentPlay
        {
            get
            {
                return _CurrentPlay;
            }
            set
            {

                _CurrentPlay = value;
                RaisePropertyChanged("CurrentPlay");

            }
        }

        private void ResetCurrentPlay()
        {
            CurrentPlay = new PlayItemDataItem();
            CurrentPlay.PlayDate = DateTime.Today;
            CurrentPlay.NumPlays = 1;

            if (CurrentGame != null)
            {
                CurrentPlay.GameId = CurrentGame.GameId;
                CurrentPlay.Thumbnail = CurrentGame.Thumbnail;
                CurrentPlay.Name = CurrentGame.Name;
            }
        }

        public async void LoadGame(int gameId)
        {
            CurrentGame = null;
            CurrentGame = await BGGDataSource.Singleton.LoadGame(gameId);
            ResetCurrentPlay();
        }

        private bool _IsLoading { get; set; }
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            set
            {
                if (value != _IsLoading)
                {
                    _IsLoading = value;
                    RaisePropertyChanged("IsLoading");
                }

            }
        }

        public bool _ShowPlayDialog { get; set; }
        public bool ShowPlayDialog
        {
            get
            {
                return _ShowPlayDialog;
            }
            set
            {
                if (value != _ShowPlayDialog)
                {
                    _ShowPlayDialog = value;
                    RaisePropertyChanged("ShowPlayDialog");
                }

            }
        }

    }

}
