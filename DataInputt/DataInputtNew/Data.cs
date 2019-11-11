using System.Collections.Generic;
using DataInputt.Models;

namespace DataInputt
{
    internal class Data
    {
        public bool TryImportMedia(string filename, out IEnumerable<Medium> result)
        {
            return new CsvImporter().TryImportMedia(filename, out result);
        }
    }
}