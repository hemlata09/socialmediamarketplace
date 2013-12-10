using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using TweetSharp;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System.Web.Script.Serialization;


namespace twitterbatchprocess
{
    public class WorkerRole : RoleEntryPoint
    {
        Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
         CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private CloudQueue queue;
        CloudQueueClient queueClient;
        CloudTable table;
        CloudTableClient tableClient;
        CloudTable tableFSVenue;
        CloudTableClient tableClientFSVenue;
        CloudTable tableTwitterMedia;
        CloudTableClient tableClientTwitterMedia;
        CloudTable tableTwitterTweet;
        CloudTableClient tableClientTwitterTweet;
        CloudTable tableTwitterHashTag;
        CloudTableClient tableClientTwitterHashTag;
        CloudTable tableTwitterUrl;
        CloudTableClient tableClientTwitterUrl;
        CloudTable tableTwitterUserMention;
        CloudTableClient tableClientTwitterUserMention;

        CloudTable tableTwitterHistory;
        CloudTableClient tableClientTwitterHistory;
        TableBatchOperation batchOperation;
        List<TwitterStatus> tweets;
        List<FoursquareEntities> foursquareEntities = new List<FoursquareEntities>();


        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("twitterapi entry point called", "Information");

            while (true)
            {// Fetch the queue attributes.
                queue.FetchAttributes();

                // Retrieve the cached approximate message count.
                int? cachedMessageCount = queue.ApproximateMessageCount;
                if (cachedMessageCount > 0)
                {

                    foreach (CloudQueueMessage message in queue.GetMessages(30, TimeSpan.FromMinutes(5)))
                    {
                        // Process all messages in less than 5 minutes, deleting each message after processing.
                        if (message != null)
                        {
                            byte[] byteArr = message.AsBytes;
                            string locationName = Encoding.UTF8.GetString(byteArr);

                            this.StoreMetadata(locationName);
                        }
                        queue.DeleteMessage(message);
                    }
                }
                else
                {


                }


                Thread.Sleep(36000);

                Trace.WriteLine("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;
            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();
            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference("socialmediaadmintwitterqueue");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();
            // Create the table client.
            tableClient = storageAccount.CreateCloudTableClient();

            // Create the table client.
            tableClientFSVenue = storageAccount.CreateCloudTableClient();

            // Create the table client.
            tableClientTwitterMedia = storageAccount.CreateCloudTableClient();

            // Create the table client.
            tableClientTwitterTweet = storageAccount.CreateCloudTableClient();

            // Create the table client.
            tableClientTwitterHashTag = storageAccount.CreateCloudTableClient();

            // Create the table client.
            tableClientTwitterUrl = storageAccount.CreateCloudTableClient();

            // Create the table client.
            tableClientTwitterUserMention = storageAccount.CreateCloudTableClient();

            // Create the table client.
            tableClientTwitterHistory = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            table = tableClient.GetTableReference("TwitterPlaceMetaDataVer1");
            table.CreateIfNotExists();

            // Create the table if it doesn't exist.
            tableTwitterMedia = tableClientTwitterMedia.GetTableReference("TwitterMediaMetaDataVer1");
            tableTwitterMedia.CreateIfNotExists();

            // Create the table if it doesn't exist.
            tableFSVenue = tableClientFSVenue.GetTableReference("FoursquareVenuesTable");
            tableFSVenue.CreateIfNotExists();

            // Create the table if it doesn't exist.
            tableTwitterTweet = tableClientTwitterTweet.GetTableReference("TwitterTweetMetaDataVer1");
            tableTwitterTweet.CreateIfNotExists();

            // Create the table if it doesn't exist.
            tableTwitterHashTag = tableClientTwitterHashTag.GetTableReference("TwitterHashTagMetaDataVer1");
            tableTwitterHashTag.CreateIfNotExists();

            // Create the table if it doesn't exist.
            tableTwitterUrl = tableClientTwitterUrl.GetTableReference("TwitterUrlMetaDataVer1");
            tableTwitterUrl.CreateIfNotExists();

            // Create the table if it doesn't exist.
            tableTwitterUserMention = tableClientTwitterUserMention.GetTableReference("TwitterUserMentionMetaDataVer1");
            tableTwitterUserMention.CreateIfNotExists();


            // Create the table if it doesn't exist.
            tableTwitterHistory = tableClientTwitterHistory.GetTableReference("TwitterHistoryLogVer1");
            tableTwitterHistory.CreateIfNotExists();

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        // This thread procedure performs the task. 
        static void ThreadProc(Object stateInfo)
        {
            // No state object was passed to QueueUserWorkItem, so  
            // stateInfo is null.
            Console.WriteLine("Hello from the thread pool.");
            //CAAFRi4vpNigBAI0ylGsXGbF4SKsbJPZA991LlqIBWwJE9rPcwjp0elGurIfa0Mds6tgtoEAO93oJeZCoiP3WHYXRh0VESZAarhxy0EMhDZBTDDO6mCpvtTjlQX98axHjO8RPTsS4z5RrifJnoVZAqGgWZAu6Ee3xwZD
        }

        //public string GenerateAuthToken()
        //{
        //    var fb = new FacebookClient();
        //    Dictionary<string, object> parameters = new Dictionary<string, object>();
        //    parameters.Add("client_id", "160125594163512");
        //    parameters.Add("redirect_uri", "https://apps.facebook.com/my-namespace-/");
        //    parameters.Add("client_secret", "e77794adf47d3d6a7d64bf9d70af6315");
        //    parameters.Add("code", "AQCWCLaLcckA7fxKhG-7XE39apFvxkfqWeQom9-AtIuVOJEDwAjoDP9QNnbhhxNBcJvkzoC1BNAvcunawjYkJiQW1wvQ0YJFCYHZNtFsGu_ymRMzWx10X7-d8R5UU1E2H7jMOFvk1I-HICO7jU3oRTMbxtGHxpCF6W3i7Y5XmwEGmuY_o_jcpwzwzvNC_MtD_hVZJysDiAXcNb7pts6gfRrYSMU7xjb_hb2Trott6Tl_2b92vOzhJ4V5Wr3897H0uTsKQaJC2REVzwxFGpZw7zDVz8ez8Cn4P23mFg8ptLihF2LiiST8b4iDIyHO7XRJXvw#_=_");

        //    dynamic result = fb.Get("/oauth/access_token", parameters);

        //    string accessToken = result["access_token"];
        //    return accessToken;
        //}

        ///// <summary>
        ///// Renews the token.. (offline deprecation)
        ///// </summary>
        ///// <param name="existingToken">The token to renew</param>
        ///// <returns>A new token (or the same as existing)</returns>
        //public static string RenewToken(string existingToken)
        //{
        //    var fb = new FacebookClient();
        //    dynamic result = fb.Get("oauth/access_token",
        //                            new
        //                            {
        //                                client_id = "371134766331432",
        //                                client_secret = "66511d29ffbc5bd4fd436e33075843d1",
        //                                grant_type = "fb_exchange_token",
        //                                fb_exchange_token = existingToken
        //                            });

        //    return result.access_token;
        //}



        public void StoreMetadata(string screenName)
        {
            TableServiceContext tableServiceContext = tableClient.GetTableServiceContext();
            var dt = (from x in tableServiceContext.CreateQuery<TwitterEntity>("TwitterPlaceMetaDataVer1")
                      where x.PartitionKey == screenName.ToLower().ToString().Trim()

                      select x).FirstOrDefault();


            TwitterEntity twitter = null;
            batchOperation = new TableBatchOperation();
            string noDataAvailable = "No data available".Trim();
            List<String> Followers = new List<string>();
            List<TwitterStatus> tweets = new List<TwitterStatus>();
            var service = new TwitterService("bCSdjjDu6MI8zuWIEIQVBw", "MNRNu72sHqhy6Id6tdBMWtwkJMkGqa6TRNAZq9x92A");
            service.AuthenticateWith("1410007567-7ayzzNc2TdBLct8p2YXLeUqrWEbJFD9kPn1ykZp", "AOVwMmWeC2EM8lPgjhNaqTEp7gY7ODwcXFjysETQQ");
            var search = service.GetUserProfileFor(new GetUserProfileForOptions { ScreenName = screenName.Trim() });
            var d = search;

            if (dt == null)
            {
                if (d != null)
                {
                    screenName = d.ScreenName.ToString();

                    var followerList = service.ListFollowers(new ListFollowersOptions { ScreenName = screenName, Cursor = -1 });
                    if (followerList == null)
                    {
                        Thread.Sleep(3600000);
                        followerList = service.ListFollowers(new ListFollowersOptions { ScreenName = screenName, Cursor = -1 });

                    }

                    if (followerList != null)
                    {
                        foreach (var follower1 in followerList)
                        {
                            Followers.Add(follower1.ScreenName.ToString());
                        }
                    }
                    /* if (followerList != null)
                     {
                         while (followerList.NextCursor != null)
                         {

                             //followerListOfDominos = service.ListFollowersOf(user_id, followers.NextCursor);
                             followerList = service.ListFollowers(new ListFollowersOptions { ScreenName = screenName, Cursor = followerList.NextCursor });
                             if (followerList != null)
                             {
                                 foreach (var follower in followerList)
                                 {
                                     Followers.Add(follower.ScreenName.ToString());
                                 }

                                 if (followerList.NextCursor.Value == 0)
                                 {
                                     break;
                                 }
                             }
                             else
                             {
                                 break;
                             }

                         }
                     }*/


                    //var favoriteTweetsList = service.ListTweetsOnList(new ListTweetsOnListOptions { OwnerScreenName = screenName });


                    //var favoriteTweetsListOnUser = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions { ScreenName = screenName, Count = 200, SinceId = 201 });


                    // Create a new Instagram entity.
                    Guid guid = Guid.NewGuid();
                    twitter = new TwitterEntity(guid, screenName.ToLower().Trim());
                    if (screenName != null)
                        twitter.TwitterScreenName = screenName.Trim();
                    else
                        twitter.TwitterScreenName = noDataAvailable;


                    if (d.Id.ToString() != null)
                        twitter.TwitterId = d.Id.ToString().Trim();
                    else
                        twitter.TwitterId = noDataAvailable;



                    if (d.Name.ToString() != null)
                        twitter.TwitterName = d.Name.ToString().Trim();
                    else
                        twitter.TwitterId = noDataAvailable;

                    if (d.Description != null)
                        twitter.TwitterDescription = d.Description.Trim();
                    else
                        twitter.TwitterDescription = noDataAvailable;

                    if (d.FollowersCount != null)
                        twitter.TwitterFollowerCount = d.FollowersCount.ToString().Trim();
                    else
                        twitter.TwitterFollowerCount = noDataAvailable;

                    if (d.FriendsCount != null)
                        twitter.TwitterFriendsCount = d.FriendsCount.ToString().Trim();
                    else
                        twitter.TwitterFriendsCount = noDataAvailable;

                    if (d.FavouritesCount != null)
                        twitter.TwitterFavouritesCount = d.FavouritesCount.ToString().Trim();
                    else
                        twitter.TwitterFavouritesCount = noDataAvailable;


                    if (d.ListedCount != null)
                        twitter.TwitterListedCount = d.ListedCount.ToString().Trim();
                    else
                        twitter.TwitterListedCount = noDataAvailable;

                    if (d.Location != null)
                        twitter.TwitterLocation = d.Location.ToString().Trim();
                    else
                        twitter.TwitterLocation = noDataAvailable;

                    if (d.Status != null)
                        twitter.TwitterStatus = d.Status.Text.Trim();
                    else
                        twitter.TwitterStatus = noDataAvailable;

                    if (d.StatusesCount != null)
                        twitter.TwitterStatusCount = d.StatusesCount.ToString().Trim();
                    else
                        twitter.TwitterStatusCount = noDataAvailable;

                    if (d.TimeZone != null)
                        twitter.TwitterTimeZone = d.TimeZone.ToString().Trim();
                    else
                        twitter.TwitterTimeZone = noDataAvailable;

                    if (d.Url != null)
                        twitter.TwitterUrl = d.Url.ToString().Trim();
                    else
                        twitter.TwitterUrl = noDataAvailable;

                    if (d.ProfileImageUrl != null)
                        twitter.TwitterProfileImageUrl = d.ProfileImageUrl.ToString().Trim();
                    else
                        twitter.TwitterProfileImageUrl = noDataAvailable;

                    if (Followers != null)
                    {

                        foreach (var followerName in Followers)
                        {
                            if (Followers.Count() > 1)
                            {
                                twitter.FollowerList += followerName.ToString().Trim() + "," + " ";
                            }
                            else
                            {
                                twitter.FollowerList = followerName.ToString().Trim();
                            }
                        }
                    }
                    else
                        twitter.FollowerList = noDataAvailable;

                    /*if (favoriteTweetsList.Count() != 0)
                    {
                        foreach (var favoriteTweetsListData in favoriteTweetsList)
                        {
                            twitter.favouriteTweetComments += favoriteTweetsListData.Text.ToString().Trim() + "," + " ";
                        }
                    }
                    else
                        twitter.favouriteTweetComments = noDataAvailable;

                    if (favoriteTweetsListOnUser.Count() != 0)
                    {
                        foreach (var favoriteTweetsListDataOnUser in favoriteTweetsListOnUser)
                        {
                            twitter.favouriteTweetListOnRestaurant += favoriteTweetsListDataOnUser.Text.ToString().Trim() + "," + " ";
                        }
                    }
                    else
                        twitter.favouriteTweetListOnRestaurant = noDataAvailable;*/

                    batchOperation.Insert(twitter);
                    table.ExecuteBatch(batchOperation);
                }
            }
            batchOperation = null;

            // string socialmediatype = DateTimeOffset.UtcNow.UtcDateTime.ToLongDateString().Trim() + "_" + DateTimeOffset.UtcNow.Hour.ToString().Trim() + ":00:00_" + "FoursqaurePhotoMetadata";
            //LogHistory(socialmediatype.Trim(), fourSquareId, "Not available", Convert.ToString(increamentalCount), newPhotoLinks, newPhotoIds, "null", "null", "null", Convert.ToString(PhotoIds.Count), "null", DateTime.UtcNow.ToString());
            //Store Tweets of the restaurant
            tweets = new List<TwitterStatus>();
            tweets = this.StoreTweetMetadata(d.ScreenName, d.Id.ToString(), d.Name.ToString().Trim(), d.StatusesCount);

            //Store media of the restaurant
            this.StoreMediaMetadata(tweets, d.ScreenName, d.Id.ToString(), d.Name.ToString().Trim(), d.StatusesCount);

            //Store HashTag of the restaurant
            this.StoreHashTagsMetadata(tweets, d.ScreenName, d.Id.ToString(), d.StatusesCount);

            //Store Urls of the restaurant
            this.StoreUrlsMetadata(tweets, d.ScreenName, d.Id.ToString(), d.StatusesCount);

            //Store User mentions of the restaurant
            this.StoreUsermentionsMetadata(tweets, d.ScreenName, d.Id.ToString(), d.StatusesCount);

        }

        /// <summary>
        /// Storing Tweet Metadata of particular Restaurant
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="twitterId"></param>
        /// <param name="statusCount"></param>
        public List<TwitterStatus> StoreTweetMetadata(string screenName, string twitterId, string placeName, long statusCount)
        {
            //TwitterMediaEntities twitterMedia = null;
            TwitterTweetEntities twitterTweet = null;
            List<TwitterTweetEntities> TweetEntities = new List<TwitterTweetEntities>();
            List<String> TweetIds = new List<String>();
            bool hasTweetIds = false;
            int increamentalCount = 0;

            String newTweetIds = null;
            batchOperation = new TableBatchOperation();
            string noDataAvailable = "No data available".Trim();
            string noMediaAvailable = "No media available".Trim();
            List<String> Followers = new List<string>();
            string tickcount = null;
            tweets = new List<TwitterStatus>();
            var service = new TwitterService("bCSdjjDu6MI8zuWIEIQVBw", "MNRNu72sHqhy6Id6tdBMWtwkJMkGqa6TRNAZq9x92A");
            service.AuthenticateWith("1410007567-7ayzzNc2TdBLct8p2YXLeUqrWEbJFD9kPn1ykZp", "AOVwMmWeC2EM8lPgjhNaqTEp7gY7ODwcXFjysETQQ");


            // var tweetsrl = db.GetCollection<TwitterStatus>("tweetSerialized");
            //RawDataSource rwdt = new RawDataSource();
            //TweetEntities = rwdt.GetTweetEntities(screenName);



            long max_id = 0;
            IEnumerable<TwitterStatus> favoriteTweetsListOnUser1;
            for (int i = 0; i < statusCount; i += 200)
            {
                if (max_id == 0)
                {
                    favoriteTweetsListOnUser1 = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions { ScreenName = screenName, Count = 200 });
                }
                else
                {
                    favoriteTweetsListOnUser1 = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions { ScreenName = screenName, Count = 200, MaxId = max_id });
                }

                if (favoriteTweetsListOnUser1 == null)
                {
                    Thread.Sleep(3600000);
                    favoriteTweetsListOnUser1 = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions { ScreenName = screenName, Count = 200, MaxId = max_id });
                }

                foreach (var twt in favoriteTweetsListOnUser1)
                {
                    tweets.Add(twt);
                    TwitterStatus t = tweets.LastOrDefault();
                    max_id = t.Id;
                }
                //if (tweets.Count == statusCount)
                //break;
            }

