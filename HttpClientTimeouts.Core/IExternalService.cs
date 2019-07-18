
using System.Threading.Tasks;

namespace HttpClientTimeouts.Core
{
    public interface IExternalService
    {
        Task<string> SendAsync(byte[] payload);
    }
}
