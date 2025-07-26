using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class LoginController : ApiController
    {
        [HttpPost]
        public UserProfileModal Login([FromBody]UserProfileModal value)
        {
            APIResponse res = new APIResponse();
            UserProfileModal user = new UserProfileModal();
            try
            {
                UserProfileHelper _helper = new UserProfileHelper();
                user = _helper.Login(value);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Login Exception : " + ex.Message);
                LogHelper.writelog("Login Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return user;
        }
        [HttpPost]
        public UserRole RolePremission([FromBody]UserProfileModal value)
        {
            APIResponse res = new APIResponse();
            UserRole role = new UserRole();
            try
            {
                UserProfileHelper _helper = new UserProfileHelper();
                role = _helper.RolePremission(value.RoleOrder);
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("RolePremission Exception : " + ex.Message);
                LogHelper.writelog("RolePremission Inner Exception : " + ex.InnerException);
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
            }
            return role;
        }
    }
}