            if (tweets.Count() > 0)
            {
                for (int i = 0; i < tweets.Count(); i++)
                { // Create a new Instagram entity.
                    TwitterStatus tweetdata = tweets[i];
                    var serializer = new JavaScriptSerializer();
                    var dictionary = (IDictionary<string, object>)serializer.DeserializeObject(tweetdata.RawSource);
                    var dictionaryResponse = (IDictionary<string, object>)dictionary["entities"];
                    DateTime tweetCreateddt = new DateTime();
                    if (dictionary.ContainsKey("created_at") != false)
                    {
                        string[] arrTimeStamp = Convert.ToString(dictionary["created_at"]).Trim().Split(' ');

                        tweetCreateddt = this.GetDate(arrTimeStamp, out tickcount);


                    }

                    TableServiceContext tableServiceContext = tableClient.GetTableServiceContext();
                    TweetEntities = (from x in tableServiceContext.CreateQuery<TwitterTweetEntities>("TwitterTweetMetaDataVer1")
                                     where x.PartitionKey == screenName.ToLower().ToString().Trim() && x.TweetCreatedDate == tweetCreateddt.ToShortDateString().Trim()

                                     select x).ToList();

                    TweetIds = (from g in TweetEntities
                                select g).AsEnumerable().Select(t => t.TweetId).ToList();

                    twitterTweet = new TwitterTweetEntities(DateTimeOffset.UtcNow.Ticks.ToString().Trim(), screenName.ToLower().Trim());
                    batchOperation = new TableBatchOperation();

                    if (TweetIds.Count != 0)
                        hasTweetIds = TweetIds.Contains(Convert.ToString(dictionary["id"]).Trim());

                    if (hasTweetIds == false)
                    {

                        twitterTweet.TweetCreatedTimestamp = Convert.ToString(dictionary["created_at"]).Trim();
                        twitterTweet.TweetCreatedTimestampTicks = tickcount.Trim();
                        twitterTweet.TweetCreatedDate = tweetCreateddt.ToShortDateString().Trim();
                        twitterTweet.TweetId = Convert.ToString(dictionary["id"]).Trim();
                        increamentalCount = increamentalCount + 1;
                        newTweetIds = twitterTweet.TweetId + ";";

                        if (dictionaryResponse.ContainsKey("symbols") != false)
                        {
                            var symbols = (System.Object)dictionaryResponse["symbols"];
                            object[] symbolsList = (object[])symbols;
                        }
                        else
                        {
                        }

                        twitterTweet.TweetText = Convert.ToString(dictionary["text"]).Trim();
                        twitterTweet.TweetSource = Convert.ToString(dictionary["source"]).Trim();
                        if (dictionaryResponse.ContainsKey("hashtags") != false)
                        {
                            var hashTag = (System.Object)dictionaryResponse["hashtags"];
                            object[] hashTagList = (object[])hashTag;
                            twitterTweet.TweetEntityHashTagCount = Convert.ToString(hashTagList.Count()).Trim();
                        }
                        else
                        {
                            twitterTweet.TweetEntityHashTagCount = noDataAvailable;
                        }

                        if (dictionaryResponse.ContainsKey("media") != false)
                        {
                            var media = (System.Object)dictionaryResponse["media"];
                            object[] mediaList = (object[])media;
                            twitterTweet.TweetEntityMediaCount = Convert.ToString(mediaList.Count()).Trim();
                        }
                        else { twitterTweet.TweetEntityMediaCount = noDataAvailable; }

                        if (dictionaryResponse.ContainsKey("urls") != false)
                        {
                            var urls = (System.Object)dictionaryResponse["urls"];
                            object[] urlsList = (object[])urls;
                            twitterTweet.TweetEntityUrlCount = Convert.ToString(urlsList.Count()).Trim();
                        }
                        else
                        {
                            twitterTweet.TweetEntityUrlCount = noDataAvailable;
                        }

                        if (dictionaryResponse.ContainsKey("user_mentions") != false)
                        {
                            var userMention = (System.Object)dictionaryResponse["user_mentions"];
                            object[] userMentionList = (object[])userMention;
                            twitterTweet.TweetEntityUserMentionCount = Convert.ToString(userMentionList.Count()).Trim();
                        }
                        else
                        {
                            twitterTweet.TweetEntityUserMentionCount = noDataAvailable;
                        }
                        twitterTweet.TweetRetweetCount = Convert.ToString(dictionary["retweet_count"]).Trim();

                        twitterTweet.TweetEntityDetailUrl = "https://twitter.com/" + screenName + "/status/" + twitterTweet.TweetId.Trim();




                        batchOperation.Insert(twitterTweet);
                        if (twitterTweet != null)
                        {
                            tableTwitterTweet.ExecuteBatch(batchOperation);

                            batchOperation = null;
                            string tickCount = Convert.ToString(DateTimeOffset.UtcNow.UtcTicks);
                            LogTweetHistory(screenName.ToLower().Trim(), "Notavailable", tickCount, "Tweets", twitterId, placeName, Convert.ToString(increamentalCount), newTweetIds, Convert.ToString(TweetIds.Count), DateTime.UtcNow.ToString());
                        }
                    }

                }
            }



