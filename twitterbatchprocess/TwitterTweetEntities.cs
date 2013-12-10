using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitterbatchprocess
{
    public class TwitterTweetEntities : TableEntity
    {
        public TwitterTweetEntities(string tickCount, string screenName)
        {
            this.PartitionKey = screenName;
            this.RowKey = tickCount.ToString();
        }

        public TwitterTweetEntities() { }

        public string TweetId { get; set; }
        public string TweetCreatedTimestamp { get; set; }
        public string TweetCreatedTimestampTicks { get; set; }
        public string TweetCreatedDate { get; set; }
        public string TweetText { get; set; }
        public string TweetSource { get; set; }

        public string TweetEntityHashTagCount { get; set; }
        public string TweetEntityMediaCount { get; set; }
        public string TweetEntityUrlCount { get; set; }
        public string TweetEntityDetailUrl { get; set; }
        public string TweetEntityUserMentionCount { get; set; }
        public string TweetRetweetCount { get; set; }
        


    
    }
}
