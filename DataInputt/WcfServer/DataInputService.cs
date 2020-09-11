using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DataInputService : IDataInputService
    {
        public static List<InternalUser> Users = new List<InternalUser>();
        public static List<Time> Times = new List<Time>();

        public int CreateUser(User user)
        {
            var internalUser = new InternalUser(user);
            Users.Add(internalUser);

            return internalUser.uId;
        }

        public int Login(User user)
        {
            var internalUsers = Users.FindAll(x => x.Name == user.Name);
            if (internalUsers.Count == 0)
            {
                CalculateEarnings();
                return CreateUser(user);
            }
                

            foreach(var internalUser in internalUsers)
                if (user.Passwort == internalUser.Passwort)
                {
                 CalculateEarnings();
                    return internalUser.uId;
                }

            CalculateEarnings();

            return CreateUser(user);
        }

        public List<Time> GetTimes(int userId)
        {
            return Times.FindAll(x => x.uId == userId);
        }

        public void AddTime(Time time, int userId)
        {
            var existingTime = Times.Find(x => x.Id == time.Id);
            if (existingTime == null)
                Times.Add(time);
            else if (userId == -1)
                Times.Remove(existingTime);
            else
            {
                Times.Remove(existingTime);
                Times.Add(time);
            }

            CalculateEarnings();
        }

        public List<string> Projects()
        {
            return new List<string> { "Projekt 1", "Projekt 2", "Projekt 3", "Projekt 4", "Projekt 5" };
        }

        private void CalculateEarnings()
        {
            var result = new ConcurrentDictionary<int, decimal>();
            var callback = OperationContext.Current.GetCallbackChannel<IDataCallback>();

            for (int i = 0; i < Times.Count; i++)
            {
                var t = Times[i];
                if (result.ContainsKey(t.uId) == true)
                {
                    result[t.uId] += Convert.ToDecimal((t.End - t.Start).TotalHours);
                }
                else
                {
                    result.TryAdd(t.uId, Convert.ToDecimal((t.End - t.Start).TotalHours));
                }
            }

            foreach (var u in Users)
            {
                if (result.ContainsKey(u.uId))
                    result[u.uId] = result[u.uId] * 120;
            }

            callback.EarningsCalculated(result);
        }
    }

    public class InternalUser : User
    {
        public int uId { get; private set; }

        public InternalUser(User user)
        {
            uId = GetHashCode();
            Name = user.Name;
            Passwort = user.Passwort;
        }
    }
}
