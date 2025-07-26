using CarisbrookeShippingAPI.Resources.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;

namespace CarisbrookeShippingAPI.Helper_Code.Common
{
    // JSL 09/26/2022 added this class
    public class AuthorizationHeaderHandler : DelegatingHandler
    {
        UserProfileHelper objUserProfileHelper = new UserProfileHelper();   // JSL 09/26/2022
        #region Send method.
        /// <summary>   
        /// Send method.   
        /// </summary>   
        /// <param name="request">Request parameter</param>   
        /// <param name="cancellationToken">Cancellation token parameter</param>   
        /// <returns>Return HTTP response.</returns>   
        // JSL 09/26/2022
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Initialization.   
            IEnumerable<string> apiKeyHeaderValues = null;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;
            string userName = null;
            string password = null;
            // Verification.   
            if (request.Headers.TryGetValues(ApiInfo.API_KEY_HEADER, out apiKeyHeaderValues) && !string.IsNullOrEmpty(authorization.Parameter))
            {
                var apiKeyHeaderValue = apiKeyHeaderValues.First();
                // Get the auth token   
                string authToken = authorization.Parameter;
                // Decode the token from BASE64   
                string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                // Extract username and password from decoded token   
                userName = decodedToken.Substring(0, decodedToken.IndexOf(":"));
                password = decodedToken.Substring(decodedToken.IndexOf(":") + 1);
                bool IsUserExist = IsRequestFromValidUser(userName, password);

                // Verification.   
                if (apiKeyHeaderValue.Equals(ApiInfo.API_KEY_VALUE) && 
                    ((userName.Equals(ApiInfo.USERNAME_VALUE) && password.Equals(ApiInfo.PASSWORD_VALUE))
                    || IsUserExist)
                    )
                {
                    // Setting   
                    var identity = new GenericIdentity(userName);
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
            }
            // Info.   
            return base.SendAsync(request, cancellationToken);
        }
        // End JSL 09/26/2022
        #endregion
        #region Set principal method.  
        /// <summary>   
        /// Set principal method.   
        /// </summary>   
        /// <param name="principal">Principal parameter</param>   
        // JSL 09/26/2022
        private static void SetPrincipal(IPrincipal principal)
        {
            // setting.   
            Thread.CurrentPrincipal = principal;
            // Verification.   
            if (HttpContext.Current != null)
            {
                // Setting.   
                HttpContext.Current.User = principal;
            }
        }
        // End JSL 09/26/2022
        #endregion

        #region Check User Exist Or Not 
        private bool IsRequestFromValidUser(string strUsername, string strPassword)
        {
            bool blnReturn = false;
            UserProfileModal userProfileModal = new UserProfileModal();
            userProfileModal.Email = strUsername;
            userProfileModal.Password = strPassword;

            var userDetails = objUserProfileHelper.Login(userProfileModal);
            if (userDetails != null)
            {
                if (userDetails.Email != null)
                    blnReturn = true;
            }
            return blnReturn;
        }
        #endregion
    }
}