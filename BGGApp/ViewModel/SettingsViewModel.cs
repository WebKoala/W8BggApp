using BGGApp.DataModel;
using BGGApp.Helpers;
using BGGApp.Messaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BGGApp.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private SynchronizationContext SynchronizationContext;
        private AppSettings Settings = AppSettings.Singleton;

        public SettingsViewModel()
        {
            this.SynchronizationContext = SynchronizationContext.Current;
            if (this.SynchronizationContext == null)
            {
                throw new ArgumentNullException("No synchronization context was specified and no default synchronization context was found.");
            }

            this.RegisterCommand = new RelayCommand(RegisterUser);
            //this.SendFeedbackCommand = new RelayCommand(SendFeedback);

            // Load settings
            _username = Settings.UserNameSetting;
            _password = Settings.UserPasswordSetting;

        }

        #region Property: "Username"
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged("Username");
                }
            }
        }
        #endregion
        #region Property: "Password"
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged("Password");
                }
            }
        }
        #endregion
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
        #region Property: "IsWorking"
        private bool _isWorking;
        public bool IsWorking
        {
            get { return _isWorking; }
            set
            {
                if (_isWorking != value)
                {
                    _isWorking = value;
                    RaisePropertyChanged("IsWorking");
                }
            }
        }
        #endregion

        //#region Property: "FeedbackMessage"
        //private string feedbackMessage;
        //public string FeedbackMessage
        //{
        //    get { return feedbackMessage; }
        //    set
        //    {
        //        if (feedbackMessage != value)
        //        {
        //            feedbackMessage = value;
        //            RaisePropertyChanged("FeedbackMessage");
        //        }
        //    }
        //}
        //#endregion
        //#region Property: "FeedbackEmail"
        //private string feedbackEmail;
        //public string FeedbackEmail
        //{
        //    get { return feedbackEmail; }
        //    set
        //    {
        //        if (feedbackEmail != value)
        //        {
        //            feedbackEmail = value;
        //            RaisePropertyChanged("FeedbackEmail");
        //        }
        //    }
        //}
        //#endregion

        public RelayCommand RegisterCommand { get; private set; }
        //public RelayCommand SendFeedbackCommand { get; private set; }

        #region Register User
        public bool ValidateRegister()
        {
            StatusMessage = string.Empty;
            return true;
        }
        public void RegisterUser()
        {
            if (!ValidateRegister())
                return;

            
            Settings.UserNameSetting = Username;
            Settings.UserPasswordSetting = Password;
            StatusMessage = "Username changed";

            Messenger.Default.Send<UsernameChangedMessage>(new UsernameChangedMessage());

        }
       
        #endregion

        //#region Send Feedback
        //public void SendFeedback()
        //{
        //    if (string.IsNullOrWhiteSpace(this.FeedbackMessage))
        //    {
        //        StatusMessage = "Er is geen bericht ingevoerd";
        //        return;
        //    }

        //    Func<IApiClientStatusMessage, IApiClientStatusMessage> action = (msg) =>
        //    {
        //        if (msg.StatusCode == System.Net.HttpStatusCode.OK)
        //            msg.Message = "Bericht verstuurd. Bedankt!";

        //        this.SynchronizationContext.Post(AddFeedbackCompleted, msg);
        //        return msg;
        //    };

        //    using (IApiClient client = new ApiClient())
        //    {
        //        IsWorking = true;

        //        Task<IApiClientStatusMessage> t = client.PostFeedback(FeedbackEmail, FeedbackMessage);
        //        t.ContinueWith<IApiClientStatusMessage>(antecendent => action(antecendent.Result));
        //    }
        //}

        //public void AddFeedbackCompleted(object state)
        //{
        //    UpdateUI(state);
        //    FeedbackMessage = string.Empty;
        //}
        //#endregion

        public BGGUserDataItem CurrentUser
        {
            get
            {
                return BGGUserDataItem.Singleton;
            }
        }

    }
}
