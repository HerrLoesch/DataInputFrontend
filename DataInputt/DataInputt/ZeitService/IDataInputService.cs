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
    }

    public class DataInputService : ZeitServiceClient, IDataInputService
    {
        
    }

    public interface IDataCallback
    {
        void EarningsCalculated(IDictionary<int, decimal> earnings);
    }

    
}
