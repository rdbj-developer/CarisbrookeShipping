using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    //[Authorize]   // JSL 10/03/2022 // JSL 09/26/2022
    public class UsersController : ApiController
    {
        UserProfileHelper _ObjUPhelper = new UserProfileHelper();    // JSL 07/23/2022
        [HttpGet]
        public List<UserProfileModal> GetAllUsers()
        {
            UserProfileHelper _helper = new UserProfileHelper();
            List<UserProfileModal> res = _helper.GetAllUsers();
            return res;
        }

        [HttpPost]
        public bool AddUser(UserProfileModal value)
        {
            UserProfileHelper _helper = new UserProfileHelper();
            bool res = _helper.AddUser(value);
            return res;
        }
        #region User Goup

        [HttpGet]
        public List<UserGroupModal> GetAllUserGroups()
        {
            UserProfileHelper _helper = new UserProfileHelper();
            List<UserGroupModal> res = _helper.GetAllUserGroups();
            return res;
        }

        [HttpGet]
        public List<MenusModal> GetUserGroupMenuPermission(int id)
        {
            UserProfileHelper _helper = new UserProfileHelper();
            List<MenusModal> res = _helper.GetUserGroupMenuPermission(id);
            return res;
        }
        #endregion
        #region Ship Application

        [HttpPost]
        public ShipUserModal LoginUser(ShipUserReq value)
        {
            UserProfileHelper _helper = new UserProfileHelper();
            ShipUserModal res = _helper.LoginUser(value);
            return res;
        }
        #endregion

        // JSL 07/23/2022
        public Dictionary<string, string> CommonPostAPICall(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDicData = new Dictionary<string, string>();
            retDicData = _ObjUPhelper.PerformPostAPICall(dictMetaData);
            return retDicData;
        }
        // End JSL 07/23/2022
    }
}
