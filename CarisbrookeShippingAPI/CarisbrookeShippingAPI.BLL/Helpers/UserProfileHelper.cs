using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class UserProfileHelper
    {
        public Modals.UserProfileModal Login(Modals.UserProfileModal user)
        {
            Modals.UserProfileModal _user = new Modals.UserProfileModal();
            using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
            {
                var data = dbContext.UserProfiles.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
                if (data != null)
                {
                    _user.Email = data.Email;
                    _user.Password = data.Password;
                    _user.UserRole = data.UserRole;
                    _user.RoleOrder = data.RoleOrder;
                    _user.UserName = data.UserName;
                    _user.UserID = data.UserID;
                    _user.UserGroup = data.UserGroup;
                }
            }
            return _user;
        }
        public Modals.UserRole RolePremission(int? roleOrder)
        {
            Modals.UserRole role = new Modals.UserRole();
            using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
            {
                var data = dbContext.RolePermissions.Where(x => x.RoleID == roleOrder).FirstOrDefault();
                if (data != null)
                {
                    role.RoleID = data.RoleID;
                    role.RoleName = data.RoleName;
                    role.IsAdmin = data.IsAdmin;
                    role.IsDocument = data.IsDocument;
                    role.IsForm = data.IsForm;
                    role.IsReport = data.IsReport;
                    role.IsSettings = data.IsSettings;
                    role.IsHelp = data.IsHelp;
                    role.IsHome = data.IsHome;
                    role.IsDeficiencies = data.IsDeficiencies;
                    role.IsAbout = data.IsAbout;
                }
            }
            return role;
        }
        public bool AddUser(UserProfileModal user)
        {
            bool res = false;
            try
            {
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    UserProfile dbUser = new UserProfile();
                    dbUser.UserID = Guid.NewGuid();
                    dbUser.Email = user.Email;
                    dbUser.Password = user.Password;
                    dbUser.UserRole = user.UserRole;
                    dbUser.RoleOrder = user.RoleOrder;
                    dbUser.UserName = user.UserName;
                    dbUser.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbUser.UserGroup = user.UserGroup;
                    dbContext.UserProfiles.Add(dbUser);
                    dbContext.SaveChanges();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        public List<UserProfileModal> GetAllUsers()
        {
            List<UserProfileModal> res = new List<UserProfileModal>();
            try
            {
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    List<UserProfile> dbUsersList = dbContext.UserProfiles.OrderByDescending(x => x.CreatedDate).ToList();
                    List<UserGroupModal> dbUserGroupList = null;
                    if (dbUsersList != null && dbUsersList.Count > 0)
                        dbUserGroupList = GetAllUserGroups();
                    if (dbUserGroupList == null)
                        dbUserGroupList = new List<UserGroupModal>();

                    res = (from t1 in dbUsersList
                                 join t2temp in dbUserGroupList on t1.UserGroup equals t2temp.UserGroupId into tempJoin
                                 from t2 in tempJoin.DefaultIfEmpty()
                                 select(new UserProfileModal()
                                 {
                                     UserID = t1.UserID,
                                     Email = t1.Email,
                                     Password = string.Empty,
                                     UserRole = t1.UserRole,
                                     UserRoleName = GetRoleName(t1.UserRole),
                                     RoleOrder = t1.RoleOrder,
                                     UserName = t1.UserName,
                                     UserGroup = t1.UserGroup,
                                     UserGroupName = t2.UserGroupName
                                 })).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        public string GetRoleName(int? ID)
        {
            if (ID == 1)
                return "sa";
            if (ID == 2)
                return "repoter";
            if (ID == 3)
                return "admin";
            return string.Empty;
        }


        #region UserGroup
        public List<UserGroupModal> GetAllUserGroups()
        {
            List<UserGroupModal> res = new List<UserGroupModal>();
            try
            {
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    List<UserGroup> dbUsersList = dbContext.UserGroups.OrderBy(x => x.UserGroupName).ToList();
                    res = dbUsersList.Select(x => new UserGroupModal()
                    {
                        UserGroupId = x.UserGroupId,
                        UserGroupName = x.UserGroupName
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        public List<MenusModal> GetUserGroupMenuPermission(int userGroupId)
        {
            List<MenusModal> res = new List<MenusModal>();
            try
            {
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    List<UserGroupMenuPermission> dbUserGroupMenuPermissionList = dbContext.UserGroupMenuPermissions.Where(x => x.UserGroupID == userGroupId
                    && x.IsActive == true   // RDBJ 02/24/2022
                    ).ToList();
                    List<Menu> dbMenuList = null;
                    MenusHelper _menuHelper = new MenusHelper();
                    if (dbUserGroupMenuPermissionList != null && dbUserGroupMenuPermissionList.Count > 0)
                        dbMenuList = _menuHelper.GetAllMenus();
                    if (dbMenuList == null)
                        dbMenuList = new List<Menu>();
                    res = (from t1 in dbUserGroupMenuPermissionList
                           join t2 in dbMenuList on t1.MenuID equals t2.MenuId //--into tempJoin
                          // from t2 in tempJoin.DefaultIfEmpty()
                           select (new MenusModal()
                           {
                               MenuId = t1.MenuID,
                               ActionName = t2.ActionName,
                               Class = t2.Class,
                               ControllerName = t2.ControllerName,
                               Description = t2.Description,
                               DisplayOrder = t2.DisplayOrder,
                               IsActive = t1.IsActive,
                               IsParent = t2.IsParent,
                               MenuLevel = t2.MenuLevel,
                               MenuText = t2.MenuText,
                               MenuType = t2.MenuType,
                               ParentId = t2.ParentId,
                               UserGroupID = t1.UserGroupID,
                               IsDefaultMenu = t2.IsDefaultMenu
                           })).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        #endregion

        #region ShipApplication
        public ShipUserModal LoginUser(ShipUserReq Modal)
        {
            ShipUserModal user = new ShipUserModal();
            try
            {
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    User dbUser = dbContext.Users.Where(x => x.empnre01 == Modal.Password).FirstOrDefault();
                    if (dbUser != null)
                    {
                        string fname = dbUser.fstnme01.ToLower();
                        string surname = dbUser.surnme01.ToLower();
                        string username = string.Empty;
                        if (fname.Contains(" "))
                        {
                            string[] fArray = fname.Split(' ');
                            fname = fArray[0];
                        }
                        if (surname.Contains(" "))
                        {
                            string[] sArray = surname.Split(' ');
                            surname = sArray[0];
                        }
                        username = fname + "." + surname;
                        if (Modal.UserName.ToLower() == username && Modal.Password == dbUser.empnre01)
                        {
                            user.UID = dbUser.UID;
                            user.EmployeeID = dbUser.empnre01;
                            user.FirstName = dbUser.fstnme01;
                            user.SurName = dbUser.surnme01;
                            user.UserName = Modal.UserName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return user;
        }
        #endregion

        #region Common
        // JSL 07/23/2022
        public Dictionary<string, string> PerformPostAPICall(Dictionary<string, string> dictMetaData)
        {
            Dictionary<string, string> retDictMetaData = new Dictionary<string, string>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            string strPerformAction = string.Empty;
            bool IsPerformSuccess = false;

            if (dictMetaData.ContainsKey("strAction"))
                strPerformAction = dictMetaData["strAction"].ToString();

            switch (strPerformAction)
            {
                // JSL 07/23/2022
                case AppStatic.API_GETFORMSPERSONLIST:
                    {
                        try
                        {
                            bool IsNeedToGetJustSelectedTypePersons = false;
                            int intPersonType = 0;

                            if (dictMetaData.ContainsKey("PersonType"))
                            {
                                intPersonType = Convert.ToInt32(dictMetaData["PersonType"]);
                                IsNeedToGetJustSelectedTypePersons = true;
                                retDictMetaData["PersonType"] = intPersonType.ToString();
                            }

                            var lstFormsPerson = dbContext.CB_FormsPersonMaster
                                .Where(x => x.IsDeleted == false)
                                .OrderBy(x => x.PersonName)
                                .ToList();

                            if (IsNeedToGetJustSelectedTypePersons)
                            {
                                lstFormsPerson = lstFormsPerson.Where(x => x.PersonType == intPersonType).ToList();
                            }

                            retDictMetaData["FormsPersonList"] = JsonConvert.SerializeObject(lstFormsPerson);
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_GETFORMSPERSONLIST + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End JSL 07/23/2022
                // JSL 07/23/2022
                case AppStatic.API_ADDNEW_UPDATE_DELETE_FORMSPERSON:
                    {
                        try
                        {
                            bool IsNeedToUpdate = false;
                            string strUniqueId = string.Empty;
                            Guid guidUniqueID = new Guid();
                            string strPersonName = string.Empty;
                            int intPersonType = 0;

                            bool IsNeedToDelete = false;

                            string strCurrentUserID = string.Empty;
                            Guid guidCurrentUserID = new Guid();

                            if (dictMetaData.ContainsKey("UniqueId"))
                            {
                                strUniqueId = dictMetaData["UniqueId"];
                                if (!string.IsNullOrEmpty(strUniqueId))
                                {
                                    guidUniqueID = Guid.Parse(strUniqueId);
                                    IsNeedToUpdate = true;
                                }
                            }

                            if (dictMetaData.ContainsKey("PersonName"))
                                strPersonName = dictMetaData["PersonName"];

                            if (dictMetaData.ContainsKey("PersonType"))
                                intPersonType = Convert.ToInt32(dictMetaData["PersonType"]);

                            if (dictMetaData.ContainsKey("CurrentUserID"))
                            {
                                strCurrentUserID = dictMetaData["CurrentUserID"];
                                guidCurrentUserID = Guid.Parse(strCurrentUserID);
                            }

                            if (dictMetaData.ContainsKey("IsDelete"))
                                IsNeedToDelete = Convert.ToBoolean(dictMetaData["IsDelete"]);

                            var entUserModal = dbContext.CB_FormsPersonMaster.Where(x => x.UniqueId == guidUniqueID && x.IsDeleted == false).FirstOrDefault();
                            if (entUserModal == null)
                            {
                                entUserModal = new CB_FormsPersonMaster();
                            }

                            if (!IsNeedToDelete)
                            {
                                entUserModal.PersonName = strPersonName;
                                entUserModal.PersonType = intPersonType;
                            }
                            
                            if (IsNeedToUpdate)
                            {
                                entUserModal.IsDeleted = IsNeedToDelete;
                                entUserModal.ModifiedBy = guidCurrentUserID;
                                entUserModal.ModifiedDateTime = Utility.ToDateTimeUtcNow();
                            }
                            else
                            {
                                entUserModal.UniqueId = Guid.NewGuid();
                                entUserModal.CreatedBy = guidCurrentUserID;
                                entUserModal.CreatedDateTime = Utility.ToDateTimeUtcNow();
                                entUserModal.IsDeleted = IsNeedToDelete;
                                dbContext.CB_FormsPersonMaster.Add(entUserModal);
                            }
                            dbContext.SaveChanges();
                            IsPerformSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.writelog(AppStatic.API_ADDNEW_UPDATE_DELETE_FORMSPERSON + " Error : " + ex.Message);
                        }
                        break;
                    }
                // End JSL 07/23/2022
                default:
                    break;
            }

            if (IsPerformSuccess)
            {
                retDictMetaData["Status"] = AppStatic.SUCCESS;
            }
            else
            {
                retDictMetaData["Status"] = AppStatic.ERROR;
            }

            return retDictMetaData;
        }
        // End JSL 07/23/2022
        #endregion
    }
}
