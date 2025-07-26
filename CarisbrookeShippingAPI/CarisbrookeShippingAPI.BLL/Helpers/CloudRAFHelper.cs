using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    // JSL 11/24/2022 added this class
    public class CloudRAFHelper
    {
        CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

        // JSL 11/24/2022
        public APIResponse SubmitRiskAssessmentFormData(RiskAssessmentFormModal Modal)
        {
            APIResponse res = new APIResponse();
            try
            {
                bool blnIsExist = false;
                RiskAssessmentFormLog dbLogModal = new RiskAssessmentFormLog();
                RiskAssessmentForm dbModal = new RiskAssessmentForm();

                if (Modal.RiskAssessmentForm != null)
                {
                    if (Modal.RiskAssessmentForm.RAFUniqueID != null && Modal.RiskAssessmentForm.RAFUniqueID != Guid.Empty) 
                    {
                        dbModal = dbContext.RiskAssessmentForms.Where(x => x.RAFUniqueID == Modal.RiskAssessmentForm.RAFUniqueID).FirstOrDefault();
                    }

                    if (dbModal != null)
                    {
                        blnIsExist = true;
                    }
                    else
                    {
                        dbModal = new RiskAssessmentForm();
                    }

                    dbModal.UpdatedDate = Modal.RiskAssessmentForm.UpdatedDate;
                    dbModal.ShipName = Modal.RiskAssessmentForm.ShipName;
                    dbModal.Number = Modal.RiskAssessmentForm.Number;
                    dbModal.Title = Modal.RiskAssessmentForm.Title;
                    dbModal.ReviewerName = Modal.RiskAssessmentForm.ReviewerName;
                    dbModal.ReviewerLocation = Modal.RiskAssessmentForm.ReviewerLocation;
                    dbModal.ReviewerRank = Modal.RiskAssessmentForm.ReviewerRank;
                    dbModal.ReviewerDate = Modal.RiskAssessmentForm.ReviewerDate;
                    dbModal.AmendmentDate = Modal.RiskAssessmentForm.AmendmentDate;
                    dbModal.IsAmended = Modal.RiskAssessmentForm.IsAmended;
                    dbModal.IsApplicable = Modal.RiskAssessmentForm.IsApplicable;

                    dbLogModal.RAFUniqueID = dbModal.RAFUniqueID;
                    dbLogModal.RAFId = dbModal.RAFID;
                    dbLogModal.CreatedBy = dbModal.CreatedBy;
                    dbLogModal.CreatedDate = dbModal.CreatedDate;
                    dbLogModal.UpdatedBy = dbModal.UpdatedBy;
                    dbLogModal.UpdatedDate = dbModal.UpdatedDate;

                    if (blnIsExist)
                    {
                        if ((Modal.RiskAssessmentForm.UpdatedDate > dbModal.UpdatedDate)
                        || dbModal.UpdatedDate == null
                        )
                        {
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        dbContext.RiskAssessmentForms.Add(Modal.RiskAssessmentForm);
                        dbContext.SaveChanges();
                    }

                    SaveRiskAssessmentFormHazard(Modal);
                    SaveRiskAssessmentFormReviewer(Modal);
                }
                res.result = AppStatic.SUCCESS;
            }
            catch (Exception ex)
            {
                res.result = AppStatic.ERROR;
                res.msg = ex.Message;
                LogHelper.writelog("SubmitRiskAssessmentForm : " + ex.Message + " : " + ex.InnerException);
            }
            return res;
        }
        // End JSL 11/24/2022

        // JSL 11/24/2022
        public void SaveRiskAssessmentFormHazard(RiskAssessmentFormModal Modal)
        {
            try
            {
                if (Modal.RiskAssessmentFormHazardList != null && Modal.RiskAssessmentFormHazardList.Count > 0)
                {
                    if (Modal != null && (Modal.RiskAssessmentForm.RAFUniqueID != null && Modal.RiskAssessmentForm.RAFUniqueID != Guid.Empty))    
                    {
                        List<RiskAssessmentFormHazard> dbHazardsModal = dbContext.RiskAssessmentFormHazards.Where(x => x.RAFUniqueID == Modal.RiskAssessmentForm.RAFUniqueID).ToList(); 
                        //Remove in DB
                        if (dbHazardsModal != null && dbHazardsModal.Count > 0)
                        {
                            foreach (var item in dbHazardsModal)
                            {
                                dbContext.RiskAssessmentFormHazards.Remove(item);
                            }
                            dbContext.SaveChanges();
                        }
                    }

                    //inser in DB
                    foreach (var item in Modal.RiskAssessmentFormHazardList)
                    {
                        dbContext.RiskAssessmentFormHazards.Add(new RiskAssessmentFormHazard
                        {
                            Id = Guid.NewGuid(),
                            HazardId = item.HazardId,
                            RAFUniqueID = Modal.RiskAssessmentForm.RAFUniqueID, 
                            RAFID = Modal.RiskAssessmentForm.RAFID,
                            Stage1Description = item.Stage1Description,
                            Stage2Severity = item.Stage2Severity,
                            Stage2Likelihood = item.Stage2Likelihood,
                            Stage2RiskFactor = item.Stage2RiskFactor,
                            Stage3Description = item.Stage3Description,
                            Stage4Severity = item.Stage4Severity,
                            Stage4Likelihood = item.Stage4Likelihood,
                            Stage4RiskFactor = item.Stage4RiskFactor
                        });

                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // End JSL 11/24/2022

        // JSL 11/24/2022
        public void SaveRiskAssessmentFormReviewer(RiskAssessmentFormModal Modal)
        {
            try
            {
                if (Modal.RiskAssessmentFormReviewerList != null && Modal.RiskAssessmentFormReviewerList.Count > 0)
                {
                    if (Modal != null && (Modal.RiskAssessmentForm.RAFUniqueID != null && Modal.RiskAssessmentForm.RAFUniqueID != Guid.Empty))
                    {
                        List<RiskAssessmentFormReviewer> dbReviewModal = dbContext.RiskAssessmentFormReviewers.Where(x => x.RAFUniqueID == Modal.RiskAssessmentForm.RAFUniqueID).ToList();
                        //Remove in DB
                        if (dbReviewModal != null && dbReviewModal.Count > 0)
                        {
                            foreach (var item in dbReviewModal)
                            {
                                dbContext.RiskAssessmentFormReviewers.Remove(item);
                            }
                            dbContext.SaveChanges();
                        }
                    }

                    //insert in DB
                    int count = 1;
                    foreach (var item in Modal.RiskAssessmentFormReviewerList)
                    {
                        dbContext.RiskAssessmentFormReviewers.Add(new RiskAssessmentFormReviewer
                        {
                            Id = Guid.NewGuid(),
                            RAFUniqueID = Modal.RiskAssessmentForm.RAFUniqueID,
                            RAFID = Modal.RiskAssessmentForm.RAFID,
                            ReviewerName = item.ReviewerName,
                            IndexNo = count
                        });
                        dbContext.SaveChanges();
                        count = count + 1;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // End JSL 11/24/2022

        // JSL 11/26/2022
        public List<RiskAssessmentFormModal> getUnsynchRAFList(string strShipCode)
        {
            List<RiskAssessmentFormModal> unSyncList = new List<RiskAssessmentFormModal>();
            try
            {
                var lstRAF = dbContext.RiskAssessmentForms
                    .Where(x => x.IsSynced == false
                    && x.RAFUniqueID != null
                    ).ToList();


                if (lstRAF != null && lstRAF.Count > 0)
                {
                    if (!string.IsNullOrEmpty(strShipCode))
                    {
                        lstRAF = lstRAF.Where(x => x.ShipCode == strShipCode).ToList();
                    }

                    foreach (var item in lstRAF)
                    {
                        RiskAssessmentFormModal dbModal = new RiskAssessmentFormModal();
                        dbModal.RiskAssessmentForm = item;
                        dbModal.RiskAssessmentFormHazardList = new List<RiskAssessmentFormHazard>();
                        dbModal.RiskAssessmentFormReviewerList = new List<RiskAssessmentFormReviewer>();

                        var RiskAssessmentFormHazardList = dbContext.RiskAssessmentFormHazards.Where(x => x.RAFUniqueID == item.RAFUniqueID).ToList();
                        if (RiskAssessmentFormHazardList != null && RiskAssessmentFormHazardList.Count > 0)
                        {
                            foreach (var itemRAFHaz in RiskAssessmentFormHazardList)
                            {
                                RiskAssessmentFormHazard mRAFHazard = new RiskAssessmentFormHazard();
                                mRAFHazard.Id = itemRAFHaz.Id;
                                mRAFHazard.HazardId = itemRAFHaz.HazardId;
                                mRAFHazard.RAFUniqueID = itemRAFHaz.RAFUniqueID;
                                mRAFHazard.Stage1Description = itemRAFHaz.Stage1Description;
                                mRAFHazard.Stage2Severity = itemRAFHaz.Stage2Severity;
                                mRAFHazard.Stage2Likelihood = itemRAFHaz.Stage2Likelihood;
                                mRAFHazard.Stage2RiskFactor = itemRAFHaz.Stage2RiskFactor;
                                mRAFHazard.Stage3Description = itemRAFHaz.Stage3Description;
                                mRAFHazard.Stage4Severity = itemRAFHaz.Stage4Severity;
                                mRAFHazard.Stage4Likelihood = itemRAFHaz.Stage4Likelihood;
                                mRAFHazard.Stage4RiskFactor = itemRAFHaz.Stage4RiskFactor;

                                dbModal.RiskAssessmentFormHazardList.Add(mRAFHazard);
                            }
                        }

                        var RiskAssessmentFormReviewerList = dbContext.RiskAssessmentFormReviewers.Where(x => x.RAFUniqueID == item.RAFUniqueID).ToList();
                        if (RiskAssessmentFormReviewerList != null && RiskAssessmentFormReviewerList.Count > 0)
                        {
                            foreach (var itemRAFRev in RiskAssessmentFormReviewerList)
                            {
                                RiskAssessmentFormReviewer mRAFReviewer = new RiskAssessmentFormReviewer();
                                mRAFReviewer.Id = itemRAFRev.Id;
                                mRAFReviewer.RAFUniqueID = itemRAFRev.RAFUniqueID;
                                mRAFReviewer.ReviewerName = itemRAFRev.ReviewerName;
                                mRAFReviewer.IndexNo = itemRAFRev.IndexNo;

                                dbModal.RiskAssessmentFormReviewerList.Add(mRAFReviewer);
                            }
                        }

                        unSyncList.Add(dbModal);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getUnsynchRAFList " + ex.Message.ToString() + "\n" + ex.InnerException.ToString());
                unSyncList = null;
            }
            return unSyncList;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public bool sendSynchRAFListUFID(List<string> IdsStr)
        {
            bool response = false;
            try
            {
                for (int i = 0; i < IdsStr.Count; i++) 
                {
                    Guid UFID = Guid.Parse(IdsStr[i]); 
                    Entity.RiskAssessmentForm rafForms = dbContext.RiskAssessmentForms.Where(x => x.RAFUniqueID == UFID).FirstOrDefault();
                    rafForms.IsSynced = true;
                }
                dbContext.SaveChanges();
                response = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud sendSynchRAFListUFID " + ex.Message + "\n" + ex.InnerException);
                response = false;
            }
            return response;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public List<RiskAssessmentFormDetails> getSynchRAFList(
             string strShipCode
            )
        {
            List<RiskAssessmentFormDetails> SyncList = new List<RiskAssessmentFormDetails>();
            try
            {
                SyncList = dbContext.RiskAssessmentForms
                    .Where(x => x.RAFUniqueID != null)
                    .Select(y => new RiskAssessmentFormDetails()
                    {
                        RAFUniqueID = y.RAFUniqueID,
                        UpdatedDate = y.UpdatedDate,
                        ShipCode = y.ShipCode,
                        IsSynced = y.IsSynced,
                    }).ToList();

                if (!string.IsNullOrEmpty(strShipCode))
                {
                    SyncList = SyncList.Where(x => x.ShipCode == strShipCode).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getSynchRAFList " + ex.Message + "\n" + ex.InnerException);
                SyncList = null;
            }
            return SyncList;
        }
        // End JSL 11/26/2022

        // JSL 11/26/2022
        public RiskAssessmentFormModal getSynchRAF(string RAFUniqueID)
        {
            RiskAssessmentFormModal dbModal = new RiskAssessmentFormModal();
            Guid UFormId = Guid.Parse(RAFUniqueID);
            try
            {
                var RAFForm = dbContext.RiskAssessmentForms.Where(x => x.RAFUniqueID == UFormId).FirstOrDefault();
                if (RAFForm != null)
                {
                    dbModal.RiskAssessmentForm = RAFForm;
                    dbModal.RiskAssessmentFormHazardList = new List<RiskAssessmentFormHazard>();
                    dbModal.RiskAssessmentFormReviewerList = new List<RiskAssessmentFormReviewer>();

                    var RiskAssessmentFormHazardList = dbContext.RiskAssessmentFormHazards.Where(x => x.RAFUniqueID == UFormId).ToList();
                    if (RiskAssessmentFormHazardList != null && RiskAssessmentFormHazardList.Count > 0)
                    {
                        foreach (var itemRAFHaz in RiskAssessmentFormHazardList)
                        {
                            RiskAssessmentFormHazard mRAFHazard = new RiskAssessmentFormHazard();
                            mRAFHazard.Id = itemRAFHaz.Id;
                            mRAFHazard.HazardId = itemRAFHaz.HazardId;
                            mRAFHazard.RAFUniqueID = itemRAFHaz.RAFUniqueID;
                            mRAFHazard.Stage1Description = itemRAFHaz.Stage1Description;
                            mRAFHazard.Stage2Severity = itemRAFHaz.Stage2Severity;
                            mRAFHazard.Stage2Likelihood = itemRAFHaz.Stage2Likelihood;
                            mRAFHazard.Stage2RiskFactor = itemRAFHaz.Stage2RiskFactor;
                            mRAFHazard.Stage3Description = itemRAFHaz.Stage3Description;
                            mRAFHazard.Stage4Severity = itemRAFHaz.Stage4Severity;
                            mRAFHazard.Stage4Likelihood = itemRAFHaz.Stage4Likelihood;
                            mRAFHazard.Stage4RiskFactor = itemRAFHaz.Stage4RiskFactor;

                            dbModal.RiskAssessmentFormHazardList.Add(mRAFHazard);
                        }
                    }

                    var RiskAssessmentFormReviewerList = dbContext.RiskAssessmentFormReviewers.Where(x => x.RAFUniqueID == UFormId).ToList();
                    if (RiskAssessmentFormReviewerList != null && RiskAssessmentFormReviewerList.Count > 0)
                    {
                        foreach (var itemRAFRev in RiskAssessmentFormReviewerList)
                        {
                            RiskAssessmentFormReviewer mRAFReviewer = new RiskAssessmentFormReviewer();
                            mRAFReviewer.Id = itemRAFRev.Id;
                            mRAFReviewer.RAFUniqueID = itemRAFRev.RAFUniqueID;
                            mRAFReviewer.ReviewerName = itemRAFRev.ReviewerName;
                            mRAFReviewer.IndexNo = itemRAFRev.IndexNo;

                            dbModal.RiskAssessmentFormReviewerList.Add(mRAFReviewer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Cloud getSynchRAF " + ex.Message + "\n" + ex.InnerException);
                dbModal = null;
            }
            return dbModal;
        }
        // End JSL 11/26/2022
    }
}
