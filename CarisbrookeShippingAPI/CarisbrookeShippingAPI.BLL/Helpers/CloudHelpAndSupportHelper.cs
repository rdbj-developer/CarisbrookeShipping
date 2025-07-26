using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    // RDBJ 01/01/2022 Added this class
    public class CloudHelpAndSupportHelper
    {
        // RDBJ 01/01/2022
        public bool InsertOrUpdateHelpAndSupportSynch(Modals.HelpAndSupport Modal)
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
                dbModal.CreatedBy = Modal.CreatedBy;
                dbModal.CreatedDateTime = Modal.CreatedDateTime;
                dbModal.ModifiedBy = Modal.ModifiedBy;
                dbModal.ModifiedDateTime = Modal.ModifiedDateTime;
                dbModal.IsDeleted = Modal.IsDeleted;
                dbModal.IsSynced = 1;

                if (blnIsExist)
                {
                    // RDBJ 01/01/2022 if local copy latest then it will update
                    if ((Modal.ModifiedDateTime > dbModal.ModifiedDateTime)
                        || dbModal.ModifiedDateTime == null
                        )
                    {
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    dbContext.HelpAndSupports.Add(dbModal);
                    dbContext.SaveChanges();
                }

                blnResponse = true;
            }
            catch (Exception ex)
            {
                blnResponse = false;
                LogHelper.writelog("InsertOrUpdateHelpAndSupportSynch : " + ex.Message);
            }
            return blnResponse;
        }
        // End RDBJ 01/01/2022

        // RDBJ 01/01/2022
        public List<BLL.Modals.HelpAndSupport> GetUnsynchHelpAndSupportList()
        {
            List<BLL.Modals.HelpAndSupport> unSyncList = new List<BLL.Modals.HelpAndSupport>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var lstHelpAndSupports = dbContext.HelpAndSupports
                    .Where(
                    //x => x.IsSynced == 0 && 
                    x => x.Id != null
                    ).ToList();

                if (lstHelpAndSupports != null && lstHelpAndSupports.Count > 0)
                {
                    foreach (var itemHs in lstHelpAndSupports)
                    {
                        BLL.Modals.HelpAndSupport objHS = new BLL.Modals.HelpAndSupport();

                        objHS.Id = itemHs.Id;
                        objHS.Comments = itemHs.Comments;
                        objHS.ShipId = itemHs.ShipId;
                        objHS.IsStatus = itemHs.IsStatus;
                        objHS.Priority = itemHs.Priority;
                        objHS.CreatedBy = itemHs.CreatedBy;
                        objHS.CreatedDateTime = itemHs.CreatedDateTime;
                        objHS.ModifiedBy = itemHs.ModifiedBy;
                        objHS.ModifiedDateTime = itemHs.ModifiedDateTime;
                        objHS.IsDeleted = itemHs.IsDeleted;
                        objHS.IsSynced = itemHs.IsSynced;

                        unSyncList.Add(objHS);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud GetUnsynchHelpAndSupportList " + ex.Message + "\n" + ex.InnerException);
                unSyncList = null;
            }
            return unSyncList;
        }
        // End RDBJ 01/01/2022

        // RDBJ 01/01/2022
        public bool UpdateCloudHelpAndSupportSynchStatus(string IdsStr)
        {
            bool response = false;
            string[] FormUID = IdsStr.Split(',');
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                for (int i = 0; i < FormUID.Length; i++)
                {
                    Guid UFID = Guid.Parse(FormUID[i]);
                    Entity.HelpAndSupport itemHS = dbContext.HelpAndSupports.Where(x => x.Id == UFID).FirstOrDefault();
                    itemHS.IsSynced = 1;
                }
                dbContext.SaveChanges();
                response = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud UpdateCloudHelpAndSupportSynchStatus " + ex.Message + "\n" + ex.InnerException);
                response = false;
            }
            return response;
        }
        // End RDBJ 01/01/2022

    }
}
