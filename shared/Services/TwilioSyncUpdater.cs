using System.Threading.Tasks;
using Twilio.Rest.Sync.V1.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace shared.Services
{
    public class TwilioSyncUpdater : IStorageUpdater
    {
        public async Task UpdateStorage(string serviceId, string id, object data)
        {
            await DocumentResource.UpdateAsync(
                pathServiceSid: serviceId,
                pathSid: id,
                data: JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
    }
}