using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.Options;


namespace XeroNetStandardApp.Controllers
{
    public class BankTransactionsInfo : ApiAccessorController<AccountingApi>
    {

        public BankTransactionsInfo(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig) { }

        // GET: /BankTransactionsInfo/
        public async Task<IActionResult> Index()
        {
            var response = await Api.GetBankTransactionsAsync(XeroToken.AccessToken, TenantId);

            return View(response._BankTransactions);
        }
    }
}