            return tweets;

        }


        /// <summary>
        /// Storing Twitter Media Metadata
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="twitterId"></param>
        /// <param name="statusCount"></param>
        public void StoreMediaMetadata(List<TwitterStatus> tweets, string screenName, string twitterId, string placeName, long statusCount)
        {
            TwitterMediaEntities twitterMedia = null;
            batchOperation = new TableBatchOperation();
            string noDataAvailable = "No data available".Trim();
            string noMediaAvailable = "No media available".Trim();


            if (tweets.Count() > 0)
            {
                for (int i = 0; i < tweets.Count(); i++)
                {
                    TwitterStatus tweetdata = tweets[i];
                    var serializer = new JavaScriptSerializer();
                    var dictionary = (IDictionary<string, object>)serializer.DeserializeObject(tweetdata.RawSource);
                    var dictionaryResponse = (IDictionary<string, object>)dictionary["entities"];

                    if (dictionaryResponse.ContainsKey("media") != false)
                    {

                        var groups = (System.Object)dictionaryResponse["media"];

                        object[] groupList = (object[])groups;
                        for (int idx = 0; idx < groupList.Length; idx++)
                        {
                            // Create a new Instagram entity.
                            Guid guid = Guid.NewGuid();
                            twitterMedia = new TwitterMediaEntities(guid, screenName.ToLower().Trim());
                            batchOperation = new TableBatchOperation();
                            if (dictionaryResponse.ContainsKey("symbols") != false)
                            {
                                var symbols = (System.Object)dictionaryResponse["symbols"];
                                object[] symbolsList = (object[])symbols;
                            }
                            else
                            {
                            }

                            twitterMedia.TweetId = Convert.ToString(dictionary["id"]).Trim();
                            twitterMedia.TweetCreatedTimestamp = Convert.ToString(dictionary["created_at"]).Trim();
                            twitterMedia.TweetText = Convert.ToString(dictionary["text"]).Trim();
                            twitterMedia.TweetSource = Convert.ToString(dictionary["source"]).Trim();
                            if (dictionaryResponse.ContainsKey("hashtags") != false)
                            {
                                var hashTag = (System.Object)dictionaryResponse["hashtags"];
                                object[] hashTagList = (object[])hashTag;
                                twitterMedia.TweetEntityHashTagCount = Convert.ToString(hashTagList.Count()).Trim();
                            }
                            else
                            {
                                twitterMedia.TweetEntityHashTagCount = noDataAvailable;
                            }

                            if (dictionaryResponse.ContainsKey("media") != false)
                            {
                                var media = (System.Object)dictionaryResponse["media"];
                                object[] mediaList = (object[])media;
                                twitterMedia.TweetEntityMediaCount = Convert.ToString(mediaList.Count()).Trim();
                            }
                            else { twitterMedia.TweetEntityMediaCount = noDataAvailable; }

                            if (dictionaryResponse.ContainsKey("urls") != false)
                            {
                                var urls = (System.Object)dictionaryResponse["urls"];
                                object[] urlsList = (object[])urls;
                                twitterMedia.TweetEntityUrlCount = Convert.ToString(urlsList.Count()).Trim();
                            }
                            else
                            {
                                twitterMedia.TweetEntityUrlCount = noDataAvailable;
                            }

                            if (dictionaryResponse.ContainsKey("user_mentions") != false)
                            {
                                var userMention = (System.Object)dictionaryResponse["user_mentions"];
                                object[] userMentionList = (object[])userMention;
                                twitterMedia.TweetEntityUserMentionCount = Convert.ToString(userMentionList.Count()).Trim();
                            }
                            else
                            {
                                twitterMedia.TweetEntityUserMentionCount = noDataAvailable;
                            }
                            twitterMedia.TweetRetweetCount = Convert.ToString(dictionary["retweet_count"]).Trim();
                            if (twitterId != null)
                                twitterMedia.TwitterId = twitterId.Trim();
                            else
                                twitterMedia.TwitterId = noDataAvailable;
                            var data = (IDictionary<string, object>)groupList[idx];
                            twitterMedia.TweetMedia_URL_HTTP = Convert.ToString(data["media_url"]).Trim();
                            twitterMedia.TweetMedia_URL_HTTPS = Convert.ToString(data["media_url_https"]).Trim();
                            twitterMedia.TweetMedia_Display_URL = Convert.ToString(data["display_url"]).Trim();
                            twitterMedia.TweetMedia_Expanded_URL = Convert.ToString(data["expanded_url"]).Trim();
                            twitterMedia.TweetMedia_Type = Convert.ToString(data["type"]).Trim();
                            twitterMedia.TweetMedia_ThumbnailSize = "150 X 150".Trim();

                        }
                        batchOperation.InsertOrMerge(twitterMedia);
                        if (twitterMedia != null)
                        {
                            tableTwitterMedia.ExecuteBatch(batchOperation);
                            batchOperation = null;

                        }
                    }

                }
            }



        }


