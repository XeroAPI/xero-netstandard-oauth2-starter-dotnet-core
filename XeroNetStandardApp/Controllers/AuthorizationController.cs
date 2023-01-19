using Microsoft.AspNetCore.Mvc;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Token;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace XeroNetStandardApp.Controllers
{
    /// <summary>
    /// Controller managing authorization 
    /// </summary>
    public class AuthorizationController : BaseXeroOAuth2Controller
    {
        private readonly XeroClient _client;

        public AuthorizationController(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig)
        {
            _client = new XeroClient(xeroConfig.Value);
        }

        /// <summary>
        /// Index, redirect to login page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return Redirect(_client.BuildLoginUri());
        }

        /// <summary>
        /// Callback for authorization
        /// </summary>
        /// <param name="code">Returned code</param>
        /// <param name="state">Returned state</param>
        /// <returns>Redirect to organisations page</returns>
        public async Task<IActionResult> Callback(string code, string state)
        {
            var xeroToken = (XeroOAuth2Token)await _client.RequestAccessTokenAsync(code);

            tokenIO.StoreToken(xeroToken);

            return RedirectToAction("Index", "OrganisationInfo");
        }

        /// <summary>
        /// Disconnect org connections to sample app. Destroys token
        /// <para>GET /Authorization/Disconnect</para>
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Disconnect()
        {
            await _client.DeleteConnectionAsync(XeroToken, XeroToken.Tenants[0]);

            tokenIO.DestroyToken();

            return RedirectToAction("Index", "Home");
        }
    }
}