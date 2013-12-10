using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batchprocess
{
    class FoursqaureVenueTipEntities: TableEntity
    {
        public FoursqaureVenueTipEntities(Guid guid, string fourSquareId)
        {
            this.PartitionKey = fourSquareId;
            this.RowKey = guid.ToString();
        }

        public FoursqaureVenueTipEntities() { }

        public string FoursquareID { get; set; }
        public string PlaceName { get; set; }
       
        public string TipId { get; set; }
        public string TipText { get; set; }
        public string TipCreatedAt { get; set; }
        public string TipCanonicalUrl { get; set; }
        public string User{ get; set; }


    }
}
