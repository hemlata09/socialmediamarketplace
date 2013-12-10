using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batchprocess
{
    class EventLog: TableEntity
    {
        public EventLog(Guid guid, string eventname)
        {
            this.PartitionKey = eventname;
            this.RowKey = guid.ToString();
        }

        public EventLog() { }
        public string Event { get; set; }
        public string FoursquareID { get; set; }
        public string PlaceName { get; set; }
        public string Action { get; set; }
        public string Desc { get; set; }
        public string LogTime { get; set; }

    }
}
