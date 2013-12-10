using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitterbatchprocess
{
    public class RawDataSource
    {

        private static Microsoft.WindowsAzure.CloudStorageAccount storageAccount;
        private RawDataContext context;


        public RawDataSource()
        {
            storageAccount = Microsoft.WindowsAzure.CloudStorageAccount.Parse(
    Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("StorageConnectionString"));


            this.context = new RawDataContext(storageAccount.TableEndpoint.AbsoluteUri, storageAccount.Credentials);
        }
        public List<FoursquareEntities> GetFoursquareEntities()
        {
            List<FoursquareEntities> foursquareEntities = new List<FoursquareEntities>();
            try
            {
                foursquareEntities = (from g in this.context.FoursquareEntities.Execute()
                                 select g).ToList();
            }
            catch (StorageException ex)
            {
                throw ex;
            }

            return foursquareEntities;

        }

        public List<TwitterTweetEntities> GetTweetEntities(string twitterScreenName)
        {
            List<TwitterTweetEntities> tweetEntities = new List<TwitterTweetEntities>();
            List<String> tweetId = new List<String>();
            try
            {
                tweetEntities = (from g in this.context.TweetEntities.Execute()
                                  where g.PartitionKey == twitterScreenName.ToString().Trim()
                                    select g).ToList();
                                      //select g).Take(5).ToList();
            }
            catch (StorageException ex)
            {
                throw ex;
            }

            return tweetEntities;

        }
      
        }


    }

