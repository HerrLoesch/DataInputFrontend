using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInputt.Models
{
    static class PublisherRepo
    {
        public static List<Publisher> Publisher { get; set; }
        public delegate void PublisherCollectionImportedEventHandler(object sender, EventArgs e);
        public static event PublisherCollectionImportedEventHandler PublisherCollectionImported;
        public static void OnPublisherCollectionImport()
        {
            PublisherCollectionImported(PublisherRepo.Publisher, new EventArgs());
        }
    }
}
