using System;
using System.Collections.Generic;

namespace DataInputt.ZeitService.Api
{
    public interface IDataInputService
    {
        int CreateUser(User user);

        int Login(User user);

        List<Time> GetTimes(int userId);

        void AddTime(Time time, int userId);

        List<string> Projects();

        decimal CalculateEarnings(int? id);
    }

    public class User
    {
        public string Name { get; set; }

        public string Passwort { get; set; }
    }

    public class Time
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string Project { get; set; }

        public int uId { get; set; }

        public int Id { get; set; }
    }
}
