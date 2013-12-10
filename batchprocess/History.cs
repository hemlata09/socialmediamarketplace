using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batchprocess
{
    class History: TableEntity
    {
        public History(Guid guid, string processedDate)
        {
            this.PartitionKey = processedDate;
            this.RowKey = guid.ToString();
        }

        public History() { }

        public string FoursquareID { get; set; }
        public string PlaceName { get; set; }
        public string MediaCount { get; set; }
        public string LogTime { get; set; }

    }
}
