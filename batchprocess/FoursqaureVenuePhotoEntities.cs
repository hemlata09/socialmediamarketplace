using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batchprocess
{
    class FoursqaureVenuePhotoEntities: TableEntity
    {
        public FoursqaureVenuePhotoEntities(Guid guid, string fourSquareId)
        {
            this.PartitionKey = fourSquareId;
            this.RowKey = guid.ToString();
        }

        public FoursqaureVenuePhotoEntities() { }

        public string FoursquareID { get; set; }

       
        public string PhotoId { get; set; }
        public string PhotoCreatedAt { get; set; }
        public string PhotoPrefix { get; set; }
        public string PhotoSuffix { get; set; }
        public string PhotoHeight { get; set; }
        public string PhotoWidth { get; set; }
        public string PhotoSource { get; set; }
        public string PhotoVenue { get; set; }
        public string PhotoTip { get; set; }
        public string PhotoUploadedBy { get; set; }
    }
}
