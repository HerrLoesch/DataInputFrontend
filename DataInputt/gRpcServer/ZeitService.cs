using DataInputt.ZeitService.Api;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gRpcServer
{
    public class ZeitService : DataInputt.ZeitService.Api.ZeitService.ZeitServiceBase
    {
        public static List<InternalUser> Users = new List<InternalUser>();
        public static List<Time> Times = new List<Time>();

        public override Task<UserResponse> CreateUser(UserDto request, ServerCallContext context)
        {
            var internalUser = new InternalUser(request.ToUser());
            Users.Add(internalUser);

            return Task.FromResult(new UserResponse { Id = internalUser.uId });
        }

        public override Task<UserResponse> Login(UserDto request, ServerCallContext context)
        {
            var internalUsers = Users.FindAll(x => x.Name == request.Name);
            if (internalUsers.Count == 0)
                return CreateUser(request, context);

            foreach (var internalUser in internalUsers)
                if (request.Passwort == internalUser.Passwort)
                    return Task.FromResult(new UserResponse { Id = internalUser.uId });

            return CreateUser(request, context);
        }

        public override Task<TimeCollection> GetTimes(GetTimesRequest request, ServerCallContext context)
        {
            var times = Times.FindAll(x => x.uId == request.UserId);
            var timesDto = times.Select(t => t.ToTimeDto());

            var response = new TimeCollection();
            response.Times.AddRange(timesDto);

            return Task.FromResult(response);
        }

        public override Task<Empty> AddTime(AddTimeRequest request, ServerCallContext context)
        {
            var time = request.Time.ToTime();
            var existingTime = Times.Find(x => x.Id == time.Id);
            if (existingTime == null)
                Times.Add(time);
            else if (request.UserId == -1)
                Times.Remove(existingTime);
            else
            {
                Times.Remove(existingTime);
                Times.Add(time);
            }

            return Task.FromResult(new Empty());
        }

        public override Task<ProjectCollection> Projects(Empty request, ServerCallContext context)
        {
            var response = new ProjectCollection();
            response.Projects.AddRange(new[] { "Projekt 1", "Projekt 2", "Projekt 3", "Projekt 4", "Projekt 5" });
            return Task.FromResult(response);
        }

        public override Task<CalculateEarningResponse> CalculateEarnings(CalculateEarningRequest request, ServerCallContext context)
        {
            float earning = 0;
            var result = new ConcurrentDictionary<int, decimal>();

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

            if (result.ContainsKey(request.UserId))
            {
                earning = (float)result[request.UserId] * 120;
            }

            return Task.FromResult(new CalculateEarningResponse { Earning = earning });
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
}
