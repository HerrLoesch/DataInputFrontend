using DataInputt.ZeitService.Api;
using System;
using System.Collections.Generic;

namespace DataInputt.ZeitService
{
    public interface IDataInputService
    {
        int CreateUser(User user);

        int Login(User user);

        List<Time> GetTimes(int? userId);

        void AddTime(Time time, int? userId);

        List<string> Projects();

        decimal CalculateEarnings(int? id);
    }

    public class DataInputService : ZeitServiceClient, IDataInputService
    {
        
    }
}
