using Microsoft.Extensions.Configuration;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Sync.V1.Service;
using Twilio.Rest.Sync.V1.Service.SyncList;
using System.Linq;
using Newtonsoft.Json;

namespace shared.Services
{
    public class TwilioPrizeStorageService : IPrizeStorageService
    {
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly IConfiguration _config;
        private readonly string _twilioSyncServiceSid;
        private string _twilioSyncListSid;

        public bool IsInitialized { get; set; }

        public TwilioPrizeStorageService(IConfiguration config)
        {
            _config = config;
            _twilioAccountSid = _config[Constants.ACCOUNT_SID_PATH];
            _twilioAuthToken = _config[Constants.AUTH_TOKEN_PATH];
            _twilioSyncServiceSid = _config[Constants.SYNC_SERVICE_SID_PATH];
            _twilioSyncListSid = _config[Constants.SYNC_SERVICE_LIST_SID_PATH];
            TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
        }

        public async Task InitializeService()
        {
            if (string.IsNullOrEmpty(_twilioSyncListSid))
            {
                await CreateRepository();
                _config[Constants.SYNC_SERVICE_LIST_SID_PATH] = _twilioSyncListSid;
            }

            IsInitialized = true;
        }

        public async Task<object> CreateRepository()
        {
            var response = await SyncListResource.CreateAsync(
                pathServiceSid: _twilioSyncServiceSid,
                uniqueName: $"Prize Service {DateTime.UtcNow}");
            _twilioSyncListSid = response.Sid;

            return response;
        }

        public Task<RafflePrize> FindItemByName(string itemName)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRepository(int index, RafflePrize data)
        {
            var response = await SyncListItemResource.UpdateAsync(
                pathServiceSid: _twilioSyncServiceSid,
                pathListSid: _twilioSyncListSid,
                pathIndex: index);
        }

        public async Task<IDictionary<int, RafflePrize>> GetItems()
        {
            var results = await SyncListItemResource.ReadAsync(
                pathServiceSid: _twilioSyncServiceSid,
                pathListSid: _twilioSyncListSid);
            return results.ToDictionary(entry => entry.Index.Value, entry => JsonConvert.DeserializeObject<RafflePrize>(JsonConvert.SerializeObject(entry.Data)));
        }

        public async Task<int> AddItemToRepository(RafflePrize data) => 
            (await SyncListItemResource.CreateAsync(
                pathServiceSid: _twilioSyncServiceSid,
                pathListSid: _twilioSyncListSid,
                data: data)).Index.Value;
    }
}