        /// <summary>
        /// Storing Media Metadata
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="twitterId"></param>
        /// <param name="statusCount"></param>
        public void StoreHashTagsMetadata(List<TwitterStatus> tweets, string screenName, string twitterId, long statusCount)
        {
            TwitterHashTagEntities twitterHashTag = null;
            batchOperation = new TableBatchOperation();
            string noDataAvailable = "No data available".Trim();
            string noMediaAvailable = "No media available".Trim();


            if (tweets.Count() > 0)
            {
                for (int i = 0; i < tweets.Count(); i++)
                {
                    TwitterStatus tweetdata = tweets[i];
                    var serializer = new JavaScriptSerializer();
                    var dictionary = (IDictionary<string, object>)serializer.DeserializeObject(tweetdata.RawSource);
                    var dictionaryResponse = (IDictionary<string, object>)dictionary["entities"];

                    if (dictionaryResponse.ContainsKey("hashtags") != false)
                    {
                        // Create a new Instagram entity.
                        Guid guid = Guid.NewGuid();
                        twitterHashTag = new TwitterHashTagEntities(guid, screenName.ToLower().Trim());
                        batchOperation = new TableBatchOperation();
                        if (dictionaryResponse.ContainsKey("symbols") != false)
                        {
                            var symbols = (System.Object)dictionaryResponse["symbols"];
                            object[] symbolsList = (object[])symbols;
                        }
                        else
                        {
                        }

                        twitterHashTag.TweetId = Convert.ToString(dictionary["id"]).Trim();
                        twitterHashTag.TweetCreatedTimestamp = Convert.ToString(dictionary["created_at"]).Trim();
                        twitterHashTag.TweetText = Convert.ToString(dictionary["text"]).Trim();
                        twitterHashTag.TweetSource = Convert.ToString(dictionary["source"]).Trim();
                        if (dictionaryResponse.ContainsKey("hashtags") != false)
                        {
                            var hashTag = (System.Object)dictionaryResponse["hashtags"];
                            object[] hashTagList = (object[])hashTag;
                            twitterHashTag.TweetEntityHashTagCount = Convert.ToString(hashTagList.Count()).Trim();
                        }
                        else
                        {
                            twitterHashTag.TweetEntityHashTagCount = noDataAvailable;
                        }

                        if (dictionaryResponse.ContainsKey("media") != false)
                        {
                            var media = (System.Object)dictionaryResponse["media"];
                            object[] mediaList = (object[])media;
                            twitterHashTag.TweetEntityMediaCount = Convert.ToString(mediaList.Count()).Trim();
                        }
                        else { twitterHashTag.TweetEntityMediaCount = noDataAvailable; }

                        if (dictionaryResponse.ContainsKey("urls") != false)
                        {
                            var urls = (System.Object)dictionaryResponse["urls"];
                            object[] urlsList = (object[])urls;
                            twitterHashTag.TweetEntityUrlCount = Convert.ToString(urlsList.Count()).Trim();
                        }
                        else
                        {
                            twitterHashTag.TweetEntityUrlCount = noDataAvailable;
                        }

                        if (dictionaryResponse.ContainsKey("user_mentions") != false)
                        {
                            var userMention = (System.Object)dictionaryResponse["user_mentions"];
                            object[] userMentionList = (object[])userMention;
                            twitterHashTag.TweetEntityUserMentionCount = Convert.ToString(userMentionList.Count()).Trim();
                        }
                        else
                        {
                            twitterHashTag.TweetEntityUserMentionCount = noDataAvailable;
                        }
                        twitterHashTag.TweetRetweetCount = Convert.ToString(dictionary["retweet_count"]).Trim();
                        if (twitterId != null)
                            twitterHashTag.TwitterId = twitterId.Trim();
                        else
                            twitterHashTag.TwitterId = noDataAvailable;
                        var hashTags = (System.Object)dictionaryResponse["hashtags"];

                        object[] hashTagsList = (object[])hashTags;
                        for (int idx = 0; idx < hashTagsList.Length; idx++)
                        {

                            var data = (IDictionary<string, object>)hashTagsList[idx];
                            twitterHashTag.TweetHashTagText = "#" + Convert.ToString(data["text"]).Trim();


                        }
                        batchOperation.InsertOrMerge(twitterHashTag);
                        if (twitterHashTag != null)
                        {
                            tableTwitterHashTag.ExecuteBatch(batchOperation);
                            batchOperation = null;

                        }
                    }

                }
            }



        }


