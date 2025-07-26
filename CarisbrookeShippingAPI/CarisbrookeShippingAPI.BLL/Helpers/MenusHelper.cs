using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class MenusHelper
    {
        public List<Menu> GetAllMenus()
        {
            List<Menu> res = new List<Menu>();
            try
            {
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    res = dbContext.Menus.OrderBy(x => x.MenuId).ToList();                   
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
    }
}
