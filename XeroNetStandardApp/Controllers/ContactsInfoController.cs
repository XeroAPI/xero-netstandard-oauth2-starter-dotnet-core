using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.Options;

namespace XeroNetStandardApp.Controllers
{
    public class ContactsInfo : ApiAccessorController<AccountingApi>
    {

        public ContactsInfo(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig) { }

        // GET: /ContactsInfo/
        public async Task<IActionResult> Index()
        {
            var response = await Api.GetContactsAsync(XeroToken.AccessToken, TenantId);

            return View(response._Contacts);
        }

        // GET: /ContactsInfo#Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ContactsInfo#Create
        [HttpPost]
        public async Task<IActionResult> Create(string name, string emailAddress)
        {
            var newContact = new Contact
            {
                Name = name,
                EmailAddress = emailAddress
            };

            var contacts = new Contacts
            {
                _Contacts = new List<Contact>{ newContact }
            };

            await Api.CreateContactsAsync(XeroToken.AccessToken, TenantId, contacts);

            return RedirectToAction("Index", "ContactsInfo");
        }
    }
}