using DataInputt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInputt
{
    static class MediaRepo
    {
        public static List<Medium> Media { get; set; }
        public delegate void MediaCollectionImportedEventHandler(object sender, EventArgs e);
        public static event MediaCollectionImportedEventHandler MediaCollectionImported;
        public static void OnMediaCollectionImport()
        {
            MediaCollectionImported(MediaRepo.Media, new EventArgs());
        }
    }
}
