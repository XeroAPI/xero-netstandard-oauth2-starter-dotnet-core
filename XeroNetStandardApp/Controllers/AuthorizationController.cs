using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Token;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Models;
using System.Collections.Generic;

namespace XeroNetStandardApp.Controllers
{
  public class AuthorizationController : Controller
  {
    private readonly ILogger<AuthorizationController> _logger;
    private readonly IOptions<XeroConfiguration> XeroConfig;
    private readonly IHttpClientFactory httpClientFactory;

    // GET /Authorization/
    public AuthorizationController(IOptions<XeroConfiguration> XeroConfig, IHttpClientFactory httpClientFactory, ILogger<AuthorizationController> logger)
    {
      _logger = logger;
      this.XeroConfig = XeroConfig;
      this.httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {

      var client = new XeroClient(XeroConfig.Value, httpClientFactory);

      return Redirect(client.BuildLoginUri());
    }

    // GET /Authorization/Callback
    public async Task<ActionResult> Callback(string code, string state)
    {
      var client = new XeroClient(XeroConfig.Value, httpClientFactory);
      var xeroToken = (XeroOAuth2Token)await client.RequestXeroTokenAsync(code);

      List<Tenant> tenants = await client.GetConnectionsAsync(xeroToken);

      Tenant firstTenant = tenants[0];

      TokenUtilities.StoreToken(xeroToken);

      return RedirectToAction("Index", "OrganisationInfo");
    }

    // GET /Authorization/Disconnect
    public async Task<ActionResult> Disconnect()
    {      
      var client = new XeroClient(XeroConfig.Value, httpClientFactory);

      var xeroToken = TokenUtilities.GetStoredToken();
      var utcTimeNow = DateTime.UtcNow;

      if (utcTimeNow > xeroToken.ExpiresAtUtc)
      {
        xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);
        TokenUtilities.StoreToken(xeroToken);
      }

      string accessToken = xeroToken.AccessToken;
      Tenant xeroTenant = xeroToken.Tenants[0];

      await client.DeleteConnectionAsync(xeroToken, xeroTenant);

      TokenUtilities.DestroyToken();

      return RedirectToAction("Index", "Home");
    }
  }
}