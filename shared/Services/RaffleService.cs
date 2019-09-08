using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private List<RaffleWinner> RaffleWinners {  get; set; }
        public bool IsInitialized { get; set; }

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
            _logger.LogInformation("Initializing the Raffle Service");
            if (LatestRaffle == null)
            {
                LatestRaffle = await _storageUpdater.GetLatestRaffle();
            }

            if (!_prizeService.IsInitialized)
            {
                await _prizeService.InitializeService();
            }
            IsInitialized = true;
            _logger.LogInformation("Initialized the Raffle Service");
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

        public async Task StartRaffle(bool reuseEntrants = false)
        {
            // Determine if Raffle is in progress
            // If so throw an exception so the raffle is ended
            _logger.LogInformation("Starting a new raffle");
            if (LatestRaffle != null && LatestRaffle.State == RaffleState.Running)
            {
                _logger.LogWarning("There is a Raffle in progress. Stop that raffle first");
                await Task.CompletedTask;
            }
            else
            {
                // Select re-entrants to the new raffle
                var prize = await SelectRafflePrize();
                LatestRaffle = new Raffle
                {
                    Name = $"Raffle-{DateTime.UtcNow}",
                    State = RaffleState.Running,
                    Entries = LatestRaffle?.Entries?.Where(x => !x.IsWinner).ToList() ?? new List<RaffleEntry>(),
                    Prize = prize
                };
                var doc = (await _storageUpdater.CreateRaffle(LatestRaffle.Name,
                    LatestRaffle)) as DocumentResource;

                LatestRaffle.Sid = doc?.Sid;
                if (reuseEntrants)
                {
                    await NotifyRaffleReEntrants();
                }

                _notificationSid = string.Empty;
            }
            _logger.LogInformation("Started a new raffle");
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
            if (!string.IsNullOrEmpty(_notificationSid) || LatestRaffle.Entries.Any(entry => entry.IsWinner))
            {
                return _notificationSid ?? "";
            }

            var rand = new Random().Next(0, LatestRaffle.Entries.Count);
            var selected = LatestRaffle.Entries[rand];
            selected.IsWinner = true;
            LatestRaffle.State = RaffleState.NotRunning;
            LatestRaffle.Prize.Quantity--;

            await _storageUpdater.UpdateRaffle(
               LatestRaffle);
            return await NotifyRaffleWinner(selected);
        }

        public async Task<bool> ContainsPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentNullException(nameof(phoneNumber));
            }

            return await Task.FromResult(LatestRaffle.Entries.Any(entry => string.Equals(entry.PhoneNumber, phoneNumber, StringComparison.CurrentCultureIgnoreCase)));
        }

        public async Task<IEnumerable<RaffleWinner>> GetPreviousRaffleWinners()
        {
            if (RaffleWinners != null)
            {
                return RaffleWinners;
            }

            var raffles = (await _storageUpdater.GetRaffles()).ToList();

            RaffleWinners =
                (from raffle in raffles
                 from entry in raffle.Entries
                 where entry.IsWinner
                 select new RaffleWinner
                 {
                     RaffleName = raffle.Name,
                     MessageSid = entry.MessageSid,
                     PrizeName = raffle.Prize.Name
                 }).ToList();

            return RaffleWinners;
        }

        private async Task<string> NotifyRaffleWinner(RaffleEntry entry)
        {
            var msg = await MessageResource.FetchAsync(
                entry.MessageSid,
                _accountSid);

            var response = await MessageResource.CreateAsync(
                msg.From,
                from: msg.To,
                body: @"You've won the latest raffle. Visit the Twilio booth to pick up your prize. If you can't email corey@twilio.com");
            _notificationSid = response.Sid;

            using (var httpClient = new HttpClient())
            {
                var data = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("winner",msg.From.ToString()),
                    new KeyValuePair<string,string>("raffleSid", LatestRaffle.Sid),
                    new KeyValuePair<string, string>("prize", LatestRaffle.Prize.Name)
                });

                var httpResponse = await httpClient.PostAsync("https://hooks.zapier.com/hooks/catch/3191324/o3o1bt1/", data);
                httpResponse.EnsureSuccessStatusCode();
            }

            return _notificationSid;
        }

        private async Task<RafflePrize> SelectRafflePrize()
        {
            _logger.LogInformation("Selecting a Raffle Prize");
            var prize = await _prizeService.SelectPrize();
            _logger.LogInformation("Selected a Raffle prize");
            return prize;
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
