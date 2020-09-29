using DataInputt.ZeitService.Api;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataInputt.Services
{
    public class DataInputService : IDataInputService
    {
        private readonly ZeitService.Api.ZeitService.ZeitServiceClient _grpcClient;

        public DataInputService()
        {
            const int port = 9000;
            string host = Environment.MachineName;

            var channel = new Channel(host, port, ChannelCredentials.Insecure);
            _grpcClient = new ZeitService.Api.ZeitService.ZeitServiceClient(channel);
        }

        public void AddTime(Time time, int userId)
        {
            _grpcClient.AddTime(new AddTimeRequest { UserId = userId, Time = time.ToTimeDto() });
        }

        public int CreateUser(User user)
        {
            return _grpcClient.CreateUser(user.ToUserDto()).Id;
        }

        public List<Time> GetTimes(int userId)
        {
            var response = _grpcClient.GetTimes(new ZeitService.Api.GetTimesRequest { UserId = userId });
            return response.Times.Select(t => t.ToTime()).ToList();
        }

        public int Login(User user)
        {
            return _grpcClient.Login(user.ToUserDto()).Id;
        }

        public List<string> Projects()
        {
            return _grpcClient.Projects(new Google.Protobuf.WellKnownTypes.Empty()).Projects.ToList();
        }
    }
}
