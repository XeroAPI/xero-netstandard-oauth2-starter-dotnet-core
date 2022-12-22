using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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
    public class InvoicePdf : ApiAccessorController<AccountingApi>
    {
        public InvoicePdf(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig) { }

        // GET: /InvoicePdf/
        public async Task<IActionResult> Index()
        {

            Guid invoiceId = Guid.Parse("d5654fc7-be8e-42e5-802a-9f5ab9d2476a");

            var AccountingApi = new AccountingApi();

            var response = await Api.GetInvoiceAsPdfAsync(XeroToken.AccessToken, TenantId, invoiceId);

            return new FileStreamResult(response, "application/pdf");

        }
    }
}