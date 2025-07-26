using System;

namespace OfficeApplication.BLL.Modals
{
    public class UserProfile
    {
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public Nullable<int> UserRole { get; set; }
        public string UserRoleName { get; set; }
        public Nullable<int> RoleOrder { get; set; }
        public string UserName { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> UserGroup { get; set; }
        public string UserGroupName { get; set; }
    }

    public class UserGroup
    {
        public int UserGroupId { get; set; }
        public string UserGroupName { get; set; }
    }
}
