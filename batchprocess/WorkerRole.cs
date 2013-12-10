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
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Text;

namespace batchprocess
{
    public class WorkerRole : RoleEntryPoint
    {
        Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
     CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private CloudQueue queue;
        CloudQueueClient queueClient;
        CloudTable tableFoursquareVenue;
        CloudTableClient tableClientFoursquareVenue;
        CloudTable tableFoursquarePhoto;
        CloudTableClient tableClientFoursquarePhoto;
        CloudTable tableFoursquareTip;
        CloudTableClient tableClientFoursquareTip;
        CloudTable tableInstagram;
        CloudTableClient tableClientInstagram;



        CloudTable tableEvent;
        CloudTableClient tableEventClient;
        CloudTable tableHistory;
        CloudTableClient tableHistoryClient;
        TableBatchOperation batchOperation;

        int offset = 1;
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("batchprocess entry point called", "Information");

            while (true)
            {
                try
                {
                    // Retrieve the cached approximate message count.
                    int? cachedMessageCount = GetQueueCount();
                    if (cachedMessageCount > 0)
                    {

                        foreach (CloudQueueMessage message in queue.GetMessages(30, TimeSpan.FromMinutes(5)))
                        {
                            // Process all messages in less than 5 minutes, deleting each message after processing.
                            if (message != null)
                            {
                                byte[] byteArr = message.AsBytes;
                                string queueMessage = Encoding.UTF8.GetString(byteArr);
                                this.StoreFourSqaureMetadata(queueMessage);
                                this.StoreFourSqaurePhotoMetadata(queueMessage);
                                this.StoreFourSqaureTipMetadata(queueMessage);
                                this.StoreInstagramMetadata(queueMessage);
                            }
                            queue.DeleteMessage(message);
                        }
                        Trace.WriteLine("Working", "Information");
                        Thread.Sleep(10000);
                        Trace.TraceInformation("Working", "Information");

                    }
                    else
                    {


                    }

                    Trace.WriteLine("Working", "Information");

                    Trace.TraceInformation("Working", "Information");
                }
                catch (Exception ex)
                {

                    Trace.TraceInformation("Something went wrong");
                    Trace.TraceError(ex.ToString());

                    //queue.Clear();
                    base.OnStart();
                    //throw ex;
                }
                finally
                {
                }
                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            try
            {

                // Create the queue client
                queueClient = storageAccount.CreateCloudQueueClient();
                // Retrieve a reference to a queue
                queue = queueClient.GetQueueReference("socialmediaadminqueue");

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();


                // Create the table for Foursqaure venue table.
                tableClientFoursquareVenue = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableFoursquareVenue = tableClientFoursquareVenue.GetTableReference("FoursquareVenuesTable");
                tableFoursquareVenue.CreateIfNotExists();

                // Create the table Foursquare photo Table.
                tableClientFoursquarePhoto = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableFoursquarePhoto = tableClientFoursquarePhoto.GetTableReference("FoursquareVenuesPhotoTableVer3");
                tableFoursquarePhoto.CreateIfNotExists();

                // Create the table Foursquare Tip Table.
                tableClientFoursquareTip = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableFoursquareTip = tableClientFoursquareTip.GetTableReference("FoursquareVenuesTipTableVer3");
                tableFoursquareTip.CreateIfNotExists();

                // Create the table Instagram media Table.
                tableClientInstagram = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableInstagram = tableClientInstagram.GetTableReference("InstagramLocationPhotosTableVer1");
                tableInstagram.CreateIfNotExists();

                // Create the table Instagram Event.
                tableEventClient = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableEvent = tableEventClient.GetTableReference("InstagramEventLogTable");
                tableEvent.CreateIfNotExists();


                // Create the table Instagram History.
                tableHistoryClient = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableHistory = tableHistoryClient.GetTableReference("InstagramHistoryLogTable");
                tableHistory.CreateIfNotExists();

            }
            catch (Exception ex)
            {
                //throw ex;

            }
            finally
            {
            }
            return base.OnStart();
        }
        /// <summary>
        /// Get Queue count
        /// </summary>
        /// <returns></returns>
        private int? GetQueueCount()
        {
            // Fetch the queue attributes.
            queue.FetchAttributes();

            // Retrieve the cached approximate message count.
            int? cachedMessageCount = queue.ApproximateMessageCount;
            return cachedMessageCount;
        }

