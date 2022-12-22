using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.Options;

namespace XeroNetStandardApp.Controllers
{
    public class OrganisationInfo : ApiAccessorController<AccountingApi>
    {
        public OrganisationInfo(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig) { }

        // GET: /Organisation/
        public async Task<IActionResult> Index()
        {
            var response = await Api.GetOrganisationsAsync(XeroToken.AccessToken, TenantId);

            return View(response._Organisations[0]);
        }
    }
}