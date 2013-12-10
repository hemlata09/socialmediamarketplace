using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitterbatchprocess
{
    class TwitterEntity: TableEntity
    {
        public TwitterEntity(Guid guid, string locationName)
        {
            this.PartitionKey = locationName;
            this.RowKey = guid.ToString();
        }

        public TwitterEntity() { }
        public string TwitterId { get; set; }
        
        public string TwitterScreenName { get; set; }
        public string TwitterName { get; set; }
        public string TwitterDescription { get; set; }
        public string TwitterFollowerCount { get; set; }
        public string TwitterFriendsCount { get; set; }
        public string TwitterFavouritesCount { get; set; }
        public string TwitterListedCount { get; set; }
        public string TwitterLocation { get; set; }
        public string TwitterStatus { get; set; }
        public string TwitterStatusCount { get; set; }
        public string TwitterTimeZone { get; set; }
        public string TwitterUrl { get; set; }
        public string TwitterProfileImageUrl { get; set; }
        public string FollowerList { get; set; }
        
        //public string favouriteTweetComments { get; set; }
        //public string favouriteTweetListOnRestaurant { get; set; }
        /*public string LatestTweetStatus { get; set; }
        public string TweetHasTag { get; set; }
        public string TweetMedia { get; set; }
        public string TweetText { get; set; }*/
    
    }
    }

