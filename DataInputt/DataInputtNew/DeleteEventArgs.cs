using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInputt
{
    class DeleteEventArgs : EventArgs
    {
        public object Object { get; set; }
        public DeleteEventArgs(object deleteObject)
        {
            this.Object = deleteObject;
        }
    }
}
