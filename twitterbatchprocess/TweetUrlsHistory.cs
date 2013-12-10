﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitterbatchprocess
{
    class TweetUrlsHistory: TableEntity
    {
        public TweetUrlsHistory(string tickCount, string TwitterScreename)
        {
            this.PartitionKey = TwitterScreename;
            this.RowKey = tickCount.ToString();
        }

        public TweetUrlsHistory() { }

        public string FoursquareId { get; set; }
        public string tweetType { get; set; }
        public string TwitterScreename { get; set; }
        public string TwitterId { get; set; }
        public string PlaceName { get; set; }
        public string NewUrlsCount { get; set; }
        public string TweetId { get; set; }
        public string TweetUrls { get; set; }
        public string TotalTweetUrls { get; set; }
        public string LogTime { get; set; }

    }
}
