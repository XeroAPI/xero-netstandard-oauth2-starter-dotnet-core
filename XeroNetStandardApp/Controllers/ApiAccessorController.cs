using Microsoft.Extensions.Options;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;

namespace XeroNetStandardApp.Controllers
{
    /// <summary>
    /// Extends BaseXeroOAuth2Controller providing xero token management and T api setup logic
    /// </summary>
    /// <typeparam name="T">Api to setup</typeparam>
    public abstract class ApiAccessorController<T> : BaseXeroOAuth2Controller where T : IApiAccessor, new()
    {
        protected readonly T Api;

        protected ApiAccessorController(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig)
        {
            Api = new T();
        }
    }
}
