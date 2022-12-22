using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.Options;

namespace XeroNetStandardApp.Controllers
{
    public class InvoiceSync : ApiAccessorController<AccountingApi>
    {
        public InvoiceSync(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig) { }

        // GET: /InvoiceSync/
        public async Task<IActionResult> Index()
        {
            var sevenDaysAgo = DateTime.Now.AddDays(-7).ToString("yyyy, MM, dd");
            var invoicesFilter = "Date >= DateTime(" + sevenDaysAgo + ")";

            var response = await Api.GetInvoicesAsync(XeroToken.AccessToken, TenantId, where: invoicesFilter);

            return View(response._Invoices);
        }

        // GET: /InvoiceSync#Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /InvoiceSync#Create
        [HttpPost]
        public async Task<IActionResult> Create(string name, string lineDescription, string lineQuantity, string lineUnitAmount, string lineAccountCode)
        {
            var contact = new Contact { Name = name };

            var line = new LineItem
            {
                Description = lineDescription,
                Quantity = decimal.Parse(lineQuantity),
                UnitAmount = decimal.Parse(lineUnitAmount),
                AccountCode = lineAccountCode
            };
            var lines = new List<LineItem> { line };

            var invoice = new Invoice
            {
                Type = Invoice.TypeEnum.ACCREC,
                Contact = contact,
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(30),
                LineItems = lines
            };

            var invoices = new Invoices
            {
                _Invoices = new List<Invoice> { invoice }
            };

            await Api.CreateInvoicesAsync(XeroToken.AccessToken, TenantId, invoices);

            return RedirectToAction("Index", "InvoiceSync");
        }
    }
}