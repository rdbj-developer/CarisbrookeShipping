using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class HoldVentilationRecordFormHelper
    {
        public void SubmitHoldVentilationRecord(HoldVentilationRecordFormModal Modal)
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                HoldVentilationRecordForm dbModal = new HoldVentilationRecordForm();
                if (Modal != null && Modal.HoldVentilationRecordForm.HoldVentilationRecordFormId > 0)
                {
                    dbModal = dbContext.HoldVentilationRecordForms.Where(x => x.HoldVentilationRecordFormId == Modal.HoldVentilationRecordForm.HoldVentilationRecordFormId).FirstOrDefault();
                    dbModal.UpdatedDate = DateTime.Now;
                }
                else
                    dbContext.HoldVentilationRecordForms.Add(Modal.HoldVentilationRecordForm);
                dbContext.SaveChanges();
                if (Modal.HoldVentilationRecordList != null && Modal.HoldVentilationRecordList.Count > 0)
                {
                    foreach (var item in Modal.HoldVentilationRecordList)
                    {
                        dbContext.HoldVentilationRecordSheets.Add(new HoldVentilationRecordSheet
                        {
                            CreatedDate = DateTime.Now,
                            HODewPOint = item.HODewPOint,
                            HODryBulb = item.HODryBulb,
                            HoldVentilationRecordFormId = Modal.HoldVentilationRecordForm.HoldVentilationRecordFormId,
                            HOWetBulb = item.HOWetBulb,
                            HVRDate = item.HVRDate,
                            HVRTime = item.HVRTime,
                            IsVentilation = item.IsVentilation,
                            OUTDewPOint = item.OUTDewPOint,
                            OUTDryBulb = item.OUTDryBulb,
                            OUTWetBulb = item.OUTWetBulb,
                            Remarks = item.Remarks,
                            SeaTemp = item.SeaTemp
                        });
                        dbContext.SaveChanges();
                    }
                }
                LogHelper.writelog("SubmitHoldVentilationRecord : HoldVentilationRecord save");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitHoldVentilationRecord : " + ex.Message + " : " + ex.InnerException);
            }
        }
    }
}
