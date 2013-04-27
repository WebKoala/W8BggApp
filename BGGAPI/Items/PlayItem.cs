using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGAPI.Items
{
    public class PlayItem
    {
        public string Name { get; set; }
        public int NumPlays { get; set; }

        public int GameId { get; set; }

        public DateTime PlayDate { get; set; }
    }
}
