﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitterbatchprocess
{
    class TwitterUrlEntities: TableEntity
    {
        public TwitterUrlEntities(Guid guid, string screenName)
        {
            this.PartitionKey = screenName;
            this.RowKey = guid.ToString();
        }

        public TwitterUrlEntities() { }
      
        public string TwitterId { get; set; }

        public string TweetId { get; set; }
        public string TweetCreatedTimestamp { get; set; }
        public string TweetText { get; set; }
        public string TweetSource { get; set; }

        public string TweetEntityHashTagCount { get; set; }
        public string TweetEntityMediaCount { get; set; }
        public string TweetEntityUrlCount { get; set; }
        public string TweetEntityUserMentionCount { get; set; }
        public string TweetRetweetCount { get; set; }
        public string TweetExpandedUrl{ get; set; }
        public string TweetDisplayUrl { get; set; }

    }
}
