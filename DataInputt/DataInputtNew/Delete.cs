using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataInputt
{
    class Delete
    {
        private static Delete instance;
        public delegate void EventHandler(object sender, DeleteEventArgs e);
        public event EventHandler SomethingDeleted;
        public void OnDeleteSomething(object sender, object deleteObject)
        {
            SomethingDeleted(sender, new DeleteEventArgs(deleteObject));
        }

        private Delete() {}
        public static Delete GetInstance()
        {
            if (instance == null)
            {
                instance = new Delete();
            }
            return instance;
        }
    }
}
