using Microsoft.Extensions.Configuration;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Sync.V1.Service;
using Twilio.Rest.Sync.V1.Service.SyncList;
using System.Linq;

namespace shared.Services
{
    public class TwilioPrizeStorageService : IPrizeStorageService
    {
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        private readonly IConfiguration _config;
        private readonly string _twilioSyncServiceSid;
        private string _twilioSyncListSid;

        public TwilioPrizeStorageService(IConfiguration config)
        {
            _config = config;
            _twilioAccountSid = _config[Constants.ACCOUNT_SID_PATH];
            _twilioAuthToken = _config[Constants.AUTH_TOKEN_PATH];
            _twilioSyncServiceSid = _config[Constants.SYNC_SERVICE_SID_PATH];
            //TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);
        }

        public async Task<object> CreateStorage(string serviceId, string raffleName, object data)
        {
            var response = await SyncListResource.CreateAsync(
                pathServiceSid: serviceId,
                uniqueName: raffleName);
            _twilioSyncListSid = response.Sid;

            return response;
        }

        public Task<object> FindItemByName(string serviceId, string itemName)
        {
            throw new NotImplementedException();
        }

        public Task UpdateStorage(string serviceId, string id, object data)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<RafflePrize>> GetItems()
        {
            await SyncListItemResource.FetchAsync(
                pathServiceSid: _twilioSyncServiceSid,
                pathListSid: _twilioSyncListSid,
                pathIndex: 1);

            return new List<RafflePrize>();
            
        }
    }
}
