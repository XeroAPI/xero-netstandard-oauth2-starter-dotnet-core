using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.Options;

namespace XeroNetStandardApp.Controllers
{
    public class IdentityInfo : ApiAccessorController<IdentityApi>
    {

        public IdentityInfo(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig) { }

        // GET: /IdentityInfo/
        public async Task<IActionResult> Index()
        {
            var connections = await Api.GetConnectionsAsync(XeroToken.AccessToken);

            return View(connections);
        }

        // GET: /Identity#Delete
        [HttpGet]
        public async Task<IActionResult> Delete(string connectionId)
        {
            await Api.DeleteConnectionAsync(XeroToken.AccessToken, Guid.Parse(connectionId));

            tokenIO.DestroyToken();

            return RedirectToAction("Index", "Home");
        }
    }
}