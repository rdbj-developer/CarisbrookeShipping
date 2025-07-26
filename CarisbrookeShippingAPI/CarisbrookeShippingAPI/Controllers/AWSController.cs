using CarisbrookeShippingAPI.BLL.Helpers;
using CarisbrookeShippingAPI.BLL.Modals;
using System.Collections.Generic;
using System.Web.Http;

namespace CarisbrookeShippingAPI.Controllers
{
    [Authorize] // JSL 09/26/2022
    public class AWSController : ApiController
    {
        [HttpGet]
        public List<UserModal> GetAWSUsers()
        {
            AWSHelper _helper = new AWSHelper();
            List<UserModal> users = _helper.GetAWSUsers();
            return users;
        }
        [HttpGet]
        public bool UpdateLocalDbForUsers()
        {
            AWSHelper _helper = new AWSHelper();
            bool res = _helper.UpdateLocalDbForUsers();
            return res;
        }
        [HttpGet]
        public List<UserModal> GetAWSUsersFromOfficeDBForSync()
        {
            AWSHelper _helper = new AWSHelper();
            List<UserModal> users = _helper.GetAWSUsersFromOfficeDBForSync();
            return users;
        }

        //RDBJ 09/16/2021
        [HttpGet]
        public List<CSShipsModalAWS> GetAWSCSShipsFromOfficeDBForSync()
        {
            AWSHelper _helper = new AWSHelper();
            List<CSShipsModalAWS> csShips = _helper.GetAWSCSShipsFromOfficeDBForSync();
            return csShips;
        }
        //End RDBJ 09/16/2021

        //RDBJ 11/08/2021
        [HttpGet]   // JSL 02/25/2023
        public List<string> GetEmailFromUserProfileTableWhereTechnicalAndISMGroup(
            string ShipCode // JSL 02/24/2023
            )
        {
            AWSHelper _helper = new AWSHelper();
            List<string> UserProfileModal = _helper.GetEmailFromUserProfileTableWhereTechnicalAndISMGroup(ShipCode);
            return UserProfileModal;
        }
        //Rnd RDBJ 11/08/2021

    }
}
