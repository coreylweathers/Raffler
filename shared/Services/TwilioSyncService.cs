using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Sync.V1.Service;

namespace shared.Services
{
    public class TwilioSyncService : IStorageService
    {
        protected string AccountSid { get; set; }
        protected string ServiceSid { get; set; }

        private readonly string _authToken;
        private readonly IConfiguration _config;
        private bool isConnected;

        public TwilioSyncService(IConfiguration config)
        {
            _config = config;
            AccountSid = _config[Constants.ACCOUNTSIDPATH];
            ServiceSid = _config[Constants.SERVICESIDPATH];
            // TODO: Ensure AuthToken can be grabbed from multiple places
            _authToken = _config[Constants.AUTHTOKENPATH];
            StartConnection();
        }

        protected void StartConnection()
        {
            TwilioClient.Init(AccountSid, _authToken);
            isConnected = true;
        }

        public async Task<string> AddRaffleEntryAsync(RaffleEntry entry)
        {
            var data = new Dictionary<string, RaffleEntry>
            {
                {"entry", entry }
            };

            var document = await DocumentResource.CreateAsync(
                pathServiceSid: ServiceSid,
                data: data,
                ttl: 0);

            return document.Sid;
        }

        public async Task<IEnumerable<RaffleEntry>> GetRaffleEntriesAsync()
        {
            var listOfDocuments = await DocumentResource.ReadAsync(
                pathServiceSid: ServiceSid);

            var data = listOfDocuments
                   .Select(doc => doc.Data.ToString())
                   .Select(entry => JObject.Parse(entry))
                   .Select(json => new RaffleEntry
                   {
                       EmailAddress = json["entry"]["EmailAddress"].ToString(),
                       MessageSid = json["entry"]["MessageSid"].ToString(),
                       TimeStamp = Convert.ToDateTime(json["entry"]["TimeStamp"].ToString())
                   });

            return data;
        }
    }
}
