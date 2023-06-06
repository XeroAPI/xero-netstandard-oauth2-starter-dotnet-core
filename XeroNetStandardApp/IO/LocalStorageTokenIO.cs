using System.IO;
using System.Linq;
using System.Text.Json;
using Xero.NetStandard.OAuth2.Token;

namespace XeroNetStandardApp.IO
{
    /// <summary>
    /// Manager for locally storing and loading token data 
    /// </summary>
    public sealed class LocalStorageTokenIO : ITokenIO
    {
        // Singleton
        private static LocalStorageTokenIO _instance;
        private static readonly object Lock = new object();

        /// <summary>
        /// Thread safe instance retrieval
        /// </summary>
        public static LocalStorageTokenIO Instance
        {
            get
            {
                lock (Lock) return _instance ??= new LocalStorageTokenIO();
            }
        }

        // Prevent object being instantiated outside of class
        private LocalStorageTokenIO(){}

        private const string TokenFilePath = "./xerotoken.json";
        private const string TenantIdFilePath = "./tenantid.json";

        /// <summary>
        /// Check if a stored token exists if so, return saved token otherwise, instantiate a new token
        /// </summary>
        /// <returns>Returns a Xero OAuth2 Token</returns>
        public XeroOAuth2Token GetToken()
        {
            if (File.Exists(TokenFilePath))
            {
                var serializedToken = File.ReadAllText(TokenFilePath);
                return JsonSerializer.Deserialize<XeroOAuth2Token>(serializedToken);
            }

            return new XeroOAuth2Token();
        }

        /// <summary>
        /// Save token contents to file
        /// </summary>
        /// <param name="xeroToken">Xero OAuth2 token to save</param>
        public void StoreToken(XeroOAuth2Token xeroToken)
        {
            var serializedToken = JsonSerializer.Serialize(xeroToken);
            File.WriteAllText(TokenFilePath, serializedToken);
        }

        /// <summary>
        /// Destroy json file holding Xero OAuth2 token data
        /// </summary>
        public void DestroyToken()
        {
            if (File.Exists(TokenFilePath))
                File.Delete(TokenFilePath);
        }

        /// <summary>
        /// Get a valid tenant id associated with stored xero OAuth2 token
        /// </summary>
        /// <returns>Returns a valid tenant id</returns>
        public string GetTenantId()
        {
            var xeroToken = GetToken();
            string tenantId = null;
            if (File.Exists(TenantIdFilePath))
            {
                var serializedTenantId = File.ReadAllText(TenantIdFilePath);
                tenantId = JsonSerializer.Deserialize<TenantIdModel>(serializedTenantId)?.CurrentTenantId;
            }

            if (xeroToken.Tenants.All((t) => t.TenantId.ToString() != tenantId))
            {
                tenantId = xeroToken.Tenants.First().TenantId.ToString();
                StoreTenantId(tenantId);
            }

            return tenantId;
        }

        /// <summary>
        /// Save tenant id to disk
        /// <param name="tenantId">Tenant Id to store</param>
        /// </summary>
        public void StoreTenantId(string tenantId)
        {
            var serializedTenantId = JsonSerializer.Serialize(new TenantIdModel { CurrentTenantId = tenantId });
            if (File.Exists(TenantIdFilePath))
            {
                File.WriteAllText(TenantIdFilePath, serializedTenantId);
            }
        }

        /// <summary>
        /// Check if stored token file exists
        /// </summary>
        /// <returns>Returns boolean specify if stored token file exists</returns>
        public bool TokenExists()
        {
            return File.Exists(TokenFilePath);
        }
    }

    /// <summary>
    /// Holds file structure for saving tenant ids to disk
    /// </summary>
    internal class TenantIdModel
    {
        public string CurrentTenantId { get; set; }
    }

}