        public void StoreFourSqaureMetadata(string fourSquareId)
        {


            HttpClient client1 = null;
            HttpResponseMessage response1 = null;
            TableServiceContext tableServiceContext = tableClientFoursquareVenue.GetTableServiceContext();
            string noDataAvailable = "No data available".Trim();
            try
            {
                var d = (from x in tableServiceContext.CreateQuery<FoursquareEntities>("FoursquareVenuesTable")
                         where x.PartitionKey == fourSquareId.ToString().Trim()

                         select x).FirstOrDefault();
                if (d == null)
                {
                    client1 = new HttpClient();
                    response1 = client1.GetAsync("https://api.foursquare.com/v2/venues/" + fourSquareId + "?oauth_token=XTM4IHI0VMKMDBNDD4NB025SDXAGQVMLHSZMQLVJHMCTYPLT&v=20130713").Result;


                    if (response1 != null)
                    {
                        if (response1.IsSuccessStatusCode)
                        {
                            response1.EnsureSuccessStatusCode();

                            string responseBody1 = response1.Content.ReadAsStringAsync().Result;
                            var serializer1 = new JavaScriptSerializer();
                            var dictionary1 = (IDictionary<string, object>)serializer1.DeserializeObject(responseBody1);
                            var data = (IDictionary<string, object>)dictionary1["response"];
                            if (data.Count != 0)
                            {
                                batchOperation = new TableBatchOperation();
                                FoursquareEntities fourSquareEntity = null;

                                var data2 = (IDictionary<string, object>)data["venue"];
                                string locationId = (string)data2["id"];
                                string locationName = (string)data2["name"];
                                // Create a new Instagram entity.
                                Guid guid = Guid.NewGuid();
                                fourSquareEntity = new FoursquareEntities(guid, fourSquareId.Trim());
                                fourSquareEntity.FoursquareID = locationId.Trim();
                                if (data2.ContainsKey("url") != false)
                                    fourSquareEntity.Url = (string)data2["url"];
                                else
                                    fourSquareEntity.Url = noDataAvailable;

                                if (data2.ContainsKey("canonicalUrl") != false)
                                    fourSquareEntity.CanonicalUrl = (string)data2["canonicalUrl"];
                                else
                                    fourSquareEntity.CanonicalUrl = (string)data2["canonicalUrl"];

                                var categories = (object[])data2["categories"];
                                for (int i = 0; i < categories.Length; i++)
                                {
                                    var category = (IDictionary<string, object>)categories[i];
                                    fourSquareEntity.CategoryId += (string)category["id"].ToString().Trim() + ",";
                                    fourSquareEntity.CategoryName += (string)category["name"].ToString().Trim() + ",";
                                }

                                #region GetContact
                                // Get Contact Metadata-------------------------------------------------
                                var contact = (IDictionary<string, object>)data2["contact"];

                                if (contact.Count() != 0)
                                {
                                    if (contact.ContainsKey("twitter") != false)
                                    {
                                        fourSquareEntity.TwitterScreenName = (string)contact["twitter"];
                                    }
                                    else
                                        fourSquareEntity.TwitterScreenName = noDataAvailable;

                                    if (contact.ContainsKey("phone") != false)
                                    {
                                        fourSquareEntity.ContactNo = (string)contact["phone"];
                                    }
                                    else
                                        fourSquareEntity.ContactNo = noDataAvailable;

                                }
                                else
                                {
                                    fourSquareEntity.ContactNo = noDataAvailable;
                                    fourSquareEntity.TwitterScreenName = noDataAvailable;
                                }
                                #endregion
                                //-----------------------------------------------------------------------------
                                #region GetLocation
                                //Get Location Data
                                var location = (IDictionary<string, object>)data2["location"];
                                if (location.Count() != 0)
                                {
                                    if (location.ContainsKey("address") != false)
                                    {
                                        fourSquareEntity.Address = (string)location["address"];
                                    }
                                    else
                                        fourSquareEntity.Address = noDataAvailable;

                                    if (location.ContainsKey("lat") != false)
                                    {
                                        fourSquareEntity.Latitude = Convert.ToString(location["lat"]);
                                    }
                                    else
                                        fourSquareEntity.Latitude = noDataAvailable;

                                    if (location.ContainsKey("lng") != false)
                                    {
                                        fourSquareEntity.Longitude = Convert.ToString(location["lng"]);
                                    }
                                    else
                                        fourSquareEntity.Longitude = noDataAvailable;

                                    if (location.ContainsKey("postalCode") != false)
                                    {
                                        fourSquareEntity.PostalCode = (string)location["postalCode"];
                                    }
                                    else
                                        fourSquareEntity.PostalCode = noDataAvailable;

                                    if (location.ContainsKey("city") != false)
                                    {
                                        fourSquareEntity.City = (string)location["city"];
                                    }
                                    else
                                        fourSquareEntity.City = noDataAvailable;

                                    if (location.ContainsKey("state") != false)
                                    {
                                        fourSquareEntity.State = (string)location["state"];
                                    }
                                    else
                                        fourSquareEntity.State = noDataAvailable;

                                    if (location.ContainsKey("country") != false)
                                    {
                                        fourSquareEntity.Country = (string)location["country"];
                                    }
                                    else
                                        fourSquareEntity.Country = noDataAvailable;

                                }
                                else
                                {
                                    fourSquareEntity.Address = noDataAvailable;
                                    fourSquareEntity.Latitude = noDataAvailable;
                                    fourSquareEntity.Longitude = noDataAvailable;
                                    fourSquareEntity.PostalCode = noDataAvailable;
                                    fourSquareEntity.City = noDataAvailable;
                                    fourSquareEntity.State = noDataAvailable;
                                    fourSquareEntity.Country = noDataAvailable;
                                }
                                #endregion

                                //----------------------------------------------------------------------------------
                                #region GetStats
                                //Get Location Data
                                var stats = (IDictionary<string, object>)data2["stats"];
                                if (stats.Count() != 0)
                                {
                                    if (stats.ContainsKey("checkinsCount") != false)
                                    {
                                        fourSquareEntity.CheckinsCount = Convert.ToString(stats["checkinsCount"]);
                                    }
                                    else
                                        fourSquareEntity.CheckinsCount = noDataAvailable;

                                    if (stats.ContainsKey("userCount") != false)
                                    {
                                        fourSquareEntity.UserCount = Convert.ToString(stats["userCount"]);
                                    }
                                    else
                                        fourSquareEntity.UserCount = noDataAvailable;

                                    if (stats.ContainsKey("tipCount") != false)
                                    {
                                        fourSquareEntity.TipCount = Convert.ToString(stats["tipCount"]);
                                    }
                                    else
                                        fourSquareEntity.TipCount = noDataAvailable;

                                }
                                else
                                {
                                    fourSquareEntity.CheckinsCount = noDataAvailable;
                                    fourSquareEntity.UserCount = noDataAvailable;
                                    fourSquareEntity.TipCount = noDataAvailable;
                                }
                                #endregion

                                //-------------------------------------------------------------------------------
                                #region Getlikes
                                //Get Likes Data
                                var likes = (IDictionary<string, object>)data2["likes"];
                                if (likes.Count() != 0)
                                {
                                    if (likes.ContainsKey("count") != false)
                                    {
                                        fourSquareEntity.LikesCount = Convert.ToString(likes["count"]);
                                    }
                                    else
                                        fourSquareEntity.LikesCount = noDataAvailable;

                                }
                                else
                                {
                                    fourSquareEntity.LikesCount = noDataAvailable;

                                }
                                #endregion
                                //---------------------------------------------------------------------------------

                                #region GetMenu
                                //Get Menu Data
                                if (data2.ContainsKey("menu") != false)
                                {
                                    var menu = (IDictionary<string, object>)data2["menu"];
                                    if (menu.Count() != 0)
                                    {
                                        if (menu.ContainsKey("type") != false)
                                        {
                                            fourSquareEntity.MenuType = (string)menu["type"];
                                        }
                                        else
                                            fourSquareEntity.MenuType = noDataAvailable;

                                        if (menu.ContainsKey("url") != false)
                                        {
                                            fourSquareEntity.MenuUrl = (string)menu["url"];
                                        }
                                        else
                                            fourSquareEntity.MenuUrl = noDataAvailable;

                                        if (menu.ContainsKey("mobileUrl") != false)
                                        {
                                            fourSquareEntity.MenuMobileUrl = (string)menu["mobileUrl"];
                                        }
                                        else
                                            fourSquareEntity.MenuMobileUrl = noDataAvailable;

                                    }
                                    else
                                    {
                                        fourSquareEntity.MenuType = noDataAvailable;
                                        fourSquareEntity.MenuUrl = noDataAvailable;
                                        fourSquareEntity.MenuMobileUrl = noDataAvailable;

                                    }
                                }
                                else
                                {
                                    fourSquareEntity.MenuType = noDataAvailable;
                                    fourSquareEntity.MenuUrl = noDataAvailable;
                                    fourSquareEntity.MenuMobileUrl = noDataAvailable;
                                }
                                #endregion
                                //--------------------------------------------------------------------------------
                                #region GetVenuePage
                                //Get VenuePage Data
                                if (data2.ContainsKey("venuePage") != false)
                                {
                                    var venuePage = (IDictionary<string, object>)data2["venuePage"];
                                    if (venuePage.Count() != 0)
                                    {
                                        if (venuePage.ContainsKey("id") != false)
                                        {
                                            fourSquareEntity.VenuePageId = Convert.ToString(venuePage["id"]);
                                        }
                                    }
                                    else
                                    {
                                        fourSquareEntity.VenuePageId = noDataAvailable;
                                    }
                                }
                                else
                                    fourSquareEntity.VenuePageId = noDataAvailable;

                                #endregion

                                #region GetDescription
                                //Get VenuePage Data   
                                if (data2.ContainsKey("description") != false)
                                {
                                    fourSquareEntity.Description = (string)data2["description"];
                                }

                                else
                                {
                                    fourSquareEntity.Description = noDataAvailable;
                                }
                                #endregion
                                //---------------------------------------------------------------------------
                                #region GetRating
                                //Get VenuePage Data   
                                if (data2.ContainsKey("rating") != false)
                                {
                                    fourSquareEntity.Rating = Convert.ToString(data2["rating"]);
                                }

                                else
                                {
                                    fourSquareEntity.Rating = noDataAvailable;
                                }
                                #endregion
                                //---------------------------------------------------------------------------
                                #region GetCreatedAt
                                //Get CreatedAt Data   
                                if (data2.ContainsKey("createdAt") != false)
                                {
                                    fourSquareEntity.CreatedAt = Convert.ToString(data2["createdAt"]);
                                }

                                else
                                {
                                    fourSquareEntity.CreatedAt = noDataAvailable;
                                }
                                #endregion
                                //---------------------------------------------------------------------------
                                #region GetShortUrl
                                //Get ShortUrl Data   
                                if (data2.ContainsKey("shortUrl") != false)
                                {
                                    fourSquareEntity.ShortUrl = (string)data2["shortUrl"];
                                }

                                else
                                {
                                    fourSquareEntity.ShortUrl = noDataAvailable;
                                }
                                #endregion
                                //---------------------------------------------------------------------------
                                #region GetTimeZone
                                //Get TimeZone Data   
                                if (data2.ContainsKey("timeZone") != false)
                                {
                                    fourSquareEntity.TimeZone = Convert.ToString(data2["timeZone"]);
                                }

                                else
                                {
                                    fourSquareEntity.TimeZone = noDataAvailable;
                                }
                                #endregion
                                //---------------------------------------------------------------------------
                                #region GetPhrase
                                //Get TimeZone Data   
                                //Get VenuePage Data
                                if (data2.ContainsKey("phrases") != false)
                                {
                                    var phrase = (object[])data2["phrases"];
                                    if (phrase.Count() != 0)
                                    {

                                        var sample = (IDictionary<string, object>)phrase[0];
                                        var sampleText = (IDictionary<string, object>)sample["sample"];
                                        fourSquareEntity.PhraseText = (string)sampleText["text"];

                                    }
                                    else
                                    {
                                        fourSquareEntity.PhraseText = noDataAvailable;
                                    }
                                }
                                else
                                    fourSquareEntity.PhraseText = noDataAvailable;

                                #endregion
                                //---------------------------------------------------------------------------
                                fourSquareEntity.PlaceName = locationName.Trim();


                                batchOperation.InsertOrMerge(fourSquareEntity);
                                tableFoursquareVenue.ExecuteBatch(batchOperation);
                            }
                        }
                        else
                        {
                            Thread.Sleep(360000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(360000);
                    }

                }
            }

            catch (StorageException ex)
            {
                var extendedInformation = ex.RequestInformation.ExtendedErrorInformation;
                var errorCode = extendedInformation.ErrorCode;
                var addDetails = extendedInformation.AdditionalDetails;
                throw ex;
            }
            finally
            {

            }


        }
        /// <summary>
        /// Store Foursquare Photos based on foursquare id
        /// </summary>
        /// <param name="fourSquareId"></param>
        public void StoreFourSqaurePhotoMetadata(string fourSquareId)
        {


            HttpClient client1 = null;
            HttpResponseMessage response1 = null;
            TableServiceContext tableServiceContext = tableClientFoursquarePhoto.GetTableServiceContext();
            batchOperation = new TableBatchOperation();
            FoursqaureVenuePhotoEntities fourSqaureVenuePhotoEntities = null;
            string noDataAvailable = "No data available".Trim();
            bool hasPhotoIds = false;

            try
            {
                List<String> PhotoIds = new List<String>();



                client1 = new HttpClient();
                for (int offset = 1; offset <= 100; offset++)
                {
                    PhotoIds = this.GetPhotoIds(fourSquareId);
                    response1 = client1.GetAsync("https://api.foursquare.com/v2/venues/" + fourSquareId + "/photos?limit=200&offset=" + offset + "&client_id=SCJB54YMMFSLARWZFKCN1AGWCIBYGU5YYJYLI0W3I5HF25TF&client_secret=V0FBLVKX0RHI5YRLXXYNWS0YSMBPMX40JPWGVLM0X1DOOJTD&v=20130731").Result;


                    if (response1 != null)
                    {
                        if (response1.IsSuccessStatusCode)
                        {
                            response1.EnsureSuccessStatusCode();

                            string responseBody1 = response1.Content.ReadAsStringAsync().Result;
                            var serializer1 = new JavaScriptSerializer();
                            var dictionary1 = (IDictionary<string, object>)serializer1.DeserializeObject(responseBody1);
                            var data = (IDictionary<string, object>)dictionary1["response"];
                            if (data.Count != 0)
                            {

                                if (data.ContainsKey("photos") != false)
                                {
                                    var photos = (IDictionary<string, object>)data["photos"];

                                    if (photos.Count != 0)
                                    {

                                        if (photos.ContainsKey("items") != false)
                                        {
                                            var items = (object[])photos["items"];
                                            for (int idx = 0; idx < items.Length; idx++)
                                            {   // Create a new Instagram entity.
                                                var imageList = (IDictionary<string, object>)items[idx];

                                                string photoId = Convert.ToString(imageList["id"]).Trim();
                                                if (PhotoIds.Count != 0)
                                                    hasPhotoIds = PhotoIds.Contains(photoId);

                                                if (hasPhotoIds == false)
                                                {
                                                    string notAvailable = "Not Available".Trim();
                                                    Guid guid = Guid.NewGuid();
                                                    fourSqaureVenuePhotoEntities = new FoursqaureVenuePhotoEntities(guid, fourSquareId.Trim());

                                                    if (imageList.ContainsKey("id") != false)
                                                        fourSqaureVenuePhotoEntities.PhotoId = Convert.ToString(imageList["id"]).Trim();
                                                    else
                                                        fourSqaureVenuePhotoEntities.PhotoId = notAvailable;

                                                    if (imageList.ContainsKey("createdAt") != false)
                                                        fourSqaureVenuePhotoEntities.PhotoCreatedAt = Convert.ToString(imageList["createdAt"]).Trim();
                                                    else
                                                        fourSqaureVenuePhotoEntities.PhotoCreatedAt = notAvailable;

                                                    if (imageList.ContainsKey("height") != false)
                                                        fourSqaureVenuePhotoEntities.PhotoHeight = Convert.ToString(imageList["height"]).Trim();
                                                    else
                                                        fourSqaureVenuePhotoEntities.PhotoHeight = notAvailable;

                                                    if (imageList.ContainsKey("width") != false)
                                                        fourSqaureVenuePhotoEntities.PhotoWidth = Convert.ToString(imageList["width"]).Trim();
                                                    else
                                                        fourSqaureVenuePhotoEntities.PhotoWidth = notAvailable;

                                                    if (imageList.ContainsKey("source") != false)
                                                    {
                                                        var source = (IDictionary<string, object>)imageList["source"];

                                                        if (source.ContainsKey("url") != false)
                                                            fourSqaureVenuePhotoEntities.PhotoSource = Convert.ToString(source["url"]).Trim();
                                                        else
                                                            fourSqaureVenuePhotoEntities.PhotoSource = notAvailable;

                                                    }
                                                    else
                                                    {
                                                        fourSqaureVenuePhotoEntities.PhotoSource = notAvailable;
                                                    }

                                                    if (imageList.ContainsKey("prefix") != false)
                                                        fourSqaureVenuePhotoEntities.PhotoPrefix = Convert.ToString(imageList["prefix"]).Trim();
                                                    else
                                                        fourSqaureVenuePhotoEntities.PhotoPrefix = notAvailable;

                                                    if (imageList.ContainsKey("suffix") != false)
                                                        fourSqaureVenuePhotoEntities.PhotoSuffix = Convert.ToString(imageList["suffix"]).Trim();
                                                    else
                                                        fourSqaureVenuePhotoEntities.PhotoSuffix = notAvailable;

                                                    if (imageList.ContainsKey("user") != false)
                                                    {
                                                        var user = (IDictionary<string, object>)imageList["user"];
                                                        fourSqaureVenuePhotoEntities.PhotoUploadedBy = Convert.ToString(user["firstName"]).Trim();
                                                    }
                                                    batchOperation.InsertOrMerge(fourSqaureVenuePhotoEntities);
                                                    tableFoursquarePhoto.ExecuteBatch(batchOperation);
                                                    batchOperation = null;
                                                    batchOperation = new TableBatchOperation();
                                                }
                                            }

                                        }
                                    }

                                }
                                //table.ExecuteBatch(batchOperation);
                            }
                        }
                        else
                        {
                            //Thread.Sleep(360000);
                        }
                    }
                    else
                    {
                        //Thread.Sleep(360000);
                    }
                }


            }

            catch (Exception ex)
            {

                Trace.TraceInformation("Something went wrong");
                Trace.TraceError(ex.ToString());
                base.OnStart();
                //var extendedInformation = ex.RequestInformation.ExtendedErrorInformation;
                //var errorCode = extendedInformation.ErrorCode;
                //var addDetails = extendedInformation.AdditionalDetails;
                //throw ex;
            }
            finally
            {

            }


        }

        /// <summary>
        /// Store Foursquare Tip/Comments based on foursquare id
        /// </summary>
        /// <param name="fourSquareId"></param>
        public void StoreFourSqaureTipMetadata(string fourSquareId)
        {


            HttpClient client1 = null;
            HttpResponseMessage response1 = null;
            TableServiceContext tableServiceContext = tableClientFoursquarePhoto.GetTableServiceContext();
            batchOperation = new TableBatchOperation();
            FoursqaureVenueTipEntities fourSqaureVenueTipEntities = null;
            string noDataAvailable = "No data available".Trim();
            bool hasTipIds = false;
            try
            {
                List<String> TipIds = new List<String>();


                client1 = new HttpClient();

                for (int offset = 1; offset <= 100; offset++)
                {
                    TipIds = this.GetTipIds(fourSquareId);
                    response1 = client1.GetAsync("https://api.foursquare.com/v2/venues/" + fourSquareId + "/tips?limit=500&offset=" + offset + "&client_id=SCJB54YMMFSLARWZFKCN1AGWCIBYGU5YYJYLI0W3I5HF25TF&client_secret=V0FBLVKX0RHI5YRLXXYNWS0YSMBPMX40JPWGVLM0X1DOOJTD&v=20130731").Result;


                    if (response1 != null)
                    {
                        if (response1.IsSuccessStatusCode)
                        {
                            response1.EnsureSuccessStatusCode();

                            string responseBody1 = response1.Content.ReadAsStringAsync().Result;
                            var serializer1 = new JavaScriptSerializer();
                            var dictionary1 = (IDictionary<string, object>)serializer1.DeserializeObject(responseBody1);
                            var data = (IDictionary<string, object>)dictionary1["response"];
                            if (data.Count != 0)
                            {

                                var tips = (IDictionary<string, object>)data["tips"];
                                if (tips.Count != 0)
                                {


                                    var items = (object[])tips["items"];
                                    for (int idx = 0; idx < items.Length; idx++)
                                    {
                                        var imageList = (IDictionary<string, object>)items[idx];
                                        string tipId = Convert.ToString(imageList["id"]).Trim();
                                        if (TipIds.Count != 0)
                                            hasTipIds = TipIds.Contains(tipId);

                                        if (hasTipIds == false)
                                        {
                                            string notAvailable = "Not available".Trim();
                                            // Create a new Instagram entity.
                                            Guid guid = Guid.NewGuid();
                                            fourSqaureVenueTipEntities = new FoursqaureVenueTipEntities(guid, fourSquareId.Trim());

                                            if (imageList.ContainsKey("id") != false)
                                                fourSqaureVenueTipEntities.TipId = Convert.ToString(imageList["id"]).Trim();
                                            else
                                                fourSqaureVenueTipEntities.TipId = notAvailable;

                                            if (imageList.ContainsKey("createdAt") != false)
                                                fourSqaureVenueTipEntities.TipCreatedAt = Convert.ToString(imageList["createdAt"]).Trim();
                                            else
                                                fourSqaureVenueTipEntities.TipCreatedAt = notAvailable;

                                            if (imageList.ContainsKey("text") != false)
                                                fourSqaureVenueTipEntities.TipText = Convert.ToString(imageList["text"]).Trim();
                                            else
                                                fourSqaureVenueTipEntities.TipText = notAvailable;

                                            if (imageList.ContainsKey("canonicalUrl") != false)
                                                fourSqaureVenueTipEntities.TipCanonicalUrl = Convert.ToString(imageList["canonicalUrl"]).Trim();
                                            else
                                                fourSqaureVenueTipEntities.TipCanonicalUrl = notAvailable;

                                            if (imageList.ContainsKey("user") != false)
                                            {
                                                var user = (IDictionary<string, object>)imageList["user"];
                                                fourSqaureVenueTipEntities.User = Convert.ToString(user["firstName"]).Trim();
                                            }
                                            else
                                                fourSqaureVenueTipEntities.User = notAvailable;

                                            batchOperation.InsertOrMerge(fourSqaureVenueTipEntities);
                                            tableFoursquareTip.ExecuteBatch(batchOperation);
                                            batchOperation = null;
                                            batchOperation = new TableBatchOperation();
                                        }
                                    }
                                }


                                //table.ExecuteBatch(batchOperation);
                            }
                        }
                        else
                        {
                            //Thread.Sleep(360000);
                        }
                    }
                    else
                    {
                        //Thread.Sleep(360000);
                    }
                }

            }

            catch (Exception ex)
            {

                Trace.TraceInformation("Something went wrong");
                Trace.TraceError(ex.ToString());
                base.OnStart();
            }
            finally
            {

            }


        }
        /// <summary>
        /// Store Metadata
        /// </summary>
        /// <param name="fourSquareId">fourSquareId</param>
        public void StoreInstagramMetadata(string fourSquareId)
        {
            HttpClient client1 = null;
            HttpResponseMessage response1 = null;
            HttpClient client2 = null;
            HttpResponseMessage response2 = null;


            List<object> photoMetadata = new List<object>();
            try
            {
                TableServiceContext tableServiceContext = tableClientInstagram.GetTableServiceContext();
                List<String> mediaIds = new List<String>();

                mediaIds = this.GetMediaIds(fourSquareId);
                LogEvent("GetAllMediaIds", "0", "Not available", "GetMedia", "Getting total media id: " + Convert.ToString(mediaIds.Count), DateTime.UtcNow.ToString());
                batchOperation = new TableBatchOperation();
                client1 = new HttpClient();
                response1 = client1.GetAsync("https://api.instagram.com/v1/locations/search?foursquare_v2_id=" + fourSquareId + "&client_id=90d15e59c78d49abbfe1af6e25e4e83d").Result;
                if (response1 != null)
                {
                    if (response1.IsSuccessStatusCode)
                    {
                        response1.EnsureSuccessStatusCode();

                        string responseBody1 = response1.Content.ReadAsStringAsync().Result;
                        var serializer1 = new JavaScriptSerializer();
                        var dictionary1 = (IDictionary<string, object>)serializer1.DeserializeObject(responseBody1);
                        var data = (object[])dictionary1["data"];
                        if (data.Length != 0)
                        {
                            var data2 = (IDictionary<string, object>)data[0];
                            string locationId = (string)data2["id"];
                            string locationName = (string)data2["name"];
                            string locationFullName = locationName;

                            client2 = new HttpClient();
                            response2 = client2.GetAsync("https://api.instagram.com/v1/locations/" + locationId.Trim() + "/media/recent?client_id=90d15e59c78d49abbfe1af6e25e4e83d").Result;
                            if (response2 != null)
                            {
                                if (response2.IsSuccessStatusCode)
                                {
                                    response2.EnsureSuccessStatusCode();
                                    string responseBody2 = response2.Content.ReadAsStringAsync().Result;
                                    var serializer2 = new JavaScriptSerializer();
                                    var dictionaryPhoto = (IDictionary<string, object>)serializer2.DeserializeObject(responseBody2);
                                    var dataPhoto = (object[])dictionaryPhoto["data"];
                                    if (dataPhoto.Length != 0)
                                    {

                                        for (int idx = 0; idx < dataPhoto.Length; idx++)
                                        {
                                            photoMetadata.Add(dataPhoto[idx]);
                                        }
                                    }
                                }
                            }


                            if (photoMetadata.Count != 0)
                            {

                                InstagramPhotoEntity instagram = null;

                                string type = null;
                                string imageTitle = null;
                                string fullname = null;
                                string userPhoto = null;
                                string commentedBy = null;
                                string time = null;
                                string mediaId = null;
                                bool hasMediaIds = false;
                                int photoCount = 0;
                                for (int idx = 0; idx < photoMetadata.Count; idx++)
                                {
                                    var imageList = (IDictionary<string, object>)photoMetadata[idx];
                                    mediaId = Convert.ToString(imageList["id"]).Trim();

                                    if (mediaIds.Count != 0)
                                        hasMediaIds = mediaIds.Contains(mediaId);

                                    if (hasMediaIds == false)
                                    {
                                        photoCount = photoCount + 1;
                                        LogEvent("CheckMediaExist", fourSquareId.Trim(), locationFullName.Trim(), "CheckMediaExist", "Not exist.  New media found: " + mediaId, DateTime.UtcNow.ToString());
                                        if (imageList.ContainsKey("images") != false)
                                        {
                                            var thumbnailList = (IDictionary<string, object>)imageList["images"];

                                            if (thumbnailList.ContainsKey("low_resolution") != false)
                                            {
                                                var thumbnail = (IDictionary<string, object>)thumbnailList["low_resolution"];
                                                long sec = 0;
                                                //TimeSpan t = DateTime.UtcNow.TimeOfDay;
                                                if (imageList.ContainsKey("created_time") != false)
                                                {
                                                    sec = Convert.ToInt64(imageList["created_time"]);
                                                    DateTime dateTimeFromPartitionKeyValue = new DateTime(sec);



                                                    time = dateTimeFromPartitionKeyValue.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                                                }
                                                //instagram.CreatedTime = time.Trim();
                                                string link = null;
                                                string notAvailable = "Not Available".Trim();
                                                if (imageList.ContainsKey("link") != false)
                                                    link = Convert.ToString(imageList["link"]).Trim();
                                                else
                                                    link = notAvailable;

                                                if (imageList.ContainsKey("type") != false)
                                                {
                                                    if (Convert.ToString(imageList["type"]) != null)
                                                    {
                                                        type = Convert.ToString(imageList["type"]);
                                                    }
                                                    else
                                                    { type = "No type"; }
                                                }
                                                if (imageList.ContainsKey("caption") != false)
                                                {
                                                    var caption = (IDictionary<string, object>)imageList["caption"];
                                                    if (caption != null)
                                                    {
                                                        if (caption.ContainsKey("text") != false)
                                                            imageTitle = (string)caption["text"];
                                                        else
                                                            imageTitle = notAvailable;

                                                    }
                                                    else
                                                    {
                                                        imageTitle = "No Title";
                                                    }
                                                }

                                                string url = null;
                                                string width = null;
                                                string height = null;

                                                if (thumbnail.ContainsKey("url") != false)
                                                    url = (string)thumbnail["url"];

                                                if (thumbnail.ContainsKey("width") != false)
                                                    width = Convert.ToString(thumbnail["width"]);

                                                if (thumbnail.ContainsKey("height") != false)
                                                    height = Convert.ToString(thumbnail["height"]);

                                                string commentText = null;
                                                if (imageList.ContainsKey("comments") != false)
                                                {
                                                    var comments = (IDictionary<string, object>)imageList["comments"];
                                                    if (comments.ContainsKey("data") != false)
                                                    {
                                                        var dataComments = (object[])comments["data"];

                                                        if (dataComments.Length != 0)
                                                        {
                                                            for (int commentIndex = 0; commentIndex < dataComments.Length; commentIndex++)
                                                            {
                                                                var commentsAttrib = (IDictionary<string, object>)dataComments[commentIndex];
                                                                if (dataComments.Length > 1)
                                                                {

                                                                    commentText += (string)commentsAttrib["text"] + ";";

                                                                }
                                                                else
                                                                {
                                                                    commentText = (string)commentsAttrib["text"];
                                                                }

                                                                var from = (IDictionary<string, object>)commentsAttrib["from"];

                                                                if (dataComments.Length > 1)
                                                                {

                                                                    commentedBy += (string)from["full_name"] + ";";
                                                                }
                                                                else
                                                                {

                                                                    commentedBy += (string)from["full_name"];
                                                                }
                                                                //string locationName = (string)data2["name"];
                                                            }
                                                        }
                                                        else
                                                            commentText = "No Comments".Trim();
                                                    }
                                                }

                                                if (imageList.ContainsKey("user") != false)
                                                {
                                                    var user = (IDictionary<string, object>)imageList["user"];
                                                    if (user != null)
                                                    {
                                                        fullname = Convert.ToString(user["full_name"]);
                                                        userPhoto = Convert.ToString(user["profile_picture"]);
                                                    }
                                                    else
                                                    {
                                                        fullname = "No User Name".Trim();
                                                        userPhoto = "No user photo".Trim();
                                                    }
                                                }
                                                Guid guidInsta = Guid.NewGuid();
                                                instagram = new InstagramPhotoEntity(guidInsta, fourSquareId.Trim());
                                                if (locationId != null)
                                                    instagram.InstagramLocationId = locationId.Trim();
                                                else
                                                    instagram.InstagramLocationId = "No data available".Trim();

                                                if (locationFullName != null)
                                                    instagram.InstagramLocationName = locationFullName.Trim();
                                                else
                                                    instagram.InstagramLocationName = "No data available".Trim();

                                                if (type != null)
                                                    instagram.MetaDataType = type.Trim();
                                                else
                                                    instagram.MetaDataType = "No data available".Trim();

                                                if (link != null)
                                                    instagram.InstagramLink = link.Trim();
                                                else
                                                    instagram.InstagramLink = "No data available".Trim();

                                                if (imageTitle != null)
                                                    instagram.ImageTitle = imageTitle.Trim();
                                                else
                                                    instagram.ImageTitle = "No data available".Trim();

                                                if (url != null)
                                                    instagram.ImageUrl = url.Trim();
                                                else
                                                    instagram.ImageUrl = "No data available".Trim();

                                                if (width != null)
                                                    instagram.ImageWidth = width.Trim();
                                                else
                                                    instagram.ImageWidth = "No data available".Trim();

                                                if (height != null)
                                                    instagram.ImageHeight = height.Trim();
                                                else
                                                    instagram.ImageHeight = "No data available".Trim();

                                                if (fullname != null)
                                                    instagram.UploadedBy = fullname.Trim();
                                                else
                                                    instagram.UploadedBy = "No data available".Trim();

                                                if (commentText != null)
                                                    instagram.Comments = commentText.Trim();
                                                else
                                                    instagram.Comments = "No data available".Trim();

                                                if (commentedBy != null)
                                                    instagram.CommentedBy = commentedBy.Trim();
                                                else
                                                    instagram.CommentedBy = "No data available".Trim();

                                                if (mediaId != null)
                                                    instagram.MediaID = mediaId.Trim();
                                                else
                                                    instagram.MediaID = "No data available".Trim();

                                                if (time != null)
                                                    instagram.CreatedTime = time.Trim();
                                                else
                                                    instagram.CreatedTime = "No data available".Trim();



                                                batchOperation.InsertOrMerge(instagram);


                                                LogEvent("StoreMetaData", fourSquareId.Trim(), locationFullName.Trim(), "ProcessVenues", "Processed and stored media", DateTime.UtcNow.ToString());

                                            }
                                        }
                                    }

                                    else
                                    {
                                        LogEvent("CheckMediaExist", fourSquareId.Trim(), locationFullName.Trim(), "CheckMediaExist", "Already exist.", DateTime.UtcNow.ToString());
                                    }

                                }
                                if (batchOperation.Count != 0)
                                    tableInstagram.ExecuteBatch(batchOperation);

                                //mediaIds = this.GetMediaIds(fourSquareId);
                                string socialmediatype = DateTimeOffset.UtcNow.UtcDateTime.ToLongDateString().Trim() + "_" + "InstagramMetadata";
                                LogHistory(socialmediatype.Trim(), fourSquareId, locationFullName, Convert.ToString(photoCount), DateTime.UtcNow.ToString());
                                batchOperation = null;
                                batchOperation = new TableBatchOperation();
                                response2.Dispose();
                                client2.Dispose();


                                instagram = null;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(900000);
                    }
                }

            }
            catch (Exception ex)
            {

                base.OnStart();
            }
            finally
            {
                response2 = null;
                client2 = null;
                response1 = null;
                client1 = null;
            }


        }
        /// Log Event
        /// </summary>
        /// <param name="eventname"></param>
        /// <param name="foursquareid"></param>
        /// <param name="placename"></param>
        /// <param name="action"></param>
        /// <param name="desc"></param>
        /// <param name="logtime"></param>
        private void LogEvent(string eventname, string foursquareid, string placename, string action, string desc, string logtime)
        {
            EventLog eventlog = null;
            TableBatchOperation batchOperationLog = new TableBatchOperation();
            // Create a new Instagram entity.
            Guid guid = Guid.NewGuid();
            eventlog = new EventLog(guid, eventname.Trim());
            eventlog.Event = eventname.Trim();
            eventlog.FoursquareID = foursquareid.Trim();
            eventlog.PlaceName = placename.Trim();
            eventlog.Action = action.Trim();
            eventlog.Desc = desc.Trim();
            eventlog.LogTime = logtime.Trim();
            batchOperationLog.Insert(eventlog);
            if (batchOperationLog.Count != 0)
                tableEvent.ExecuteBatch(batchOperationLog);

            batchOperationLog = null;

        }

        /// Log Event
        /// </summary>
        /// <param name="eventname"></param>
        /// <param name="foursquareid"></param>
        /// <param name="placename"></param>
        /// <param name="action"></param>
        /// <param name="desc"></param>
        /// <param name="logtime"></param>
        private void LogHistory(string socialmediatype, string foursquareid, string placename, string count, string logtime)
        {
            string notAvailable = "Not Available".Trim();
            History historyLog = null;
            TableBatchOperation batchOperationLog = new TableBatchOperation();
            // Create a new Instagram entity.
            Guid guid = Guid.NewGuid();
            historyLog = new History(guid, socialmediatype);

            if (foursquareid != null)
                historyLog.FoursquareID = foursquareid.Trim();
            else
                historyLog.FoursquareID = notAvailable;

            if (placename != null)
                historyLog.PlaceName = placename.Trim();
            else
                historyLog.PlaceName = notAvailable;

            historyLog.MediaCount = count.Trim();

            historyLog.LogTime = DateTimeOffset.UtcNow.UtcDateTime.ToString().Trim();
            batchOperationLog.Insert(historyLog);
            if (batchOperationLog.Count != 0)
                tableHistory.ExecuteBatch(batchOperationLog);

            batchOperationLog = null;

        }
        /// <summary>
        /// Get Instagram Images
        /// </summary>
        /// <param name="fourSquareId"></param>
        /// <returns></returns>
        private List<String> GetMediaIds(string fourSquareId)
        {

            List<String> mediaIds = new List<String>();
            // Create the table client.
            tableClientInstagram = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            tableInstagram = tableClientInstagram.GetTableReference("InstagramLocationPhotosTableVer1");
            tableInstagram.CreateIfNotExists();
            TableServiceContext tableServiceContext = tableClientInstagram.GetTableServiceContext();
            List<InstagramPhotoEntity> instagramMetadata = new List<InstagramPhotoEntity>();
            mediaIds = (from x in tableServiceContext.CreateQuery<InstagramPhotoEntity>("InstagramLocationPhotosTableVer1")
                        where x.PartitionKey == fourSquareId.ToString().Trim()

                        select x).AsEnumerable().Select(t => t.MediaID).ToList();

            return mediaIds;
        }
        /// <summary>
        /// Get Foursquare venues photos entities
        /// </summary>
        /// <param name="fourSquareId"></param>
        /// <returns></returns>
        private List<String> GetPhotoIds(string fourSquareId)
        {
            List<String> photoId = new List<String>();
            try
            {
                // Create the table client.
                tableClientFoursquarePhoto = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableFoursquarePhoto = tableClientFoursquarePhoto.GetTableReference("FoursquareVenuesPhotoTableVer3");
                tableFoursquarePhoto.CreateIfNotExists();
                TableServiceContext tableServiceContext = tableClientFoursquarePhoto.GetTableServiceContext();

                photoId = (from x in tableServiceContext.CreateQuery<FoursqaureVenuePhotoEntities>("FoursquareVenuesPhotoTableVer3")
                           where x.PartitionKey == fourSquareId.ToString().Trim()
                           select x).AsEnumerable().Select(t => t.PhotoId).ToList();

            }
            catch (StorageException ex)
            {
                //queue.Clear();

                Trace.TraceError(ex.ToString());
                throw ex;
            }
            return photoId;
        }

        /// <summary>
        /// Get Foursquare venues Tips entities
        /// </summary>
        /// <param name="fourSquareId"></param>
        /// <returns></returns>
        private List<String> GetTipIds(string fourSquareId)
        {
            List<String> tipId = new List<String>();

            try
            {
                // Create the table client.
                tableClientFoursquareTip = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                tableFoursquareTip = tableClientFoursquareTip.GetTableReference("FoursquareVenuesTipTableVer3");
                tableFoursquareTip.CreateIfNotExists();
                TableServiceContext tableServiceContext = tableClientFoursquareTip.GetTableServiceContext();

                tipId = (from x in tableServiceContext.CreateQuery<FoursqaureVenueTipEntities>("FoursquareVenuesTipTableVer3")
                         where x.PartitionKey == fourSquareId.ToString().Trim()

                         select x).AsEnumerable().Select(t => t.TipId).ToList();
            }
            catch (StorageException ex)
            {

                Trace.TraceInformation("Something went wrong");
                Trace.TraceError(ex.ToString());
                throw ex;
            }
            return tipId;
        }
    }
}
