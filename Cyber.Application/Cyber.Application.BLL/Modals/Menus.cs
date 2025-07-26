using System;

namespace OfficeApplication.BLL.Modals
{
    public class Menus
    {
        public int MenuId { get; set; }
        public int? MenuLevel { get; set; }
        public int? ParentId { get; set; }
        public int? DisplayOrder { get; set; }
        public string MenuText { get; set; }
        public string Description { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Class { get; set; }
        public bool? IsParent { get; set; }
        public string MenuType { get; set; }
        public bool? IsActive { get; set; }
        public int UserGroupID { get; set; }
        public Nullable<bool> IsDefaultMenu { get; set; }
    }
    public class UserGroupMenuPermission
    {
        public Guid ID { get; set; }
        public int MenuID { get; set; }
        public int UserGroupID { get; set; }
        public bool IsActive { get; set; }
    }
}
