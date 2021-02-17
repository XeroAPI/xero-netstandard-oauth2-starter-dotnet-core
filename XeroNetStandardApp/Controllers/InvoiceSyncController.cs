using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace XeroNetStandardApp.Controllers
{
  public class InvoiceSync : Controller
  {
    private readonly ILogger<AuthorizationController> _logger;
    private readonly IOptions<XeroConfiguration> XeroConfig;
    private readonly IHttpClientFactory httpClientFactory;

    public InvoiceSync(IOptions<XeroConfiguration> XeroConfig, IHttpClientFactory httpClientFactory, ILogger<AuthorizationController> logger)
    {
      _logger = logger;
      this.XeroConfig = XeroConfig;
      this.httpClientFactory = httpClientFactory;
    }

    // GET: /InvoiceSync/
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


      var sevenDaysAgo = DateTime.Now.AddDays(-7).ToString("yyyy, MM, dd");
      var invoicesFilter = "Date >= DateTime(" + sevenDaysAgo + ")";

      var response = await AccountingApi.GetInvoicesAsync(accessToken, xeroTenantId, null, invoicesFilter);
      var invoices = response._Invoices;

      return View(invoices);
    }

    // GET: /InvoiceSync#Create
    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    // POST: /InvoiceSync#Create
    [HttpPost]
    public async Task<ActionResult> Create(string Name, string LineDescription, string LineQuantity, string LineUnitAmount, string LineAccountCode)
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

      var contact = new Contact();
      contact.Name = Name;
      
      var line = new LineItem() {
        Description = LineDescription,
        Quantity = decimal.Parse(LineQuantity),
        UnitAmount = decimal.Parse(LineUnitAmount),
        AccountCode = LineAccountCode
      };

      var lines = new List<LineItem>() {
        line
      };

      var invoice = new Invoice() {
        Type = Invoice.TypeEnum.ACCREC,
        Contact = contact,
        Date = DateTime.Today,
        DueDate = DateTime.Today.AddDays(30),
        LineItems = lines
      };

      var invoiceList = new List<Invoice>();
      invoiceList.Add(invoice);

      var invoices = new Invoices();
      invoices._Invoices = invoiceList;

      var AccountingApi = new AccountingApi();
      var response = await AccountingApi.CreateInvoicesAsync(accessToken, xeroTenantId, invoices);
      
      var updatedUTC = response._Invoices[0].UpdatedDateUTC;

      return RedirectToAction("Index", "InvoiceSync");
    }
  }
}