using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DataInputt.Models;

namespace DataInputt
{
    internal class CsvImporter
    {
        public bool TryImportMedia(string filename, out IEnumerable<Medium> result)
        {
            result = new List<Medium>();

            if (string.IsNullOrWhiteSpace(filename))
            {
                return false;
            }

            if (File.Exists(filename) == false)
            {
                return false;
            }

            var lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                var colums = line.Split(',');
                if (colums.Length != 4)
                {
                    continue;
                }

                var medium = new Medium();

                if (string.IsNullOrEmpty(colums[0]) == false &&
                    int.TryParse(colums[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                {
                    medium.Id = id;
                }

                if (string.IsNullOrEmpty(colums[1]) == false)
                {
                    medium.Name = colums[1];
                }

                if (string.IsNullOrEmpty(colums[2]) == false &&
                    Uri.TryCreate(colums[2], UriKind.Absolute, out var uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                    medium.Link = colums[2];
                }

                if (string.IsNullOrEmpty(colums[3]) == false &&
                    int.TryParse(colums[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
                {
                    medium.Id = id;
                }

                ((List<Medium>) result).Add(medium);
            }

            return true;
        }
    }
}