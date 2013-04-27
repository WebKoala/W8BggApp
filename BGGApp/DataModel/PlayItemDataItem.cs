using BGGApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    public class PlayItemDataItem : BindableBase
    {
        private string _name = string.Empty;
        [DataMember()]
        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        private string _thumbnail = string.Empty;
        [DataMember()]
        public string Thumbnail
        {
            get { return this._thumbnail; }
            set { this.SetProperty(ref this._thumbnail, value); }
        }

        private int _numplays = 0;
        [DataMember()]
        public int NumPlays
        {
            get { return this._numplays; }
            set { this.SetProperty(ref this._numplays, value); }
        }

        [DataMember()]
        public int GameId { get; set; }

        private string _comments = string.Empty;
        [DataMember()]
        public string Comments
        {
            get { return this._comments; }
            set { this.SetProperty(ref this._comments, value); }
        }

        private int _length = 0;
        [DataMember()]
        public int Length
        {
            get { return this._length; }
            set { this.SetProperty(ref this._length, value); }
        }


        private DateTime _playDate;
        [DataMember()]
        public DateTime PlayDate
        {
            get { return this._playDate; }
            set
            {
                this.SetProperty(ref this._playDate, value);
                OnPropertyChanged("PlayDateDisplay");
            }
        }

        public string PlayDateDisplay
        {
            get
            {
                if (PlayDate == DateTime.MinValue)
                    return string.Empty;

                return PlayDate.ToString("yyyy-MM-dd");
            }
        }

    }
}
