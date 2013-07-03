using BGGAPI.Impl;
using BGGAPI.Interface;
using BGGAPI.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace BGGAPI
{
    public class BGGAPIClient : IBGGApiClient
    {

        public const string BASE_URL = "http://www.boardgamegeek.com/xmlapi2";

        public async Task<IEnumerable<Boardgame>> LoadCollection(string Username)
        {
            IEnumerable<Boardgame> baseGames = await LoadGamesFromCollection(Username,false);
            IEnumerable<Boardgame> expansions = await  LoadGamesFromCollection(Username, true);

            return baseGames.Concat(expansions);

           

        }
        
        private async Task<IEnumerable<Boardgame>> LoadGamesFromCollection(string Username, bool GetExpansions)
        {
            try
            {
               
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/collection?username={0}&stats=1&{1}",
                    Username,
                    GetExpansions ? "subtype=boardgameexpansion" : "excludesubtype=boardgameexpansion"));
                    

                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<Boardgame> baseGames = from colItem in xDoc.Descendants("item")
                                                   select new Boardgame
                                                   {
                                                       Name = GetStringValue(colItem.Element("name")),
                                                       NumPlays = GetIntValue(colItem.Element("numplays")),
                                                       YearPublished = GetIntValue(colItem.Element("yearpublished")),
                                                       Thumbnail = GetStringValue(colItem.Element("thumbnail")),
                                                       GameId = GetIntValue(colItem, "objectid"),
                                                       ForTrade = GetBoolValue(colItem.Element("status"), "fortrade"),
                                                       Owned = GetBoolValue(colItem.Element("status"), "own"),
                                                       PreviousOwned = GetBoolValue(colItem.Element("status"), "prevowned"),
                                                       Want = GetBoolValue(colItem.Element("status"), "want"),
                                                       WantToBuy = GetBoolValue(colItem.Element("status"), "wanttobuy"),
                                                       WantToPlay = GetBoolValue(colItem.Element("status"), "wanttoplay"),
                                                       WishList = GetBoolValue(colItem.Element("status"), "wishlist"),
                                                       PreOrdered = GetBoolValue(colItem.Element("status"), "preordered"),
                                                       Rating = GetDecimalValue(colItem.Element("stats").Element("rating"), "value", -1),
                                                       AverageRating = GetDecimalValue(colItem.Element("stats").Element("rating").Element("average"), "value", -1),
                                                       Image = GetStringValue(colItem.Element("thumbnail")),
                                                       MaxPlayers = GetIntValue(colItem.Element("stats"), "maxplayers"),
                                                       MinPlayers = GetIntValue(colItem.Element("stats"), "minplayers"),
                                                       PlayingTime = GetIntValue(colItem.Element("stats"), "playingtime"),
                                                       Rank = GetRanking(colItem.Element("stats").Element("rating").Element("ranks")),
                                                       IsExpansion = GetExpansions,
                                                       UserComment = GetStringValue(colItem.Element("comment"))
                                                   };
                return baseGames;

            }
            catch (Exception ex)
            {
                //ExceptionHandler(ex);
                return new List<Boardgame>();
            }
        }

        public async Task<IEnumerable<Boardgame>> LoadHotness()
        {
            try
            {
                Uri teamDataURI = new Uri(BASE_URL + "/hot?thing=boardgame");
                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<Boardgame> gameCollection = from Boardgame in xDoc.Descendants("item")
                                                             select new Boardgame
                                                             {
                                                                 Name = Boardgame.Element("name").Attribute("value").Value,
                                                                 YearPublished = Boardgame.Element("yearpublished") != null ? int.Parse(Boardgame.Element("yearpublished").Attribute("value").Value) : 0,
                                                                 Thumbnail = Boardgame.Element("thumbnail").Attribute("value").Value,
                                                                 GameId = int.Parse(Boardgame.Attribute("id").Value)
                                                             };

                return gameCollection;

            }
            catch (Exception ex)
            {
                //ExceptionHandler(ex);
                return new List<Boardgame>();
            }
        }
        public async Task<IEnumerable<PlayItem>> LoadLastPlays(string Username)
        {
            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/plays?username={0}&subtype=boardgame&excludesubtype=videogame", Username));
                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<PlayItem> gameCollection = from Boardgame in xDoc.Descendants("play")
                                                       select new PlayItem
                                                       {
                                                           Name = Boardgame.Element("item").Attribute("name").Value,
                                                           NumPlays = int.Parse(Boardgame.Attribute("quantity").Value),
                                                           GameId = int.Parse(Boardgame.Element("item").Attribute("objectid").Value),
                                                           PlayDate = safeParseDateTime(Boardgame.Attribute("date").Value)
                                                       };
                return gameCollection;

            }
            catch (Exception ex)
            {
                return new List<PlayItem>();
            }
        }
        public async Task<Boardgame> LoadGame(int GameId)
        {

            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/thing?id={0}&stats=1&comments=1", GameId));
                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<Boardgame> gameCollection = from Boardgame in xDoc.Descendants("items")
                                                        select new Boardgame
                                                        {
                                                            Name = (from p in Boardgame.Element("item").Elements("name") where p.Attribute("type").Value == "primary" select p.Attribute("value").Value).SingleOrDefault(),
                                                            GameId = int.Parse(Boardgame.Element("item").Attribute("id").Value),
                                                            Image = Boardgame.Element("item").Element("image") != null ? Boardgame.Element("item").Element("image").Value : string.Empty,
                                                            Thumbnail = Boardgame.Element("item").Element("thumbnail") != null ? Boardgame.Element("item").Element("thumbnail").Value : string.Empty,
                                                            Description = Boardgame.Element("item").Element("description").Value,
                                                            MaxPlayers = int.Parse(Boardgame.Element("item").Element("maxplayers").Attribute("value").Value),
                                                            MinPlayers = int.Parse(Boardgame.Element("item").Element("minplayers").Attribute("value").Value),
                                                            YearPublished = int.Parse(Boardgame.Element("item").Element("yearpublished").Attribute("value").Value),
                                                            PlayingTime = int.Parse(Boardgame.Element("item").Element("playingtime").Attribute("value").Value),
                                                            AverageRating = decimal.Parse(Boardgame.Element("item").Element("statistics").Element("ratings").Element("average").Attribute("value").Value),
                                                            BGGRating = decimal.Parse(Boardgame.Element("item").Element("statistics").Element("ratings").Element("bayesaverage").Attribute("value").Value),
                                                            Rank = GetRanking(Boardgame.Element("item").Element("statistics").Element("ratings").Element("ranks")),
                                                            Publishers = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamepublisher" select p.Attribute("value").Value).ToList(),
                                                            Designers = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamedesigner" select p.Attribute("value").Value).ToList(),
                                                            Artists = (from p in Boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgameartist" select p.Attribute("value").Value).ToList(),
                                                            Comments = LoadComments(Boardgame.Element("item").Element("comments")),
                                                            PlayerPollResults = LoadPlayerPollResults(Boardgame.Element("item").Element("poll")),
                                                            IsExpansion = SetIsExpansion(Boardgame),
                                                            TotalComments = int.Parse(Boardgame.Element("item").Element("comments").Attribute("totalitems").Value)

                                                        };


                return gameCollection.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IEnumerable<SearchResult>> Search(string query)
        {
            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/search?query={0}&type=boardgame", query));

                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<SearchResult> gameCollection = from Boardgame in xDoc.Descendants("item")
                                                           select new SearchResult
                                                           {
                                                               Name = GetStringValue(Boardgame.Element("name"), "value"),
                                                               GameId = GetIntValue(Boardgame, "id")
                                                           };
                return gameCollection;

            }
            catch (Exception ex)
            {
                return new List<SearchResult>();
            }
        }

        public async Task<BGGUser> LoadUserDetails(string username)
        {
            try
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/user?name={0}", username));

                XDocument xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                IEnumerable<BGGUser> users = from Boardgame in xDoc.Descendants("user")
                                                           select new BGGUser
                                                           {
                                                               Avatar = GetStringValue(Boardgame.Element("avatarlink"), "value"),
                                                               Username = username
                                                           };
                return users.FirstOrDefault();

            }
            catch (Exception ex)
            {
                return new BGGUser();
            }
        }

        public async Task<IEnumerable<Comment>> LoadAllComments(int GameId, int totalComments)
        {
            try
            {

            List<Comment> comments = new List<Comment>();
            int page = 1;
            while ((page -1) * 100 < totalComments)
            {
                Uri teamDataURI = new Uri(string.Format(BASE_URL + "/thing?id={0}&stats=1&comments=1&page={1}", GameId,page));
                XDocument xDoc = await ReadData(teamDataURI);
                XElement commentsElement = xDoc.Element("items").Element("item").Element("comments");
                var commentsRes = LoadComments(commentsElement);
                comments.AddRange(commentsRes);
                page++;
            }
            return comments;
            }
            catch(Exception)
            {
                return new List<Comment>();
            }
        }

        private bool SetIsExpansion(XElement Boardgame)
        {
            return (from p in Boardgame.Element("item").Elements("link") where 
                 p.Attribute("type").Value == "boardgamecategory" && p.Attribute("id").Value == "1042" 
             select p.Attribute("value").Value).FirstOrDefault() != null;
        }
        private string GetStringValue(XElement element, string attribute = null, string defaultValue = "")
        {
            if (element == null)
                return defaultValue;

            if (attribute == null)
                return element.Value;

            XAttribute xatt = element.Attribute(attribute);
            if (xatt == null)
                return defaultValue;

            return xatt.Value;
        }
        private int GetIntValue(XElement element, string attribute = null, int defaultValue = -1)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            int retVal;
            if (!int.TryParse(val, out retVal))
                retVal = defaultValue;
            return retVal;
        }
        private bool GetBoolValue(XElement element, string attribute = null, bool defaultValue = false)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            int retVal;
            if (!int.TryParse(val, out retVal))
                return defaultValue;

            return retVal == 1;
        }
        private decimal GetDecimalValue(XElement element, string attribute = null, decimal defaultValue = -1)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            decimal retVal;
            if (!decimal.TryParse(val, out retVal))
                return defaultValue;

            return retVal;
        }
        private List<PlayerPollResult> LoadPlayerPollResults(XElement xElement)
        {
            List<PlayerPollResult> playerPollResult = new List<PlayerPollResult>();
            if (xElement != null)
            {
                foreach (XElement results in xElement.Elements("results"))
                {
                    PlayerPollResult pResult = new PlayerPollResult()
                    {
                        Best = GetIntResultScore(results, "Best"),
                        Recommended = GetIntResultScore(results, "Recommended"),
                        NotRecommended = GetIntResultScore(results, "Not Recommended")
                    };
                    SetNumplayers(pResult, results);
                    playerPollResult.Add(pResult);
                }
            }
            return playerPollResult;
        }
        private void SetNumplayers(PlayerPollResult pResult, XElement results)
        {

            string value = results.Attribute("numplayers").Value;
            if (value.Contains("+"))
            {
                pResult.NumPlayersIsAndHigher = true;
            }
            value = value.Replace("+", string.Empty);

            int res = 0;
            int.TryParse(value, out res);

            pResult.NumPlayers = res;
        }
        private int GetIntResultScore(XElement results, string selector)
        {
            int res = 0;
            try
            {
                string value = (from p in results.Elements("result") where p.Attribute("value").Value == selector select p.Attribute("numvotes").Value).FirstOrDefault();

                if (value != null)
                    int.TryParse(value, out res);
            }
            catch (Exception)
            {
                return 0;
            }

            return res;
        }
        private int GetRanking(XElement rankingElement)
        {
            string val = (from p in rankingElement.Elements("rank") where p.Attribute("id").Value == "1" select p.Attribute("value").Value).SingleOrDefault();
            int rank;

            if (val == null)
                rank = -1;
            else if (val.ToLower().Trim() == "not ranked")
                rank = -1;
            else if (!int.TryParse(val, out rank))
                rank = -1;

            return rank;
        }
        private List<Comment> LoadComments(XElement commentsElement)
        {
            List<Comment> comments = new List<Comment>();

            if (commentsElement != null)
                foreach (XElement commentElement in commentsElement.Elements("comment"))
                {
                    Comment c = new Comment()
                    {
                        Text = commentElement.Attribute("value").Value,
                        Username = commentElement.Attribute("username").Value
                    };

                    decimal rating;
                    decimal.TryParse(commentElement.Attribute("rating").Value, out rating);
                    c.Rating = rating;

                    comments.Add(c);
                }
            return comments;
        }
        private async Task<XDocument> ReadData(Uri requestUrl)
        {
            // Due to malformed header I cannot use GetContentAsync and ReadAsStringAsync :(
            // UTF-8 is now hard-coded...
            using (var client = new HttpClient())
            {
                var responseBytes = await client.GetByteArrayAsync(requestUrl);
                var xmlResponse = Encoding.UTF8.GetString(responseBytes, 0, responseBytes.Length);

                return XDocument.Parse(xmlResponse);
            }
        }


        /// <summary>
        /// Note, if you log in succesfully with username,correctPassword and later try to do this again with username,INcorrectpassword, the BGG server will still log you in!
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="gameId"></param>
        /// <param name="date"></param>
        /// <param name="amount"></param>
        /// <param name="comments"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<bool> LogPlay(string username, string password, int gameId, DateTime date, int amount, string comments, int length)
        {
            //http://www.boardgamegeek.com/geekplay.php?objecttype=thing&objectid=104557&ajax=1&action=new
            
            CookieContainer jar = new CookieContainer();
            jar = await GetLoginCookies(username, password,jar);


            string requestBase = "dummy=1&ajax=1&action=save&version=2&objecttype=thing&objectid={0}&playid=&action=save&playdate={1}&dateinput={2}&YUIButton=&twitter=0&savetwitterpref=0&location=&quantity={3}&length={4}&incomplete=0&nowinstats=0&comments={5}";
            string request = string.Format(requestBase, gameId, date.ToString("yyyy-MM-dd"), DateTime.Today.ToString("yyyy-MM-dd"), amount, length, comments);

            byte[] byteArray = Encoding.UTF8.GetBytes(request);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://www.boardgamegeek.com/geekplay.php");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = jar;

            using (Stream webpageStream = await webRequest.GetRequestStreamAsync())
            {
                webpageStream.Write(byteArray, 0, byteArray.Length);
            }
            using (WebResponse response = await webRequest.GetResponseAsync())
            {
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    if (responseText == "You must login to save plays")
                        return false;
                }
            }
            return true;
        }

        private async Task<CookieContainer> GetLoginCookies(string username, string password, CookieContainer cookieJar)
        {
            string postData = string.Format("lasturl=&username={0}&password={1}", username, password);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://www.boardgamegeek.com/login");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.CookieContainer = cookieJar;

            
            using (Stream webpageStream = await webRequest.GetRequestStreamAsync())
            {
                webpageStream.Write(byteArray, 0, byteArray.Length);
            }
            using (WebResponse response = await webRequest.GetResponseAsync())
            {
                
            }
            return cookieJar;
            
        }

        private DateTime safeParseDateTime(string date)
        {
            DateTime dt;
            if(!DateTime.TryParseExact(date,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out dt))
            {
                dt = DateTime.MinValue;
            }
            return dt;
        }







       
    }
}
