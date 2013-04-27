using BGGApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    public class BGGUserDataItem : BindableBase
    {
        //Singleton
        private BGGUserDataItem()
        {
        }

        private static BGGUserDataItem _instance = new BGGUserDataItem();
        public static BGGUserDataItem Singleton
        {
            get
            {
                return _instance;
            }
        }

        private string _avatar = string.Empty;
        [DataMember()]
        public string Avatar  
        {
            get { return this._avatar; }
            set { this.SetProperty(ref this._avatar, value); }
        }

        private string _username = string.Empty;
        [DataMember()]
        public string Username
        {
            get { return this._username; }
            set { 
                this.SetProperty(ref this._username, value);
                this.OnPropertyChanged("IsBGGAnonymous");
            }
        }

        public bool IsBGGAnonymous
        {
            get
            {
                return string.IsNullOrEmpty(Username);
            }

        }

    }

    
}
