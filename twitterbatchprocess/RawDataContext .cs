using Microsoft.WindowsAzure.Storage.Auth;

using Microsoft.WindowsAzure.StorageClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace twitterbatchprocess
{
    class RawDataContext : TableServiceContext
    {
        public RawDataContext(string baseAddress, Microsoft.WindowsAzure.StorageCredentials credentials)
            : base(baseAddress, credentials)
        {
        }

        public CloudTableQuery<FoursquareEntities> FoursquareEntities
        {
            get
            {
                return this.CreateQuery<FoursquareEntities>("FoursquareVenuesTable").AsTableServiceQuery();
            }
        }


        public CloudTableQuery<TwitterTweetEntities> TweetEntities
        {
            get
            {
                return this.CreateQuery<TwitterTweetEntities>("TwitterTweetMetaData").AsTableServiceQuery();
            }
        }
        //public CloudTableQuery<FoursqaureVenuePhotoEntities> FoursqaurePhotoEntities
        //{
        //    get
        //    {
        //        return this.CreateQuery<FoursqaureVenuePhotoEntities>("FoursquareVenuesPhotoTableVer2").AsTableServiceQuery();
        //    }
        //}

        //public CloudTableQuery<FoursqaureVenueTipEntities> FoursquareTipEntities
        //{
        //    get
        //    {
        //        return this.CreateQuery<FoursqaureVenueTipEntities>("FoursquareVenuesTipTableVer2").AsTableServiceQuery();
        //    }
        //}
    }
}
