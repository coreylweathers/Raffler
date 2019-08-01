using System.Threading.Tasks;

namespace shared.Services
{
    public interface IStorageUpdater
    {
        Task UpdateStorage(string serviceId, string id, object data);
    }
}
