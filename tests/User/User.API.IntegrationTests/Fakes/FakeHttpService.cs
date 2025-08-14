using FizzWare.NBuilder;
using Planorama.User.Core.Services;
using System.Threading.Tasks;

namespace Planorama.User.API.IntegrationTests.Fakes
{
    public class FakeHttpService : IHttpService
    {
        public FakeHttpService() { }

        public Task<T> GetAsync<T>(string url, string token)
        {
            if (!typeof(T).IsGenericType)
            {
                var result = Builder<T>.CreateNew().Build();
                return Task.FromResult(result);
            }
            else
            {
                var builder = new Builder();
                var genericArg = typeof(T).GetGenericArguments()[0];
                var method = typeof(Builder).GetMethod("CreateListOfSize", new[] { typeof(int) });
                var generic = method.MakeGenericMethod(genericArg);
                dynamic result = generic.Invoke(builder, new object[] { 2 });
                return Task.FromResult((T)result.Build());
            }
        }
    }
}
