using System;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class UserProfileModal
    {
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Nullable<int> UserRole { get; set; }
        public string UserRoleName { get; set; }
        public Nullable<int> RoleOrder { get; set; }
        public string UserName { get; set; }
        public Nullable<int> UserGroup { get; set; }
        public string UserGroupName { get; set; }
    }
    public class ShipUserReq
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }
    public class ShipUserModal
    {
        public int UID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string UserName { get; set; }

    }

    // JSL 07/23/2022
    public partial class CB_FormsPersonMasterModal
    {
        public System.Guid UniqueId { get; set; }
        public string PersonName { get; set; }
        public int PersonType { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<System.Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
    // End JSL 07/23/2022

}
