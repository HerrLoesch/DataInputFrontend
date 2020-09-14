using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataInputt.ZeitService.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Controllers
{
    [Route("api/zeitservice")]
    [ApiController]
    public class ZeitServiceController : ControllerBase
    {
        public static List<InternalUser> Users = new List<InternalUser>();
        public static List<Time> Times = new List<Time>();

        private readonly IHubContext<EarningsHub> earningsHubContext;
        public ZeitServiceController(IHubContext<EarningsHub> earningsHubContext)
        {
            this.earningsHubContext = earningsHubContext;
        }

        [HttpPost("CreateUser")]
        [ProducesResponseType(typeof(int), 200)]
        public ActionResult<int> CreateUser(User user)
        {
            var internalUser = new InternalUser(user);
            Users.Add(internalUser);

            return internalUser.uId;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(int), 200)]
        public ActionResult<int> Login(User user)
        {
            var internalUsers = Users.FindAll(x => x.Name == user.Name);
            if (internalUsers.Count == 0)
            {
                CalculateEarnings();
                return CreateUser(user);
            }


            foreach (var internalUser in internalUsers)
                if (user.Passwort == internalUser.Passwort)
                {
                    CalculateEarnings();
                    return internalUser.uId;
                }

            CalculateEarnings();

            return CreateUser(user);
        }

        [HttpGet("Times")]
        [ProducesResponseType(typeof(List<Time>), 200)]
        public ActionResult<List<Time>> GetTimes(int userId)
        {
            return Times.FindAll(x => x.uId == userId);
        }

        [HttpPost("AddTime")]
        [ProducesResponseType(200)]
        public ActionResult AddTime(Time time, int userId)
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

            return Ok();
        }

        [HttpGet("Projects")]
        [ProducesResponseType(typeof(List<string>), 200)]
        public ActionResult<List<string>> Projects()
        {
            return new List<string> { "Projekt 1", "Projekt 2", "Projekt 3", "Projekt 4", "Projekt 5" };
        }

        private void CalculateEarnings()
        {
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

            foreach (var u in Users)
            {
                if (result.ContainsKey(u.uId))
                    result[u.uId] = result[u.uId] * 120;
            }

            Task.Run(async () => await earningsHubContext.Clients.All.SendAsync("EarningsCalculated", result)).GetAwaiter().GetResult();
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
