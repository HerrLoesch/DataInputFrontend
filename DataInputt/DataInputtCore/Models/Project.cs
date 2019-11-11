using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInputt.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Abstract { get; set; }
        public string Position { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool UntilToday { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public string[] Tasks 
        {
            get { return TasksAsString.Split(',').ToArray(); }
            set
            {
                TasksAsString = String.Join(",", value);
            }
        }
        [NotMapped]
        public string[] Tools 
        {
            get { return ToolsAsString.Split(',').ToArray(); }
            set
            {
                ToolsAsString = String.Join(",", value);
            }
        }
        public string Sector { get; set; }

        public string TasksAsString { get; set; }
        public string ToolsAsString { get; set; }
    }
}
