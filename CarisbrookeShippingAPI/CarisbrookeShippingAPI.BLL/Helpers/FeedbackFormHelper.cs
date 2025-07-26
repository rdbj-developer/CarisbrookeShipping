using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class FeedbackFormHelper
    {
        public void SubmitFeedbackForm(FeedbackForm Modal)
        {
            try
            {
                if (Modal.Id == Guid.Empty)
                    Modal.Id = Guid.NewGuid();
                if (!Modal.CreatedDate.HasValue)
                    Modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                dbContext.FeedbackForms.Add(Modal);
                dbContext.SaveChanges();
                LogHelper.writelog("SubmitFeedbackForm save");
            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitFeedbackForm : " + ex.Message + " : " + ex.InnerException);
            }
        }
    }
}
