using DataInputt.DataInputServiceReference;
using DataInputt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInputt
{
    static class TimeRepo
    {
        public static List<Time> Times { get; set; }
        public delegate void TimesCollectionImportedEventHandler(object sender, EventArgs e);
        public static event TimesCollectionImportedEventHandler TimesCollectionImported;
        public static void OnTimesCollectionImport()
        {
            TimesCollectionImported(TimeRepo.Times, new EventArgs());
        }
    }
}
