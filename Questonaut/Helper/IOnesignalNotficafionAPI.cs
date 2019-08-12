using System;
using System.Threading.Tasks;
using Questonaut.Model;
using Refit;

namespace Questonaut.Helper
{
    [Headers("Content-Type: application/json")]
    public interface IOnesignalNotficafionAPI
    {
        [Post("/api/v1/notifications")]
        Task<HttpPost> SendNotification([Body] HttpPost post, [Header("Authorization")] string token);
    }
}
