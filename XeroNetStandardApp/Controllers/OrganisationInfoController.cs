using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Model;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;


namespace XeroNetStandardApp.Controllers
{
  public class OrganisationInfo : Controller
  {

    private readonly ILogger<AuthorizationController> _logger;
    private readonly IOptions<XeroConfiguration> XeroConfig;
    private readonly IHttpClientFactory httpClientFactory;

    public OrganisationInfo(IOptions<XeroConfiguration> XeroConfig, IHttpClientFactory httpClientFactory, ILogger<AuthorizationController> logger)
    {
      _logger = logger;
      this.XeroConfig = XeroConfig;
      this.httpClientFactory = httpClientFactory;
    }

    // GET: /Organisation/
    public async Task<ActionResult> Index()
    {
      var xeroToken = TokenUtilities.GetStoredToken();
      var utcTimeNow = DateTime.UtcNow;

      if (utcTimeNow > xeroToken.ExpiresAtUtc)
      {
        var client = new XeroClient(XeroConfig.Value, httpClientFactory);
        xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);
        TokenUtilities.StoreToken(xeroToken);
      }

      string accessToken = xeroToken.AccessToken;
      string xeroTenantId = xeroToken.Tenants[0].TenantId.ToString();

      var AccountingApi = new AccountingApi();
      var response = await AccountingApi.GetOrganisationsAsync(accessToken, xeroTenantId);

      var organisation_info = new Organisation();
      organisation_info = response._Organisations[0];

      return View(organisation_info);
    }
  }
}