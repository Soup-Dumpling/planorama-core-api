using System;
using System.Threading.Tasks;

namespace Planorama.User.Core.Services
{
    public interface IHttpService
    {
        Task<T> GetAsync<T>(string url, string token);
    }
}
