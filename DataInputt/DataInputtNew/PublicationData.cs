using System.Collections.Generic;

namespace DataInputt
{
    public class PublicationData
    {
        public string[] types { get; set; }

        public PublicationData()
        {
            types = new[] {"Workshop", "Vortrag", "Artikel", "Videotraining", "Usergroup", "Buch"};
        }

        public List<Publication> Publications { get; set; }
    }
}