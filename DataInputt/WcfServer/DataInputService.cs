﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServer
{
    public class DataInputService : IDataInputService
    {
        public static List<InternalUser> Users { get; private set; }
        public static List<Time> Times { get; private set; }

        private DataInputService()
        {
            Users = new List<InternalUser>();
            Times = new List<Time>();
        }

        public int CreateUser(User user)
        {
            var internalUser = new InternalUser(user);
            Users.Add(internalUser);

            return internalUser.uId;
        }

        public int Login(User user)
        {
            var internalUser = Users.Find(x => x.Name == user.Name);
            if (internalUser == null)
                return CreateUser(user);

            if (user.Passwort == internalUser.Passwort)
                return internalUser.uId;
            else
                return CreateUser(user);
        }

        public List<Time> GetTimes(int userId)
        {
            return Times.FindAll(x => x.uId == userId);
        }

        public void AddTime(Time time, int userId)
        {
            Times.Add(time);
        }

        public List<string> Projects()
        {
            return new List<string> { "Projekt 1", "Projekt 2", "Projekt 3", "Projekt 4", "Projekt 5" };
        }

        public decimal CalculateEarnings(int id)
        {
            decimal hours = 0;

            var times = Times.FindAll(x => x.uId == id);
            if(times == null)
                return 0;

            foreach(var time in times)
                hours += Convert.ToDecimal((time.End - time.Start).TotalHours);

            return hours * 120;
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