        /// <summary>
        /// Storing Media Metadata
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="twitterId"></param>
        /// <param name="statusCount"></param>
        public void StoreUrlsMetadata(List<TwitterStatus> tweets, string screenName, string twitterId, long statusCount)
        {
            TwitterUrlEntities twitterUrl = null;
            batchOperation = new TableBatchOperation();
            string noDataAvailable = "No data available".Trim();
            string noMediaAvailable = "No media available".Trim();


            if (tweets.Count() > 0)
            {
                for (int i = 0; i < tweets.Count(); i++)
                {
                    TwitterStatus tweetdata = tweets[i];
                    var serializer = new JavaScriptSerializer();
                    var dictionary = (IDictionary<string, object>)serializer.DeserializeObject(tweetdata.RawSource);
                    var dictionaryResponse = (IDictionary<string, object>)dictionary["entities"];

                    if (dictionaryResponse.ContainsKey("urls") != false)
                    {
                        // Create a new Instagram entity.
                        Guid guid = Guid.NewGuid();
                        twitterUrl = new TwitterUrlEntities(guid, screenName.ToLower().Trim());
                        batchOperation = new TableBatchOperation();
                        if (dictionaryResponse.ContainsKey("symbols") != false)
                        {
                            var symbols = (System.Object)dictionaryResponse["symbols"];
                            object[] symbolsList = (object[])symbols;
                        }
                        else
                        {
                        }

                        twitterUrl.TweetId = Convert.ToString(dictionary["id"]).Trim();
                        twitterUrl.TweetCreatedTimestamp = Convert.ToString(dictionary["created_at"]).Trim();
                        twitterUrl.TweetText = Convert.ToString(dictionary["text"]).Trim();
                        twitterUrl.TweetSource = Convert.ToString(dictionary["source"]).Trim();
                        if (dictionaryResponse.ContainsKey("hashtags") != false)
                        {
                            var hashTag = (System.Object)dictionaryResponse["hashtags"];
                            object[] hashTagList = (object[])hashTag;
                            twitterUrl.TweetEntityHashTagCount = Convert.ToString(hashTagList.Count()).Trim();
                        }
                        else
                        {
                            twitterUrl.TweetEntityHashTagCount = noDataAvailable;
                        }

                        if (dictionaryResponse.ContainsKey("media") != false)
                        {
                            var media = (System.Object)dictionaryResponse["media"];
                            object[] mediaList = (object[])media;
                            twitterUrl.TweetEntityMediaCount = Convert.ToString(mediaList.Count()).Trim();
                        }
                        else { twitterUrl.TweetEntityMediaCount = noDataAvailable; }

                        if (dictionaryResponse.ContainsKey("urls") != false)
                        {
                            var urls = (System.Object)dictionaryResponse["urls"];
                            object[] urlsList = (object[])urls;
                            twitterUrl.TweetEntityUrlCount = Convert.ToString(urlsList.Count()).Trim();
                        }
                        else
                        {
                            twitterUrl.TweetEntityUrlCount = noDataAvailable;
                        }

                        if (dictionaryResponse.ContainsKey("user_mentions") != false)
                        {
                            var userMention = (System.Object)dictionaryResponse["user_mentions"];
                            object[] userMentionList = (object[])userMention;
                            twitterUrl.TweetEntityUserMentionCount = Convert.ToString(userMentionList.Count()).Trim();
                        }
                        else
                        {
                            twitterUrl.TweetEntityUserMentionCount = noDataAvailable;
                        }
                        twitterUrl.TweetRetweetCount = Convert.ToString(dictionary["retweet_count"]).Trim();
                        if (twitterId != null)
                            twitterUrl.TwitterId = twitterId.Trim();
                        else
                            twitterUrl.TwitterId = noDataAvailable;
                        var url = (System.Object)dictionaryResponse["urls"];

                        object[] urlList = (object[])url;
                        for (int idx = 0; idx < urlList.Length; idx++)
                        {

                            var data = (IDictionary<string, object>)urlList[idx];
                            twitterUrl.TweetExpandedUrl = Convert.ToString(data["expanded_url"]).Trim();
                            twitterUrl.TweetDisplayUrl = Convert.ToString(data["display_url"]).Trim();


                        }
                        batchOperation.InsertOrMerge(twitterUrl);
                        if (twitterUrl != null)
                        {
                            tableTwitterUrl.ExecuteBatch(batchOperation);
                            batchOperation = null;

                        }
                    }

                }
            }



        }

