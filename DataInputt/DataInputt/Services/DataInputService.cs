using DataInputt.ZeitService.Api;
using System.Collections.Generic;

namespace DataInputt.Services
{
    public class DataInputService : IDataInputService
    {
        public DataInputService()
        {

        }

        public void AddTime(Time time, int userId)
        {
            
        }

        public int CreateUser(User user)
        {
            return 0;   
        }

        public List<Time> GetTimes(int userId)
        {
            return new List<Time>();
        }

        public int Login(User user)
        {
            return 0;
        }

        public List<string> Projects()
        {
            return new List<string>();
        }
    }
}
