using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInputt
{
    class OldPublicationData
    {
            public string[] types { get; set; }

            public OldPublicationData()
            {
                types = new[] { "Workshop", "Vortrag", "Artikel", "Videotraining", "Usergroup", "Buch" };
            }

            public List<OldPublication> Publications { get; set; }
    }
}
