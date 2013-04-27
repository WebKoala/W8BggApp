using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Runtime.Serialization;
using Windows.Storage.Streams;
using BGGApp.DataModel;

namespace BGGApp.Common
{
    public class StorageManager
    {
        private static List<BoardGameDataItem> _games = new List<BoardGameDataItem>();

        public static List<BoardGameDataItem> Games
        {
            get { return _games; }
            set { _games = value; }
        }

        private const string filename = "games.xml";

        public static readonly object _lock = new object();
        public static BoardGameDataItem GetGame(int gameid)
        {
            lock (_lock)
            {
                return Games.FirstOrDefault(x => x.GameId == gameid);
            }
        }

        public static void AddGame(BoardGameDataItem Game)
        {
            lock (_lock)
            {
                Games.Add(Game);
            }
        }

        static async public Task Save<T>()
        {
            await Windows.System.Threading.ThreadPool.RunAsync((sender) => StorageManager.SaveAsync<T>().Wait(), Windows.System.Threading.WorkItemPriority.Normal);
        }

        static async public Task Restore<T>()
        {
            await Windows.System.Threading.ThreadPool.RunAsync((sender) => StorageManager.RestoreAsync<T>().Wait(), Windows.System.Threading.WorkItemPriority.Normal);
        }

        static async private Task SaveAsync<T>()
        {
            try
            {
                StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                IRandomAccessStream sessionRandomAccess = await sessionFile.OpenAsync(FileAccessMode.ReadWrite);
                IOutputStream sessionOutputStream = sessionRandomAccess.GetOutputStreamAt(0);
                var sessionSerializer = new DataContractSerializer(typeof(List<T>), new Type[] { typeof(T) });
                sessionSerializer.WriteObject(sessionOutputStream.AsStreamForWrite(), _games);
                await sessionOutputStream.FlushAsync();
            }
            catch (Exception)
            {

            }
        }

        static async private Task RestoreAsync<T>()
        {
            try
            {
                StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                if (sessionFile == null)
                {
                    return;
                }
                IInputStream sessionInputStream = await sessionFile.OpenReadAsync();
                if (sessionInputStream.AsStreamForRead().Length == 0)
                    return;

                var sessionSerializer = new DataContractSerializer(typeof(List<T>), new Type[] { typeof(T) });
                _games = (List<BoardGameDataItem>)sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());
            }
            catch (Exception ex)
            {

            }
        }



        internal static void ClearCache()
        {
            lock (_lock)
            {
                Games.Clear();
            }
        }
    }
}