        public void StoreUsermentionsMetadata(List<TwitterStatus> tweets, string screenName, string twitterId, long statusCount)
        {
            TwitterUserMentionsEntities twitterUserMention = null;
            batchOperation = new TableBatchOperation();
            string noDataAvailable = "No data available".Trim();
            string noMediaAvailable = "No media available".Trim();


            if (tweets.Count() > 0)
            {
                for (int i = 0; i < tweets.Count(); i++)
                {
                    TwitterStatus tweetdata = tweets[i];
                    var serializer = new JavaScriptSerializer();
                    var dictionary = (IDictionary<string, object>)serializer.DeserializeObject(tweetdata.RawSource);
                    var dictionaryResponse = (IDictionary<string, object>)dictionary["entities"];

                    if (dictionaryResponse.ContainsKey("user_mentions") != false)
                    {
                        // Create a new Instagram entity.
                        Guid guid = Guid.NewGuid();
                        twitterUserMention = new TwitterUserMentionsEntities(guid, screenName.ToLower().Trim());
                        batchOperation = new TableBatchOperation();
                        if (dictionaryResponse.ContainsKey("symbols") != false)
                        {
                            var symbols = (System.Object)dictionaryResponse["symbols"];
                            object[] symbolsList = (object[])symbols;
                        }
                        else
                        {
                        }

                        twitterUserMention.TweetId = Convert.ToString(dictionary["id"]).Trim();
                        twitterUserMention.TweetCreatedTimestamp = Convert.ToString(dictionary["created_at"]).Trim();
                        twitterUserMention.TweetText = Convert.ToString(dictionary["text"]).Trim();
                        twitterUserMention.TweetSource = Convert.ToString(dictionary["source"]).Trim();
                        if (dictionaryResponse.ContainsKey("hashtags") != false)
                        {
                            var hashTag = (System.Object)dictionaryResponse["hashtags"];
                            object[] hashTagList = (object[])hashTag;
                            twitterUserMention.TweetEntityHashTagCount = Convert.ToString(hashTagList.Count()).Trim();
                        }
                        else
                        {
                            twitterUserMention.TweetEntityHashTagCount = noDataAvailable;
                        }

                        if (dictionaryResponse.ContainsKey("media") != false)
                        {
                            var media = (System.Object)dictionaryResponse["media"];
                            object[] mediaList = (object[])media;
                            twitterUserMention.TweetEntityMediaCount = Convert.ToString(mediaList.Count()).Trim();
                        }
                        else { twitterUserMention.TweetEntityMediaCount = noDataAvailable; }

                        if (dictionaryResponse.ContainsKey("urls") != false)
                        {
                            var urls = (System.Object)dictionaryResponse["urls"];
                            object[] urlsList = (object[])urls;
                            twitterUserMention.TweetEntityUrlCount = Convert.ToString(urlsList.Count()).Trim();
                        }
                        else
                        {
                            twitterUserMention.TweetEntityUrlCount = noDataAvailable;
                        }

                        if (dictionaryResponse.ContainsKey("user_mentions") != false)
                        {
                            var userMention = (System.Object)dictionaryResponse["user_mentions"];
                            object[] userMentionList = (object[])userMention;
                            twitterUserMention.TweetEntityUserMentionCount = Convert.ToString(userMentionList.Count()).Trim();
                        }
                        else
                        {
                            twitterUserMention.TweetEntityUserMentionCount = noDataAvailable;
                        }
                        twitterUserMention.TweetRetweetCount = Convert.ToString(dictionary["retweet_count"]).Trim();
                        if (twitterId != null)
                            twitterUserMention.TwitterId = twitterId.Trim();
                        else
                            twitterUserMention.TwitterId = noDataAvailable;
                        var userMentions = (System.Object)dictionaryResponse["user_mentions"];

                        object[] userMentionsList = (object[])userMentions;
                        for (int idx = 0; idx < userMentionsList.Length; idx++)
                        {

                            var data = (IDictionary<string, object>)userMentionsList[idx];
                            twitterUserMention.TweetUserMentionsId = Convert.ToString(data["id"]).Trim();
                            twitterUserMention.TweetUserMentionsName = "@" + Convert.ToString(data["name"]).Trim();


                        }
                        batchOperation.InsertOrMerge(twitterUserMention);
                        if (twitterUserMention != null)
                        {
                            tableTwitterUserMention.ExecuteBatch(batchOperation);
                            batchOperation = null;

                        }
                    }

                }
            }



        }

