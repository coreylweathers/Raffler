using System;
using System.Linq;
using System.Threading.Tasks;
using Twilio.Rest.Sync.V1.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Twilio;
using Microsoft.Extensions.Configuration;
using shared.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Twilio.Clients;

namespace shared.Services
{
    public class TwilioRaffleStorageService : IRaffleStorageService
    {
        private readonly string _authToken;
        private readonly string _accountSid;
        private readonly string _serviceSid;
        private string _documentSid;
        private readonly ILogger _logger;

        public TwilioRaffleStorageService(IConfiguration config, ILogger<TwilioRaffleStorageService> logger)
        {
            _accountSid = config[Constants.ACCOUNT_SID_PATH];
            _authToken = config[Constants.AUTH_TOKEN_PATH];
            _serviceSid = config[Constants.SYNC_SERVICE_SID_PATH];
            _logger = logger;
            TwilioClient.Init(_accountSid, _authToken);
        }
        public async Task<object> CreateRaffle(string raffleName, object data)
        {
            _logger.LogInformation("Creating the raffle");
            var response = await DocumentResource.CreateAsync(
                 _serviceSid,
                 raffleName,
                 JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                 {
                     ContractResolver = new CamelCasePropertyNamesContractResolver()
                 }));
            _logger.LogInformation("Created the raffle");
            _documentSid = response.Sid;

            return response;
        }

        public async Task UpdateRaffle(Raffle data)
        {
            _logger.LogInformation("Updating the raffle");
            var response = await DocumentResource.UpdateAsync(
                 pathServiceSid: _serviceSid,
                 pathSid: _documentSid,
                 data: JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            _logger.LogInformation("Updated the raffle");
            data.UpdatedDate = response.DateUpdated;
        }

        public async Task<object> FindRaffleItemByName(string itemName)
        {
            var resultSet = await DocumentResource.ReadAsync(_serviceSid);
            var result = resultSet.FirstOrDefault(doc => doc.UniqueName.Equals(itemName, StringComparison
                .CurrentCultureIgnoreCase));
            return result;
        }

        public async Task ClearStorage()
        {
            var sids = (await DocumentResource.ReadAsync(
                _serviceSid))
                .Select(result => result.Sid)
                .ToList();

            foreach (var sid in sids)
            {
                await DocumentResource.DeleteAsync(
                    _serviceSid,
                    sid);
            }
        }

        public async Task<Raffle> GetLatestRaffle()
        {
            var latestDocument = (await DocumentResource.ReadAsync(pathServiceSid:_serviceSid))
                .OrderByDescending(doc => doc.DateCreated)
                .FirstOrDefault();

            if (latestDocument == null)
            {
                _logger.LogError("Unable to find the latest document from Sync");
                return await Task.FromResult<Raffle>(null);
            }

            var raffleData = JObject.Parse(JsonConvert.SerializeObject(latestDocument.Data));

            var latestRaffle = new Raffle
            {
                Name = latestDocument.UniqueName,
                Sid = latestDocument.Sid,
                Entries = raffleData.GetValue("entries", StringComparison.CurrentCultureIgnoreCase).Select(entry => JsonConvert.DeserializeObject<RaffleEntry>(entry.ToString())).ToList(),
                CreatedDate = latestDocument.DateCreated,
                UpdatedDate = latestDocument.DateUpdated,
                State = (RaffleState)Enum.Parse(typeof(RaffleState), raffleData.GetValue("state", StringComparison.CurrentCultureIgnoreCase).ToString()),
                Prize = new RafflePrize
                {
                    Name = raffleData["prize"]["name"].ToString(),
                    ImageUrl = raffleData["prize"]["imageUrl"].ToString(),
                    Quantity = int.Parse(raffleData["prize"]["quantity"].ToString())
                },
                Current = bool.Parse(raffleData["current"].ToString())
            };
            return latestRaffle;
        }
    }
}