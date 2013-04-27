using BGGApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    public class SearchResultDataItem : BindableBase
    {
        private string _name = string.Empty;
        [DataMember()]
        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        [DataMember()]
        public int GameId { get; set; }

        private string _thumbnail = string.Empty;
        [DataMember()]
        public string Thumbnail
        {
            get { return this._thumbnail; }
            set { this.SetProperty(ref this._thumbnail, value); }
        }
    }
}
