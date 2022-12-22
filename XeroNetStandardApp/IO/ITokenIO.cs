using Xero.NetStandard.OAuth2.Token;

namespace XeroNetStandardApp.IO
{
    /// <summary>
    /// Interface for managing the storage and retrieval of a Xero OAuth2 token
    /// </summary>
    public interface ITokenIO
    {
        /// <summary>
        /// Get stored token
        /// </summary>
        /// <returns>Returns a Xero OAuth2 token</returns>
        public XeroOAuth2Token GetToken();

        /// <summary>
        /// Store an existing Xero OAuth2 token
        /// <param name="xeroToken">Xero OAuth2 token to store</param>
        /// </summary>
        public void StoreToken(XeroOAuth2Token xeroToken);

        /// <summary>
        /// Delete the stored Xero OAuth2 token if exists
        /// </summary>
        public void DestroyToken();

        /// <summary>
        /// Does a stored token exist
        /// </summary>
        /// <returns>Boolean specifying whether a token has been stored</returns>
        public bool TokenExists();

        /// <summary>
        /// Get tenant id stored in token
        /// </summary>
        /// <returns>Returns a tenant id inside token</returns>
        public string GetTenantId();

        /// <summary>
        /// Store tenant id
        /// <param name="tenantId">Tenant Id to store</param>
        /// </summary>
        public void StoreTenantId(string tenantId);
    }
}
