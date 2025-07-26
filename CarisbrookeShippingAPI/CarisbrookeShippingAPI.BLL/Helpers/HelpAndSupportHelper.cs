using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    // RDBJ 12/27/2021 Added Helper class
    public class HelpAndSupportHelper
    {
        // RDBJ 12/28/2021
        public List<Modals.HelpAndSupport> GetHelpAndSupportsList()
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<Modals.HelpAndSupport> list = new List<Modals.HelpAndSupport>();
            try
            {
                var data = from HS in dbContext.HelpAndSupports.Where(x => x.IsDeleted == 0)
                            join CS in dbContext.CSShips on HS.ShipId equals CS.Code
                            select new
                            {
                                Id = HS.Id,
                                ShipName = CS.Name,
                                Comment = HS.Comments,
                                IsStatus = HS.IsStatus,
                                Priority = HS.Priority,
                                CreatedBy = HS.CreatedBy,
                                CreatedDateTime = HS.CreatedDateTime,
                            };

                foreach (var item in data)
                {
                    Modals.HelpAndSupport obj = new Modals.HelpAndSupport();
                    obj.Id = item.Id;
                    obj.ShipId = item.ShipName;
                    obj.Comments = item.Comment;
                    //obj.IsStatus = item.IsStatus; // RDBJ 12/29/2021 commented this line
                    //obj.Priority = item.Priority; // RDBJ 12/29/2021 commented this line

                    // RDBJ 12/29/2021
                    obj.StrStatus = item.IsStatus == 1 ? "Closed" : "Open";
                    string strSetPriority = string.Empty;
                    if (item.Priority == 1)
                    {
                        strSetPriority = "Low";
                    }
                    else if (item.Priority == 2)
                    {
                        strSetPriority = "Medium";
                    }
                    else if (item.Priority == 3)
                    {
                        strSetPriority = "High";
                    }
                    else if (item.Priority == 4)
                    {
                        strSetPriority = "Critical";
                    }
                    obj.StrPriority = strSetPriority;
                    // End RDBJ 12/29/2021

                    obj.CreatedBy = item.CreatedBy;
                    obj.CreatedDateTime = item.CreatedDateTime;
                    list.Add(obj);
                }
                list = list.OrderByDescending(x => x.CreatedDateTime).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetHelpAndSupportsList " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
        // End RDBJ 12/28/2021

        // RDBJ 12/27/2021
        public bool InsertOrUpdateHelpAndSupport(Modals.HelpAndSupport Modal)
        {
            bool blnResponse = false;
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                Entity.HelpAndSupport dbModal = new Entity.HelpAndSupport();
                bool blnIsExist = false;
                if (Modal != null && Modal.Id != null)
                {
                    if (Modal.Id != null)
                    {
                        dbModal = dbContext.HelpAndSupports.Where(x => x.Id == Modal.Id).FirstOrDefault();
                    }
                }
                if (dbModal != null)
                {
                    blnIsExist = true;
                }
                else
                {
                    dbModal = new HelpAndSupport();
                }

                dbModal.Id = Modal.Id;
                dbModal.Comments = Modal.Comments;
                dbModal.ShipId = Modal.ShipId;
                dbModal.IsStatus = Modal.IsStatus;
                dbModal.Priority = Modal.Priority;
                dbModal.ModifiedBy = Modal.ModifiedBy;
                dbModal.ModifiedDateTime = Utility.ToDateTimeUtcNow();
                dbModal.IsSynced = 0;

                if (blnIsExist)
                {
                    dbContext.SaveChanges();
                }
                else
                {
                    dbModal.CreatedBy = Modal.CreatedBy;
                    dbModal.CreatedDateTime = Utility.ToDateTimeUtcNow();
                    dbModal.IsDeleted = 0;
                    dbContext.HelpAndSupports.Add(dbModal);
                    dbContext.SaveChanges();
                }

                blnResponse = true;
            }
            catch (Exception ex)
            {
                blnResponse = false;
                LogHelper.writelog("InsertOrUpdateHelpAndSupport : " + ex.Message);
            }
            return blnResponse;
        }
        // End RDBJ 12/27/2021

        // RDBJ 12/29/2021
        public bool DeleteHelpAndSupport(string ID, string ModifiedBy)
        {
            bool blnResponse = false;
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                Guid guidID = Guid.Parse(ID);
                Entity.HelpAndSupport dbModal = dbContext.HelpAndSupports.Where(x => x.Id == guidID).FirstOrDefault();
                if (dbModal != null)
                {
                    dbModal.IsDeleted = 1;
                    dbModal.IsSynced = 0;
                    dbModal.ModifiedBy = ModifiedBy;
                    dbModal.ModifiedDateTime = Utility.ToDateTimeUtcNow();

                    dbContext.SaveChanges();
                }
                blnResponse = true;
            }
            catch (Exception ex)
            {
                blnResponse = false;
                LogHelper.writelog("DeleteHelpAndSupport : " + ex.Message);
            }
            return blnResponse;
        }
        // End RDBJ 12/29/2021
    }
}
