using BGGAPI.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BGGAPI.Interface
{
    public interface IBGGApiClient
    {
        Task<IEnumerable<Boardgame>> LoadHotness();
        Task<IEnumerable<Boardgame>> LoadCollection(string Username);

        Task<IEnumerable<PlayItem>> LoadLastPlays(string Username);
        Task<IEnumerable<SearchResult>> Search(string query);

        Task<Boardgame> LoadGame(int GameId);

        Task<BGGUser> LoadUserDetails(string username);

        Task<bool> LogPlay(string username, string password, int gameId, DateTime date, int amount, string comments, int length);

        Task<IEnumerable<Comment>> LoadAllComments(int GameId, int totalComments);
    }
}