        //public static string RefreshAccessToken(string currentAccessToken)
        //{
        //    FacebookClient fbClient = new FacebookClient();
        //    Dictionary<string, object> fbParams = new Dictionary<string, object>();
        //    fbParams["client_id"] = "160125594163512";
        //    fbParams["grant_type"] = "fb_exchange_token";
        //    fbParams["client_secret"] = "e77794adf47d3d6a7d64bf9d70af6315";
        //    fbParams["fb_exchange_token"] = currentAccessToken;
        //    JsonObject publishedResponse = fbClient.Get("/oauth/access_token", fbParams) as JsonObject;
        //    return publishedResponse["access_token"].ToString();
        //}
        /// Log Tweet History
        /// </summary>
        /// <param name="eventname"></param>
        /// <param name="foursquareid"></param>
        /// <param name="placename"></param>
        /// <param name="action"></param>
        /// <param name="desc"></param>
        /// <param name="logtime"></param>
        private void LogTweetHistory(string twitterScreenName, string foursquareid, string tickCount, string twitterEntityType, string twitterId,
                                string placename, string tweetsCount, string tweetId, string totalTweets,
                                string logtime)
        {
            string notAvailable = "Not Available".Trim();
            TweetHistory tweetHistoryLog = null;
            TableBatchOperation batchOperationLog = new TableBatchOperation();


            // Create a new Instagram entity.
            Guid guid = Guid.NewGuid();
            tweetHistoryLog = new TweetHistory(tickCount, twitterScreenName);

            if (foursquareid != null)
                tweetHistoryLog.FoursquareId = foursquareid.Trim();
            else
                tweetHistoryLog.FoursquareId = notAvailable;

            if (twitterEntityType != null)
                tweetHistoryLog.tweetType = twitterEntityType.Trim();
            else
                tweetHistoryLog.tweetType = notAvailable;



            if (twitterId != null)
                tweetHistoryLog.TwitterId = twitterId.Trim();
            else
                tweetHistoryLog.TwitterId = notAvailable;

            if (placename != null)
                tweetHistoryLog.PlaceName = placename.Trim();
            else
                tweetHistoryLog.PlaceName = notAvailable;

            tweetHistoryLog.TweetCount = tweetsCount.ToString();

            if (tweetId != null)
                tweetHistoryLog.TweetId = tweetId.Trim();
            else
                tweetHistoryLog.TweetId = notAvailable;



            tweetHistoryLog.TotalTweetCount = totalTweets.Trim();



            tweetHistoryLog.LogTime = DateTimeOffset.UtcNow.UtcDateTime.ToString().Trim();
            batchOperationLog.Insert(tweetHistoryLog);
            if (batchOperationLog.Count != 0)
                tableTwitterHistory.ExecuteBatch(batchOperationLog);

            batchOperationLog = null;

        }


        /// Log HashTag History
        /// </summary>
        /// <param name="eventname"></param>
        /// <param name="foursquareid"></param>
        /// <param name="placename"></param>
        /// <param name="action"></param>
        /// <param name="desc"></param>
        /// <param name="logtime"></param>
        private void LogHashTagHistory(string twitterScreenName, string foursquareid, string tickCount, string twitterEntityType, string twitterId,
                                string placename, string hashTagCount, string tweetId, string hashtags, string totalHashTags,
                                string logtime)
        {
            string notAvailable = "Not Available".Trim();
            HashTagHistory hashTagHistoryLog = null;
            TableBatchOperation batchOperationLog = new TableBatchOperation();


            // Create a new Instagram entity.
            Guid guid = Guid.NewGuid();
            hashTagHistoryLog = new HashTagHistory(tickCount, twitterScreenName);

            if (foursquareid != null)
                hashTagHistoryLog.FoursquareId = foursquareid.Trim();
            else
                hashTagHistoryLog.FoursquareId = notAvailable;

            if (twitterEntityType != null)
                hashTagHistoryLog.tweetType = twitterEntityType.Trim();
            else
                hashTagHistoryLog.tweetType = notAvailable;

            if (twitterId != null)
                hashTagHistoryLog.TwitterId = twitterId.Trim();
            else
                hashTagHistoryLog.TwitterId = notAvailable;

            if (placename != null)
                hashTagHistoryLog.PlaceName = placename.Trim();
            else
                hashTagHistoryLog.PlaceName = notAvailable;

            hashTagHistoryLog.NewHashTagCount = hashTagCount.ToString();

            if (tweetId != null)
                hashTagHistoryLog.TweetId = tweetId.Trim();
            else
                hashTagHistoryLog.TweetId = notAvailable;

            if (hashtags != null)
                hashTagHistoryLog.Hashtags = hashtags.Trim();
            else
                hashTagHistoryLog.Hashtags = notAvailable;

            hashTagHistoryLog.TotalHashTags = totalHashTags.Trim();

            hashTagHistoryLog.LogTime = DateTimeOffset.UtcNow.UtcDateTime.ToString().Trim();

            batchOperationLog.Insert(hashTagHistoryLog);
            if (batchOperationLog.Count != 0)
                tableTwitterHistory.ExecuteBatch(batchOperationLog);

            batchOperationLog = null;

        }

        /// Log Usermentions History
        /// </summary>
        /// <param name="eventname"></param>
        /// <param name="foursquareid"></param>
        /// <param name="placename"></param>
        /// <param name="action"></param>
        /// <param name="desc"></param>
        /// <param name="logtime"></param>
        private void LogUserMentionsHistory(string twitterScreenName, string foursquareid, string tickCount, string twitterEntityType, string twitterId,
                                string placename, string userMentionCount, string tweetId, string userMentions, string totalUserMentions,
                                string logtime)
        {
            string notAvailable = "Not Available".Trim();
            UserMentionHistory userMentionsHistoryLog = null;
            TableBatchOperation batchOperationLog = new TableBatchOperation();


            // Create a new Instagram entity.
            Guid guid = Guid.NewGuid();
            userMentionsHistoryLog = new UserMentionHistory(tickCount, twitterScreenName);

            if (foursquareid != null)
                userMentionsHistoryLog.FoursquareId = foursquareid.Trim();
            else
                userMentionsHistoryLog.FoursquareId = notAvailable;

            if (twitterEntityType != null)
                userMentionsHistoryLog.tweetType = twitterEntityType.Trim();
            else
                userMentionsHistoryLog.tweetType = notAvailable;

            if (twitterId != null)
                userMentionsHistoryLog.TwitterId = twitterId.Trim();
            else
                userMentionsHistoryLog.TwitterId = notAvailable;

            if (placename != null)
                userMentionsHistoryLog.PlaceName = placename.Trim();
            else
                userMentionsHistoryLog.PlaceName = notAvailable;

            userMentionsHistoryLog.NewUsermentionCount = userMentionCount.ToString();

            if (tweetId != null)
                userMentionsHistoryLog.TweetId = tweetId.Trim();
            else
                userMentionsHistoryLog.TweetId = notAvailable;

            if (userMentions != null)
                userMentionsHistoryLog.Usermentions = userMentions.Trim();
            else
                userMentionsHistoryLog.Usermentions = notAvailable;

            userMentionsHistoryLog.TotalUsermentions = totalUserMentions.Trim();

            userMentionsHistoryLog.LogTime = DateTimeOffset.UtcNow.UtcDateTime.ToString().Trim();

            batchOperationLog.Insert(userMentionsHistoryLog);
            if (batchOperationLog.Count != 0)
                tableTwitterHistory.ExecuteBatch(batchOperationLog);

            batchOperationLog = null;

        }

