using DataInputt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInputt
{
    static class ProjectRepo
    {
        public static List<Project> Projects { get; set; }
        public delegate void ProjectsCollectionImportedEventHandler(object sender, EventArgs e);
        public static event ProjectsCollectionImportedEventHandler ProjectCollectionImported;
        public static void OnProjectCollectionImport()
        {
            ProjectCollectionImported(ProjectRepo.Projects, new EventArgs());
        }
    }
}
