using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    public static class DesignData
    {
        public static BoardGameDataItem GetGame()
        {
            BoardGameDataItem demodata = new BoardGameDataItem()
            {
                Description = @"Werewolves of Miller's Hollow is a game that takes place in a small village which is haunted by werewolves. Each player is secretly assigned a role - Werewolf, Ordinary Townsfolk, or special character such as The Sheriff, The Hunter, the Witch, the Little Girl, The Fortune Teller and so on... There is also a Moderator player who controls the flow of the game. The game alternates between night and day phases. At night, the Werewolves secretly choose a Villager to kill. During the day, the Villager who was killed is revealed and is out of the game. The remaining Villagers (normal and special villagers alike) then deliberate and vote on a player they suspect is a Werewolf, helped (or hindered) by the clues the special characters add to the general deliberation. The chosen player is &quot;lynched&quot;, reveals his/her role and is out of the game. Werewolf is a social game that requires no equipment to play, and can accommodate almost any large group of players.&#10;&#10;The Werewolves of Miller's Hollow/les Loups-Garous de Thiercelieux/Die Werw&ouml;lfe von D&uuml;sterwald is a published version arranged by Herv&#195;&#169; Marly and Philippe des Palli&#195;&#168;res and published by Lui-m&#195;&#170;me, 2001 for 8-23 players. This has been nominated for the 2003 Spiel des Jahres award.&#10;&#10;Werewolves of Miller's Hollow is a separate game from Werewolf, and was split from that entry at the request of Asmodee.&#10;&#10;Expanded by:&#10;&#10; New Moon: The Werewolves of Miller's Hollow&#10;&#10;&#10;&#10;&#10;Re-Implemented by:&#10;&#10; Werewolves of Miller's Hollow: The Village&#10;&#10;&#10;&#10;&#10;Home Page: http://lesloupsgarous.free.fr/&#10;&#10;",
                GameId = 25821,
                YearPublished = 2012,
                Thumbnail = @"http://cf.geekdo-images.com/images/pic510856_t.jpg",
                Name = "The Werewolves of Miller's Hollow",
                MinPlayers = 8,
                MaxPlayers = 18,
                Image = @"http://cf.geekdo-images.com/images/pic510856.jpg"
            };

            demodata.Comments.Add(new CommentDataItem()
            {
                Rating = 0,
                Username = "WebKoala",
                Text = "I expected so much and instead we got an average LCG. Simple for newcomers but if you own AGoT LCG don't even bother with this one. I give this a 4 as they could have done so much more both mechanically and it is Star Wars! An average card game I just don't want to play (some neat art though)."
            });
            demodata.Comments.Add(new CommentDataItem()
            {
                Rating = 8,
                Username = "WebKoala",
                Text = "eh.."
            });

            demodata.PlayerPollResults.Add(new PlayerPollResultDataItem()
            {
                NumPlayers = 1,
                Best = 10,
                Recommended = 20,
                NotRecommended = 5,
                NumPlayersIsAndHigher = false
            });
            demodata.PlayerPollResults.Add(new PlayerPollResultDataItem()
            {
                NumPlayers = 2,
                Best = 10,
                Recommended = 20,
                NotRecommended = 5,
                NumPlayersIsAndHigher = false
            });
            demodata.PlayerPollResults.Add(new PlayerPollResultDataItem()
            {
                NumPlayers = 3,
                Best = 15,
                Recommended = 28,
                NotRecommended = 5,
                NumPlayersIsAndHigher = false
            });
            demodata.PlayerPollResults.Add(new PlayerPollResultDataItem()
            {
                NumPlayers = 4,
                Best = 10,
                Recommended = 120,
                NotRecommended = 5,
                NumPlayersIsAndHigher = false
            });
            demodata.PlayerPollResults.Add(new PlayerPollResultDataItem()
            {
                NumPlayers = 4,
                Best = 0,
                Recommended = 0,
                NotRecommended = 132,
                NumPlayersIsAndHigher = true
            });

            return demodata;
        }

        internal static BoardGameDataItem GetCollectionItem()
        {
            return new BoardGameDataItem()
            {
                NumPlays = 10,
                WantToPlay = true
            };
        }
    }
}
