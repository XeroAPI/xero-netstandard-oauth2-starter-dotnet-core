using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xero.NetStandard.OAuth2.Model.PayrollAu;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace XeroNetStandardApp.Controllers
{
  public class EmployeesInfo : Controller
  {
    private readonly ILogger<AuthorizationController> _logger;
    private readonly IOptions<XeroConfiguration> XeroConfig;
    private readonly IHttpClientFactory httpClientFactory;

    public EmployeesInfo(IOptions<XeroConfiguration> XeroConfig, IHttpClientFactory httpClientFactory, ILogger<AuthorizationController> logger)
    {
      _logger = logger;
      this.XeroConfig = XeroConfig;
      this.httpClientFactory = httpClientFactory;
    }

    // GET: /EmployeesInfo/
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

      var PayrollAUApi = new PayrollAuApi();
      var response = await PayrollAUApi.GetEmployeesAsync(accessToken, xeroTenantId);

      var employees = response._Employees;

      return View(employees);
    }

    // GET: /Contacts#Create
    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    // POST: /Contacts#Create
    [HttpPost]
    public async Task<ActionResult> Create(string firstName, string lastName, string DateOfBirth)
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

      // var contact = new Contact();
      // contact.Name = Name;
      // contact.EmailAddress = EmailAddress;
      // var contacts = new Contacts();
      // contacts._Contacts = new List<Contact>() { contact };

      DateTime dob = DateTime.Today.AddYears(-20);

      HomeAddress homeAddress = new HomeAddress() {
        AddressLine1 = "6 MeatMe Street",
        AddressLine2 = " ",
        Region = State.VIC,
        City = "Long Island",
        PostalCode = "9999", 
        Country = "New York"
      };

      Employee employee = new Employee() {
        FirstName = firstName,
        LastName = lastName,
        DateOfBirth = dob,
        HomeAddress = homeAddress
      };

      var employees = new List<Employee>() { employee };

      var PayrollAUApi = new PayrollAuApi();
      var response = await PayrollAUApi.CreateEmployeeAsync(accessToken, xeroTenantId, employees);

      return RedirectToAction("Index", "EmployeesInfo");
    }
  }
}