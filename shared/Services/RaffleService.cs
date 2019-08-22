using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Sync.V1.Service;
using Twilio.Types;

namespace shared.Services
{
    public class RaffleService : IRaffleService
    {
        private string _notificationSid;
        private readonly string _accountSid;
        private readonly IRaffleStorageService _storageUpdater;
        private readonly IPrizeService _prizeService;
        private readonly ILogger _logger;

        public Raffle LatestRaffle { get; set; }

        public RaffleService(IRaffleStorageService storageUpdater, IPrizeService prizeService, IConfiguration config, ILogger<RaffleService> logger)
        {
            _storageUpdater = storageUpdater;
            _prizeService = prizeService;
            _logger = logger;
            _accountSid = config[Constants.ACCOUNT_SID_PATH];
        }

        public async Task InitializeService()
        {
            LatestRaffle = await _storageUpdater.GetLatestRaffle();
        }

        public async Task<string> AddRaffleEntry(RaffleEntry entry)
        {
            if (LatestRaffle.State == RaffleState.NotRunning)
            {
                _logger.LogError("There is no raffle currently in progress.", LatestRaffle);
                throw new InvalidOperationException("Unable to add a raffle entry to the current raffle. The current raffle is not in progress");
            }

            _logger.LogInformation("Adding the latest raffle entry to the list of entries", entry);
            LatestRaffle.Entries.Add(entry);
            LatestRaffle.UpdatedDate = DateTime.UtcNow;

            await _storageUpdater.UpdateRaffle(LatestRaffle);
            _logger.LogInformation("Added the latest raffle entry to the list of entries", entry);
            return LatestRaffle.Sid;
        }

        public async Task StartRaffle()
        {
            // Determine if Raffle is in progress
            // If so throw an exception so the raffle is ended
            if (LatestRaffle.State == RaffleState.Running)
            {
                _logger.LogInformation("There is a Raffle in progress");
                await Task.CompletedTask;
            }

            // Select re-entrants to the new raffle
            var prize = await SelectRafflePrize();
            LatestRaffle = new Raffle
            {
                Name = $"Raffle-{DateTime.UtcNow}",
                State = RaffleState.Running,
                Entries = LatestRaffle.Entries,
                Prize = prize
            };
            var doc = (await _storageUpdater.CreateRaffle(LatestRaffle.Name,
                LatestRaffle)) as DocumentResource;

            LatestRaffle.Sid = doc?.Sid;
            await NotifyRaffleReEntrants();
            _notificationSid = string.Empty;
        }

        public async Task ClearRaffles()
        {
            await _storageUpdater.ClearStorage();
        }

        public async Task StopRaffle()
        {
            LatestRaffle.Current = false;
            LatestRaffle.State = RaffleState.NotRunning;

            await _storageUpdater.UpdateRaffle(LatestRaffle);
        }

        public async Task<string> SelectRaffleWinner()
        {
            if (!string.IsNullOrEmpty(_notificationSid))
            {
                return _notificationSid;
            }

            var rand = new Random().Next(0, LatestRaffle.Entries.Count);
            var selected = LatestRaffle.Entries[rand];
            selected.IsWinner = true;
            LatestRaffle.State = RaffleState.NotRunning;
            LatestRaffle.Prize.Quantity = 0;

            await _storageUpdater.UpdateRaffle(
               LatestRaffle);
            return await NotifyRaffleWinner(selected.MessageSid);
        }

        private async Task<string> NotifyRaffleWinner(string messageSid)
        {
            var msg = await MessageResource.FetchAsync(
                messageSid,
                _accountSid);

            var response = await MessageResource.CreateAsync(
                msg.From,
                from: msg.To,
                body: @"You've won the latest raffle. Visit the Twilio booth to pick up your prize. If you can't email corey@twilio.com");
            _notificationSid = response.Sid;

            return _notificationSid;
        }

        private async Task<RafflePrize> SelectRafflePrize()
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

        private async Task NotifyRaffleReEntrants()
        {
            // TODO: Replace individual SMS messages with Notification Service
            await RemoveDuplicateEntries();
            await Task.Run(() => Parallel.ForEach(LatestRaffle.Entries, async entry =>
            {
                var resource = await MessageResource.FetchAsync(entry.MessageSid);
                var url = new Uri($"https://corey.ngrok.io/images/{LatestRaffle.Prize.ImageUrl}");
                await MessageResource.CreateAsync(
                    resource.From,
                    from: new PhoneNumber(resource.To),
                    body: $"Greetings from Twilio. Looks like you didn't win the last raffle but don't worry. We've re-entered you into the next one to win this prize: {LatestRaffle.Prize.Name}. Good luck and keep an eye out to see if you've won the prize",
                    mediaUrl: new List<Uri> { url });
            }));
        }

        private async Task RemoveDuplicateEntries()
        {
            var flaggedEntries = new List<RaffleEntry>();

            foreach (var entry in LatestRaffle.Entries)
            {
                var recipient = (await MessageResource.FetchAsync(
                    entry.MessageSid)).From;
                if (flaggedEntries.Any(num => string.Equals(num.ToString(), recipient.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    flaggedEntries.Add(entry);
                }
            }

            LatestRaffle.Entries = LatestRaffle.Entries.Except(flaggedEntries).ToList();
        }
    }
}
