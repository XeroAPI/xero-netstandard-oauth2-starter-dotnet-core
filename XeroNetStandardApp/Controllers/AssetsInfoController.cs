using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Model.Asset;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Config;
using Microsoft.Extensions.Options;

namespace XeroNetStandardApp.Controllers
{
    public class AssetsInfo : ApiAccessorController<AssetApi>
    {

        public AssetsInfo(IOptions<XeroConfiguration> xeroConfig) : base(xeroConfig) { }

        // GET: /Assets/
        public async Task<IActionResult> Index()
        {
            var response = await Api.GetAssetsAsync(XeroToken.AccessToken, TenantId, AssetStatusQueryParam.DRAFT);

            return View(response.Items);
        }

        // GET: /AssetsInfo#Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AssetsInfo#Create
        [HttpPost]
        public async Task<ActionResult> Create(string name, string number)
        {
            var asset = new Asset()
            {
                AssetName = name,
                AssetNumber = number
            };

            await Api.CreateAssetAsync(XeroToken.AccessToken, TenantId, asset);

            return RedirectToAction("Index", "AssetsInfo");
        }
    }
}