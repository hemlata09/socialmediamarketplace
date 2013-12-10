using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batchprocess
{
   public class FoursquareEntities: TableEntity
    {
        public FoursquareEntities(Guid guid, string fourSquareId)
        {
            this.PartitionKey = fourSquareId;
            this.RowKey = guid.ToString();
        }

        public FoursquareEntities() { }

        public string FoursquareID { get; set; }
        public string PlaceName { get; set; }
        public string Url { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CanonicalUrl { get; set; }
        public string ContactNo { get; set; }
        public string TwitterScreenName { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public string CheckinsCount { get; set; }
        public string UserCount { get; set; }
        public string TipCount { get; set; }
        public string LikesCount { get; set; }
        public string MenuType { get; set; }
        public string MenuUrl { get; set; }
        public string MenuMobileUrl { get; set; }

        public string VenuePageId { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }

        public string ShortUrl { get; set; }
        public string TimeZone { get; set; }
        public string PhraseText { get; set; }
        public string Rating { get; set; }

        //public string MenuUrl { get; set; }
       // public string MenuMobileUrl { get; set; }
    
    }
}
