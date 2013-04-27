using BGGApp.Common;
using BGGApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGGApp.DataModel
{
    public static class BoardGameStorage
    {
        public static void AddGame(BoardGameDataItem Game)
        {
            StorageManager.AddGame(Game);
            // await StorageManager.Save<BoardGameDataItem>();
        }

        
        public static BoardGameDataItem LoadGame(int gameId)
        {
            return StorageManager.GetGame(gameId);
        }

        public static void RemoveGame(BoardGameDataItem Game)
        {
            StorageManager.Games.Remove(Game);
        }

        public static IEnumerable<BoardGameDataItem> CollectionGames
        {
            get
            {
                return StorageManager.Games.Where(x => x.IsCollectionItem);
            }

        }

        public static async Task Restore()
        {
            try
            {
                StorageManager.Games.Clear();
                await StorageManager.Restore<BoardGameDataItem>();
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task Persist()
        {
            await StorageManager.Save<BoardGameDataItem>();
        }

        internal static void ClearCache()
        {
            //Throw away cached data (used username is changed, this prevents duplicate Owned and private information mixing up)
            // This is quite inefficient, but username changing is very rare...

            StorageManager.ClearCache();
        }
    }
}