        /// Log TwitterMedia History
        /// </summary>
        /// <param name="eventname"></param>
        /// <param name="foursquareid"></param>
        /// <param name="placename"></param>
        /// <param name="action"></param>
        /// <param name="desc"></param>
        /// <param name="logtime"></param>
        private void LogTwitterMediaHistory(string twitterScreenName, string foursquareid, string tickCount, string twitterEntityType, string twitterId,
                                string placename, string twitterMediaCount, string tweetId, string twitterMediaId, string totalTwitterMedias,
                                string logtime)
        {
            string notAvailable = "Not Available".Trim();
            TwitterMediaHistory twitterMediaHistoryLog = null;
            TableBatchOperation batchOperationLog = new TableBatchOperation();


            // Create a new Instagram entity.
            Guid guid = Guid.NewGuid();
            twitterMediaHistoryLog = new TwitterMediaHistory(tickCount, twitterScreenName);

            if (foursquareid != null)
                twitterMediaHistoryLog.FoursquareId = foursquareid.Trim();
            else
                twitterMediaHistoryLog.FoursquareId = notAvailable;

            if (twitterEntityType != null)
                twitterMediaHistoryLog.tweetType = twitterEntityType.Trim();
            else
                twitterMediaHistoryLog.tweetType = notAvailable;

            if (twitterId != null)
                twitterMediaHistoryLog.TwitterId = twitterId.Trim();
            else
                twitterMediaHistoryLog.TwitterId = notAvailable;

            if (placename != null)
                twitterMediaHistoryLog.PlaceName = placename.Trim();
            else
                twitterMediaHistoryLog.PlaceName = notAvailable;

            twitterMediaHistoryLog.NewTwitterMediaCount = twitterMediaCount.ToString();

            if (tweetId != null)
                twitterMediaHistoryLog.TweetId = tweetId.Trim();
            else
                twitterMediaHistoryLog.TweetId = notAvailable;

            if (twitterMediaId != null)
                twitterMediaHistoryLog.TwitterMediaId = twitterMediaId.Trim();
            else
                twitterMediaHistoryLog.TwitterMediaId = notAvailable;

            twitterMediaHistoryLog.TotalTwitterMedia = totalTwitterMedias.Trim();

            twitterMediaHistoryLog.LogTime = DateTimeOffset.UtcNow.UtcDateTime.ToString().Trim();

            batchOperationLog.Insert(twitterMediaHistoryLog);
            if (batchOperationLog.Count != 0)
                tableTwitterHistory.ExecuteBatch(batchOperationLog);

            batchOperationLog = null;

        }

        /// Log TwitterUrls History
        /// </summary>
        /// <param name="eventname"></param>
        /// <param name="foursquareid"></param>
        /// <param name="placename"></param>
        /// <param name="action"></param>
        /// <param name="desc"></param>
        /// <param name="logtime"></param>
        private void LogTwitterUrlsHistory(string twitterScreenName, string foursquareid, string tickCount, string twitterEntityType, string twitterId,
                                string placename, string tweetUrlsCount, string tweetId, string tweetUrls, string totalTweetUrls,
                                string logtime)
        {
            string notAvailable = "Not Available".Trim();
            TweetUrlsHistory tweetUrlsHistoryLog = null;
            TableBatchOperation batchOperationLog = new TableBatchOperation();


            // Create a new Instagram entity.
            Guid guid = Guid.NewGuid();
            tweetUrlsHistoryLog = new TweetUrlsHistory(tickCount, twitterScreenName);

            if (foursquareid != null)
                tweetUrlsHistoryLog.FoursquareId = foursquareid.Trim();
            else
                tweetUrlsHistoryLog.FoursquareId = notAvailable;

            if (twitterEntityType != null)
                tweetUrlsHistoryLog.tweetType = twitterEntityType.Trim();
            else
                tweetUrlsHistoryLog.tweetType = notAvailable;

            if (twitterId != null)
                tweetUrlsHistoryLog.TwitterId = twitterId.Trim();
            else
                tweetUrlsHistoryLog.TwitterId = notAvailable;

            if (placename != null)
                tweetUrlsHistoryLog.PlaceName = placename.Trim();
            else
                tweetUrlsHistoryLog.PlaceName = notAvailable;

            tweetUrlsHistoryLog.NewUrlsCount = tweetUrlsCount.ToString();

            if (tweetId != null)
                tweetUrlsHistoryLog.TweetId = tweetId.Trim();
            else
                tweetUrlsHistoryLog.TweetId = notAvailable;

            if (tweetUrls != null)
                tweetUrlsHistoryLog.TweetUrls = tweetUrls.Trim();
            else
                tweetUrlsHistoryLog.TweetUrls = notAvailable;

            tweetUrlsHistoryLog.TotalTweetUrls = totalTweetUrls.Trim();

            tweetUrlsHistoryLog.LogTime = DateTimeOffset.UtcNow.UtcDateTime.ToString().Trim();

            batchOperationLog.Insert(tweetUrlsHistoryLog);
            if (batchOperationLog.Count != 0)
                tableTwitterHistory.ExecuteBatch(batchOperationLog);

            batchOperationLog = null;

        }


        /// <summary>
        /// Get Tweet Ids of specified twitter screen name
        /// </summary>
        /// <param name="fourSquareId"></param>
        /// <returns></returns>
        private List<String> GetTweetIds(string twitterScreenName)
        {
            List<String> tweetId = new List<String>();
            try
            {
                // Create the table client.
                tableClientTwitterTweet = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableTwitterTweet = tableClientTwitterTweet.GetTableReference("TwitterTweetMetaData");
                tableTwitterTweet.CreateIfNotExists();
                TableServiceContext tableServiceContext = tableClientTwitterTweet.GetTableServiceContext();

                tweetId = (from x in tableServiceContext.CreateQuery<TwitterTweetEntities>("TwitterTweetMetaData")
                           where x.PartitionKey == twitterScreenName.ToString().Trim()
                           select x).AsEnumerable().Select(t => t.TweetId).ToList();

            }
            catch (StorageException ex)
            {
                //queue.Clear();

                Trace.TraceInformation("Something went wrong");
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            return tweetId;
        }

        private DateTime GetDate(string[] arrTimeStamp, out string tickcount)
        {
            DateTime tweetCreateddt = new DateTime();
            DateTime ticks = new DateTime();
            string month = arrTimeStamp[1];
            string day = arrTimeStamp[2];
            string part3 = arrTimeStamp[3];

            string[] arrTime = part3.Split(':');
            string hr = arrTime[0];
            string min = arrTime[1];
            string sec = arrTime[2];
            string year = arrTimeStamp[5];
            int mon = DateTime.ParseExact(month, "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
            ticks = new DateTime(Convert.ToInt32(year), mon, Convert.ToInt32(day), Convert.ToInt32(hr), Convert.ToInt32(min), Convert.ToInt32(sec));
            tickcount = ticks.Ticks.ToString().Trim();
            tweetCreateddt = new DateTime(Convert.ToInt32(year), mon, Convert.ToInt32(day));
            return tweetCreateddt;
        }
    }
}
