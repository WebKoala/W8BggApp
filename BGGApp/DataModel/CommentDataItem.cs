using BGGApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    [KnownType(typeof(BGGApp.DataModel.CommentDataItem))]
    [DataContractAttribute]
    public class CommentDataItem : BindableBase
    {
        private string _username = string.Empty;
        [DataMember()]
        public string Username
        {
            get { return this._username; }
            set { this.SetProperty(ref this._username, value); }
        }

        private string _text = string.Empty;
        [DataMember()]
        public string Text
        {
            get { return this._text; }
            set { this.SetProperty(ref this._text, value); }
        }

        private decimal _rating = 0;
        [DataMember()]
        public decimal Rating
        {
            get { return this._rating; }
            set { 
                this.SetProperty(ref this._rating, value);
                OnPropertyChanged("RatingDisplay");
            }
        }

        public string RatingDisplay
        {
            get
            {
                if (Rating == 0)
                    return "N/A";

                return Rating.ToString();
            }
        }
        
    }
}
