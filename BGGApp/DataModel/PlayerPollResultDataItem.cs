using BGGApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    [KnownType(typeof(BGGApp.DataModel.PlayerPollResultDataItem))]
    [DataContractAttribute]
    public class PlayerPollResultDataItem : BindableBase
    {

        private int _numPlayers = 0;
        [DataMember()]
        public int NumPlayers
        {
            get { return this._numPlayers; }
            set
            {
                this.SetProperty(ref this._numPlayers, value);
                OnPropertyChanged("NumPlayersDisplay");
            }
        }

        private bool _numPlayersIsAndHigher = false;
        [DataMember()]
        public bool NumPlayersIsAndHigher
        {
            get { return this._numPlayersIsAndHigher; }
            set
            {
                this.SetProperty(ref this._numPlayersIsAndHigher, value);
                OnPropertyChanged("NumPlayersDisplay");
            }
        }

        [IgnoreDataMember]
        public string NumPlayersDisplay
        {
            get
            {
                return string.Format("{0}{1}", NumPlayers, NumPlayersIsAndHigher ? "+" : string.Empty);
            }
        }

        private int _best = 0;
        [DataMember()]
        public int Best
        {
            get { return this._best; }
            set
            {
                this.SetProperty(ref this._best, value);
                RaiseNumbersChanged();
            }
        }

        private int _recommended = 0;
        [DataMember()]
        public int Recommended
        {
            get { return this._recommended; }
            set
            {
                this.SetProperty(ref this._recommended, value);
                RaiseNumbersChanged();
            }
        }

        private int _notRecommended = 0;
        [DataMember()]
        public int NotRecommended
        {
            get { return this._notRecommended; }
            set
            {
                this.SetProperty(ref this._notRecommended, value);
                RaiseNumbersChanged();
            }
        }

        [IgnoreDataMember]
        public int Total
        {
            get
            {
                return Best + Recommended + NotRecommended;
            }
        }

        [IgnoreDataMember]
        public decimal BestPercentage
        {
            get
            {
                if (Total == 0)
                    return 0;
                return Math.Round(((decimal) Best / (decimal) Total) * 100);
            }
        }

        [IgnoreDataMember]
        public decimal RecommendedPercentage
        {
            get
            {
                if (Total == 0)
                    return 0;
                return Math.Round(((decimal)Recommended / (decimal)Total) * 100);
            }
        }

        [IgnoreDataMember]
        public decimal NotRecommendedPercentage
        {
            get
            {
                if (Total == 0)
                    return 0;
                return Math.Round(((decimal)NotRecommended / (decimal)Total) * 100);
            }
        }

        private void RaiseNumbersChanged()
        {
            OnPropertyChanged("Total");
            OnPropertyChanged("BestPercentage");
            OnPropertyChanged("RecommendedPercentage");
            OnPropertyChanged("NotRecommendedPercentage");
        }

    }
}
