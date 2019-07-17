using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Sync.V1.Service;

namespace shared.Services
{
    public class TwilioSyncService : IStorageService
    {
        protected string AccountSid { get; set; }
        protected string ServiceSid { get; set; }
        protected Raffle CurrentRaffle { get; set; }

        private readonly string _authToken;
        private readonly IConfiguration _config;
        private readonly Task _initializingTask;
        private string _notificationSid;
        public TwilioSyncService(IConfiguration config)
        {
            _config = config;
            AccountSid = _config[Constants.ACCOUNTSIDPATH];
            ServiceSid = _config[Constants.SERVICESIDPATH];
            // TODO: Ensure AuthToken can be grabbed from multiple places
            _authToken = _config[Constants.AUTHTOKENPATH];
            _initializingTask = StartConnection();
            Task.WaitAll();
        }

        protected async Task StartConnection()
        {
            TwilioClient.Init(AccountSid, _authToken);
            CurrentRaffle = await GetCurrentRaffleAsync();
        }

        public async Task<string> AddRaffleEntryAsync(RaffleEntry entry)
        {
            if (CurrentRaffle.State == RaffleState.NotRunning)
            {
                await StartRaffleAsync();
            }

            CurrentRaffle.Entries.Add(entry);
            await UpdateSyncService();

            return CurrentRaffle.Sid;
        }

        private async Task UpdateSyncService()
        {
            CurrentRaffle.UpdatedDate = DateTime.UtcNow;
            var updated = await DocumentResource.UpdateAsync(
                            pathServiceSid: ServiceSid,
                            pathSid: CurrentRaffle.Sid,
                            data: JsonConvert.SerializeObject(CurrentRaffle, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }

        public async Task<IList<RaffleEntry>> GetRaffleEntriesAsync() => (CurrentRaffle != null) ? await Task.FromResult(CurrentRaffle.Entries) : (await GetCurrentRaffleAsync()).Entries;

        public async Task<string> StartRaffleAsync()
        {
            // Determine if Raffle is in progress
            // If so end the raffle and start a new raffle
            if (CurrentRaffle != null && CurrentRaffle.State == RaffleState.Running)
            {
                CurrentRaffle.Current = false;
                await EndRaffleAsync();
            }
            CurrentRaffle = new Raffle
            {
                Name = $"Raffle-{DateTime.UtcNow}",
                State = RaffleState.Running
            };

            var prize = await SelectRafflePrizeAsync();
            CurrentRaffle.Prize = prize;

            var doc = await DocumentResource.CreateAsync(
                pathServiceSid: ServiceSid,
                uniqueName: CurrentRaffle.Name,
                data: JsonConvert.SerializeObject(CurrentRaffle, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            CurrentRaffle.Sid = doc.Sid;
            _notificationSid = String.Empty;
            return CurrentRaffle.Sid;
        }

        public async Task<string> EndRaffleAsync()
        {
            CurrentRaffle.State = RaffleState.NotRunning;

            await UpdateSyncService();

            return CurrentRaffle.Sid;
        }

        public async Task ClearRafflesAsync()
        {
            var sids = (await DocumentResource.ReadAsync(
                pathServiceSid: ServiceSid))
                .Select(result => result.Sid)
                .ToList();

            foreach (var sid in sids)
            {
                await DocumentResource.DeleteAsync(
                    pathServiceSid: ServiceSid,
                    pathSid: sid);
            }
        }

        public async Task<Raffle> GetCurrentRaffleAsync()
        {
            var documents = await DocumentResource.ReadAsync(
                 pathServiceSid: ServiceSid);

            CurrentRaffle =
                (from doc in documents
                 let temp = JObject.Parse(doc.Data.ToString())
                 where string.Equals(temp.GetValue("current", StringComparison.CurrentCultureIgnoreCase).ToString(), "true", StringComparison.CurrentCultureIgnoreCase)
                 select new Raffle
                 {
                     Name = doc.UniqueName,
                     Sid = doc.Sid,
                     Entries = temp.GetValue("entries", StringComparison.CurrentCultureIgnoreCase).Select(entry => JsonConvert.DeserializeObject<RaffleEntry>(entry.ToString())).ToList(),
                     CreatedDate = doc.DateCreated,
                     UpdatedDate = doc.DateUpdated,
                     State = (RaffleState)Enum.Parse(typeof(RaffleState), temp.GetValue("state", StringComparison.CurrentCultureIgnoreCase).ToString()),
                     Prize = new RafflePrize
                     {
                         Name = temp["prize"]["name"].ToString(),
                         ImageUrl = temp["prize"]["imageUrl"].ToString(),
                         Quantity = int.Parse(temp["prize"]["quantity"].ToString())
                     }
                 }).FirstOrDefault();

            if (CurrentRaffle == null)
            {
                await StartRaffleAsync();
            }

            return CurrentRaffle;
        }

        public async Task<string> StopRaffleAsync()
        {
            CurrentRaffle.Current = false;
            CurrentRaffle.State = RaffleState.NotRunning;

            await UpdateSyncService();

            return CurrentRaffle.Sid;
        }

        public async Task<string> SelectRaffleWinnerAsync()
        {
            if (!string.IsNullOrEmpty(_notificationSid))
            {
                return _notificationSid;
            }

            var rand = new Random().Next(0, CurrentRaffle.Entries.Count);
            var selected = CurrentRaffle.Entries[rand];
            selected.IsWinner = true;
            CurrentRaffle.State = RaffleState.NotRunning;
            CurrentRaffle.Prize.Quantity = 0;

            await UpdateSyncService();
            return await NotifyRaffleWinnerAsync(selected.MessageSid);
        }

        private async Task<string> NotifyRaffleWinnerAsync(string messageSid)
        {
            var msg = await MessageResource.FetchAsync(
                pathSid: messageSid,
                pathAccountSid: AccountSid);

            var response = await MessageResource.CreateAsync(
                to: msg.From,
                from: msg.To,
                body: @"You've won the latest raffle. Visit the Twilio booth to pick up your prize. If you can't email corey@twilio.com");
            _notificationSid = response.Sid;

            return _notificationSid;
        }

        private async Task<RafflePrize> SelectRafflePrizeAsync()
        {
            string response = String.Empty;
            using (var httpClient = new HttpClient())
            {
                response = await httpClient.GetStringAsync("https://black-bombay-6404.twil.io/assets/prizes.json");
            }

            if (String.IsNullOrEmpty(response))
            {
                return null;
            }

            var json = JObject.Parse(response);
            var prizeList = json["Raffle"].Children()
                .Select(token =>
                    new RafflePrize
                    {
                        Name = token["Prize"]["Name"].ToString(),
                        Quantity = int.Parse(token["Prize"]["Quantity"].ToString()),
                        ImageUrl = token["Prize"]["ImageUrl"].ToString()
                    })
                .Where(prize => prize.Quantity > 0).ToList();
            var selected = new Random().Next(prizeList.Count);
            return prizeList[selected];
        }
    }
}
