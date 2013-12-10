using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitterbatchprocess
{
    class HashTagHistory: TableEntity
    {
        public HashTagHistory(string tickCount, string TwitterScreename)
        {
            this.PartitionKey = TwitterScreename;
            this.RowKey = tickCount.ToString();
        }

        public HashTagHistory() { }

        public string FoursquareId { get; set; }
        public string tweetType { get; set; }
        public string TwitterScreename { get; set; }
        public string TwitterId { get; set; }
        public string PlaceName { get; set; }
        public string NewHashTagCount { get; set; }
        public string TweetId { get; set; }
        public string Hashtags { get; set; }
        public string TotalHashTags { get; set; }
        public string LogTime { get; set; }

    }
}
