using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitterbatchprocess
{
    class TweetHistory: TableEntity
    {
        public TweetHistory(string tickCount, string TwitterScreename)
        {
            this.PartitionKey = TwitterScreename;
            this.RowKey = tickCount.ToString();
        }

        public TweetHistory() { }

        public string FoursquareId { get; set; }
        public string tweetType { get; set; }

        public string TwitterId { get; set; }
        public string PlaceName { get; set; }
        public string TweetCount { get; set; }
        public string TweetId { get; set; }
        public string TotalTweetCount { get; set; }
        public string LogTime { get; set; }

    }
}
