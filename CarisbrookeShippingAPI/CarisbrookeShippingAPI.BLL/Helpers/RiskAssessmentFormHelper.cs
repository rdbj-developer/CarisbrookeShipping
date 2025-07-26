using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class RiskAssessmentFormHelper
    {
        string ConnetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);
        public List<RiskAssessmentForm> GetRAFDrafts(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<RiskAssessmentForm> list = new List<RiskAssessmentForm>();
            try
            {
                List<Entity.RiskAssessmentForm> dbList = dbContext.RiskAssessmentForms.Where(x => x.ShipName == shipCode && x.IsSynced == true).ToList();
                list = dbList.Select(x => new RiskAssessmentForm()
                {
                    ShipCode = x.ShipCode,
                    ShipName = x.ShipName,
                    Number = x.Number,
                    Title = x.Title,
                    ReviewerName = x.ReviewerName,
                    ReviewerDate = x.ReviewerDate,
                    ReviewerRank = x.ReviewerRank,
                    ReviewerLocation = x.ReviewerLocation,
                    CreatedDate = x.CreatedDate,
                    RAFID = x.RAFID,
                    AmendmentDate = x.AmendmentDate,
                    IsAmended = x.IsAmended,
                    IsApplicable = x.IsApplicable
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAForm " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }

        //public RiskAssessmentFormModal RAFormDetailsView(string ShipName, int id)   // JSL 11/20/2022 commented this line
        public RiskAssessmentFormModal RAFormDetailsView(string ShipName, string id)   // JSL 11/20/2022
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            RiskAssessmentFormModal dbModal = new RiskAssessmentFormModal();
            //DocumentsModal dbDoc = new DocumentsModal();
            Guid guidID = new Guid(id);
            // var RAFId = dbContext.Documents.Where(x=> x.DocumentID == result).FirstOrDefault();

            var Modal1 = dbContext.RiskAssessmentForms
                .Where(x => (x.ShipName == ShipName || x.ShipCode == ShipName) 
                //&& x.RAFID == id) // JSL 11/20/2022 commented
                && x.RAFUniqueID == guidID)
                .FirstOrDefault();
            var Modal2 = dbContext.RiskAssessmentFormHazards
                //.Where(x => x.RAFID == id)  // JSL 11/20/2022 commented
                .Where(x => x.RAFUniqueID == guidID)  // JSL 11/20/2022
                .OrderBy(x => x.HazardId).ToList();
            var Modal3 = dbContext.RiskAssessmentFormReviewers
                //.Where(x => x.RAFID == id)  // JSL 11/20/2022 commented
                .Where(x => x.RAFUniqueID == guidID)  // JSL 11/20/2022
                .OrderBy(x => x.Id).ToList();

            if (Modal1 == null)
                dbModal.RiskAssessmentForm = new RiskAssessmentForm();
            else
                dbModal.RiskAssessmentForm = Modal1;
            if (Modal2 == null)
                dbModal.RiskAssessmentFormHazardList = new List<RiskAssessmentFormHazard>();
            else
                dbModal.RiskAssessmentFormHazardList = Modal2;
            if (Modal3 == null)
                dbModal.RiskAssessmentFormReviewerList = new List<RiskAssessmentFormReviewer>();
            else
                dbModal.RiskAssessmentFormReviewerList = Modal3;
            return dbModal;
        }

        public void SubmitRiskAssessmentFormData(RiskAssessmentFormModal Modal)
        {
            try
            {
                RiskAssessmentFormModal dbBackupObject = new RiskAssessmentFormModal
                {
                    RiskAssessmentForm = new RiskAssessmentForm(),
                    RiskAssessmentFormHazardList = new List<RiskAssessmentFormHazard>(),
                    RiskAssessmentFormReviewerList = new List<RiskAssessmentFormReviewer>()
                };
                RiskAssessmentFormLog dbLogModal = new RiskAssessmentFormLog();
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                RiskAssessmentForm dbModal = new RiskAssessmentForm();
                

                if (Modal == null)
                    return;

                if (Modal.RiskAssessmentForm != null)
                {
                    //if (Modal.RiskAssessmentForm.RAFID > 0) // JSL 11/22/2022 commented
                    if (Modal.RiskAssessmentForm.RAFUniqueID != null && Modal.RiskAssessmentForm.RAFUniqueID != Guid.Empty) // JSL 11/22/2022
                    {
                        //dbModal = dbContext.RiskAssessmentForms.Where(x => x.RAFID == Modal.RiskAssessmentForm.RAFID).FirstOrDefault(); // JSL 11/22/2022 commented
                        dbModal = dbContext.RiskAssessmentForms.Where(x => x.RAFUniqueID == Modal.RiskAssessmentForm.RAFUniqueID).FirstOrDefault(); // JSL 11/22/2022
                        if (dbModal != null)
                        {
                            dbBackupObject.RiskAssessmentForm = dbModal;
                            dbModal.UpdatedBy = Modal.RiskAssessmentForm.UpdatedBy; // JSL 11/26/2022
                            dbModal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
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
                            dbModal.IsSynced = Modal.RiskAssessmentForm.IsSynced;   // JSL 11/26/2022
                            dbLogModal.RAFUniqueID = dbModal.RAFUniqueID;   // JSL 11/22/2022
                            dbLogModal.RAFId = dbModal.RAFID;
                            dbLogModal.CreatedBy = dbModal.CreatedBy;
                            dbLogModal.CreatedDate = dbModal.CreatedDate;
                            dbLogModal.UpdatedBy = dbModal.UpdatedBy;
                            dbLogModal.UpdatedDate = dbModal.UpdatedDate;
                        }
                    }
                    else
                    {
                        dbModal = dbContext.RiskAssessmentForms.Where(x => x.ShipCode == Modal.RiskAssessmentForm.ShipCode && x.Title == Modal.RiskAssessmentForm.Title).FirstOrDefault();
                        if (dbModal != null)
                        {
                            dbBackupObject.RiskAssessmentForm = dbModal;
                            dbModal.UpdatedBy = Modal.RiskAssessmentForm.UpdatedBy; // JSL 11/26/2022
                            dbModal.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
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
                            dbModal.IsSynced = Modal.RiskAssessmentForm.IsSynced;   // JSL 11/26/2022
                            dbLogModal.CreatedBy = dbModal.CreatedBy;
                            dbLogModal.CreatedDate = dbModal.CreatedDate;
                            dbLogModal.RAFUniqueID = dbModal.RAFUniqueID;   // JSL 11/22/2022
                            dbLogModal.RAFId = dbModal.RAFID;
                            dbLogModal.UpdatedBy = dbModal.UpdatedBy;
                            dbLogModal.UpdatedDate = dbModal.UpdatedDate;
                        }
                        else
                        {
                            List<Document> dbList = new List<Document>();
                            dbList = dbContext.Documents.Where(x => x.IsDeleted == false && x.Title == Modal.RiskAssessmentForm.Title).ToList();
                            if (dbList.Count >= 0)
                            {
                                var number = Char.IsLetterOrDigit(Modal.RiskAssessmentForm.Number, 1);
                                if (number == false)
                                {
                                    Document parentId = new Document();
                                    List<Document> parentList = dbContext.Documents.Where(x => x.IsDeleted == false
                                    && x.Title.Substring(5, 1) == Modal.RiskAssessmentForm.Number.Substring(0, 1)).AsEnumerable().Where(e => e.Title.Any(char.IsDigit)).ToList();

                                    foreach (var item in parentList)
                                    {
                                        var data = item.Title.Substring(1, 1);
                                        if (data != "0")
                                        {
                                            parentId = dbContext.Documents.Where(x => x.DocID == item.DocID).FirstOrDefault();
                                            Modal.RiskAssessmentForm.ParentID = parentId.DocumentID;
                                        }
                                        else
                                        {
                                            RiskAssessmentForm parentLists = dbContext.RiskAssessmentForms.Where(x => x.Title.Substring(5, 1) == Modal.RiskAssessmentForm.Number.Substring(0, 1)).AsEnumerable().Where(e => e.Title.Any(char.IsDigit)).FirstOrDefault();
                                            Modal.RiskAssessmentForm.ParentID = parentLists.DocumentID;
                                        }
                                    }
                                    Modal.RiskAssessmentForm.RAFUniqueID = Guid.NewGuid();  // JSL 11/22/2022
                                    Modal.RiskAssessmentForm.DocumentID = Guid.NewGuid();
                                    Modal.RiskAssessmentForm.SectionType = "ISM";
                                }
                                else
                                {
                                    Document parentId = dbContext.Documents.Where(x => x.IsDeleted == false && x.Title == "Risk Assessments").FirstOrDefault();
                                    Modal.RiskAssessmentForm.RAFUniqueID = Guid.NewGuid();  // JSL 11/22/2022
                                    Modal.RiskAssessmentForm.DocumentID = Guid.NewGuid();
                                    Modal.RiskAssessmentForm.ParentID = parentId.DocumentID;
                                    //Modal.RiskAssessmentForm.Title = Modal.RiskAssessmentForm.Number + " - " + Modal.RiskAssessmentForm.Title;
                                    //Modal.RiskAssessmentForm.Type = "FOLDER";
                                    Modal.RiskAssessmentForm.SectionType = "ISM";
                                }

                            }
                            dbContext.RiskAssessmentForms.Add(Modal.RiskAssessmentForm);
                        }
                    }
                    dbContext.SaveChanges();
                }                
               
                if (Modal.RiskAssessmentFormHazardList != null && Modal.RiskAssessmentFormHazardList.Count > 0)
                {
                    //if (Modal != null && Modal.RiskAssessmentForm.RAFID > 0)    // JSL 11/22/2022 commented
                    if (Modal != null && (Modal.RiskAssessmentForm.RAFUniqueID != null && Modal.RiskAssessmentForm.RAFUniqueID != Guid.Empty))    // JSL 11/22/2022
                    {
                        //List<RiskAssessmentFormHazard> dbHazardsModal = dbContext.RiskAssessmentFormHazards.Where(x => x.RAFID == Modal.RiskAssessmentForm.RAFID).ToList(); // JSL 11/22/2022 commented
                        List<RiskAssessmentFormHazard> dbHazardsModal = dbContext.RiskAssessmentFormHazards.Where(x => x.RAFUniqueID == Modal.RiskAssessmentForm.RAFUniqueID).ToList(); // JSL 11/22/2022
                        //Remove in DB
                        if (dbHazardsModal != null && dbHazardsModal.Count > 0)
                        {
                            dbBackupObject.RiskAssessmentFormHazardList = dbHazardsModal;
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
                            RAFUniqueID = Modal.RiskAssessmentForm.RAFUniqueID, // JSL 11/22/2022
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

                if (Modal.RiskAssessmentFormReviewerList != null && Modal.RiskAssessmentFormReviewerList.Count > 0)
                {
                    //if (Modal != null && Modal.RiskAssessmentForm.RAFID > 0)    // JSL 11/22/2022 commented
                    if (Modal != null && (Modal.RiskAssessmentForm.RAFUniqueID != null && Modal.RiskAssessmentForm.RAFUniqueID != Guid.Empty))    // JSL 11/22/2022
                    {
                        //List<RiskAssessmentFormReviewer> dbReviewModal = dbContext.RiskAssessmentFormReviewers.Where(x => x.RAFID == Modal.RiskAssessmentForm.RAFID).ToList();  // JSL 11/22/2022 commented
                        List<RiskAssessmentFormReviewer> dbReviewModal = dbContext.RiskAssessmentFormReviewers.Where(x => x.RAFUniqueID == Modal.RiskAssessmentForm.RAFUniqueID).ToList();  // JSL 11/22/2022
                        //Remove in DB
                        if (dbReviewModal != null && dbReviewModal.Count > 0)
                        {
                            dbBackupObject.RiskAssessmentFormReviewerList = dbReviewModal;
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
                        //if (item.ReviewerName != null)
                        //{
                        dbContext.RiskAssessmentFormReviewers.Add(new RiskAssessmentFormReviewer
                        {
                            Id = Guid.NewGuid(),
                            RAFUniqueID = Modal.RiskAssessmentForm.RAFUniqueID, // JSL 11/22/2022
                            RAFID = Modal.RiskAssessmentForm.RAFID,
                            ReviewerName = item.ReviewerName,
                            IndexNo = count
                        });
                        dbContext.SaveChanges();
                        count = count + 1;
                        // }
                    }
                }
                try
                {
                    if (dbLogModal != null)
                    {
                        dbLogModal.ShipCode = Modal.RiskAssessmentForm.ShipCode;
                        dbLogModal.ShipName = Modal.RiskAssessmentForm.ShipName;
                        dbLogModal.Id = Guid.NewGuid();
                        dbLogModal.RiskAssessmentFormList = JsonConvert.SerializeObject(dbBackupObject);
                        dbContext.RiskAssessmentFormLogs.Add(dbLogModal);
                        dbContext.SaveChanges();
                    }
                }
                catch (Exception)
                {

                }
                LogHelper.writelog("SubmitRiskAssessmentForm : RiskAssessmentForm save");

            }
            catch (Exception ex)
            {
                LogHelper.writelog("SubmitRiskAssessmentForm : " + ex.Message + " : " + ex.InnerException);
            }
        }
        public bool CheckRAFNumberExistFromData(string ShipCode, string RAFNumber)
        {
            bool result = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                RiskAssessmentForm dbModal = new RiskAssessmentForm();
                dbModal = dbContext.RiskAssessmentForms.Where(x => x.ShipCode == ShipCode && x.Number == RAFNumber).FirstOrDefault();
                if (dbModal != null && !string.IsNullOrWhiteSpace(dbModal.Number))
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("CheckRAFNumberExistFromData " + ex.Message);
            }
            return result;
        }
        public List<RiskAssessmentForm> GetAllDocumentRiskassessment(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<RiskAssessmentForm> list = new List<RiskAssessmentForm>();
            try
            {
                return dbContext.RiskAssessmentForms.Where(x => x.ShipCode == shipCode).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRAForm " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }

        public bool InsertDocumentsBulkDataInRiskAssesmentHazared(List<RiskAssessmentFormHazard> AllDocuments)
        {
            bool result = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                List<RiskAssessmentFormHazard> RiskAssessments = FilterRiskAssessmentHazard(AllDocuments);
                if (RiskAssessments != null && RiskAssessments.Count > 0)
                {
                    DataTable dt = Utility.ToDataTable(RiskAssessments);
                    using (SqlConnection connection = new SqlConnection(ConnetionString))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                        bulkCopy.DestinationTableName = "RiskAssessmentFormHazard";
                        connection.Open();
                        bulkCopy.WriteToServer(dt);
                        connection.Close();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssesmentHazared " + ex.Message);
            }
            return result;
        }

        public bool InsertDocumentsBulkDataInRiskAssessmentReviewer(List<RiskAssessmentFormReviewer> AllDocuments)
        {
            bool result = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                List<RiskAssessmentFormReviewer> RiskAssessments = FilterRiskAssessmentReviewer(AllDocuments);
                if (RiskAssessments != null && RiskAssessments.Count > 0)
                {
                    DataTable dt = Utility.ToDataTable(RiskAssessments);
                    using (SqlConnection connection = new SqlConnection(ConnetionString))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                        bulkCopy.DestinationTableName = "RiskAssessmentFormReviewer";
                        connection.Open();
                        bulkCopy.WriteToServer(dt);
                        connection.Close();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssessmentReviewer " + ex.Message);
            }
            return result;
        }

        public bool InsertDocumentsBulkDataInRiskAssessment(List<RiskAssessmentForm> AllDocuments)
        {
            bool result = false;
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                RiskAssessmentForm dbModal = new RiskAssessmentForm();
                List<RiskAssessmentForm> RiskAssessments = FilterRiskAssessment(AllDocuments);
                if (RiskAssessments != null && RiskAssessments.Count > 0)
                {
                    DataTable dt = Utility.ToDataTable(RiskAssessments);
                    using (SqlConnection connection = new SqlConnection(ConnetionString))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock |
                            SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, null);

                        bulkCopy.DestinationTableName = "RiskAssessmentForm";
                        connection.Open();
                        bulkCopy.WriteToServer(dt);
                        connection.Close();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("InsertDocumentsBulkDataInRiskAssessment " + ex.Message);
            }
            return result;
        }

        public List<RiskAssessmentForm> FilterRiskAssessment(List<RiskAssessmentForm> Documents)
        {
            try
            {
                List<RiskAssessmentForm> FilteredDocuments = new List<RiskAssessmentForm>();
                foreach (var item in Documents)
                {
                    RiskAssessmentForm dbDoc = GetDocumentRiskassessmentBYID(item.RAFID, item.ShipCode);
                    if (dbDoc != null && dbDoc.RAFID > 0)
                    {

                    }
                    else
                    {
                        FilteredDocuments.Add(item);
                    }
                }
                return FilteredDocuments;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("FilterDocuments Error : " + ex.Message);
                return null;
            }
        }

        public RiskAssessmentForm GetDocumentRiskassessmentBYID(long RAFID, string ShipCode)
        {
            RiskAssessmentForm response = new RiskAssessmentForm();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                response = dbContext.RiskAssessmentForms.Where(x => x.ShipCode == ShipCode && x.RAFID == RAFID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentRiskassessmentBYID Error : " + ex.Message);
            }
            return response;
        }

        public List<RiskAssessmentFormHazard> FilterRiskAssessmentHazard(List<RiskAssessmentFormHazard> Documents)
        {
            try
            {
                List<RiskAssessmentFormHazard> FilteredDocuments = new List<RiskAssessmentFormHazard>();
                foreach (var item in Documents)
                {
                    RiskAssessmentFormHazard dbDoc = GetDocumentRiskAssessmentFormHazardBYID(item.RAFID);
                    if (dbDoc != null && dbDoc.RAFID > 0)
                    {
                        FilteredDocuments.Add(item);
                    }
                    else
                    {
                        FilteredDocuments.Add(item);
                    }
                }
                return FilteredDocuments;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("FilterDocuments Error : " + ex.Message);
                return null;
            }
        }
        public RiskAssessmentFormHazard GetDocumentRiskAssessmentFormHazardBYID(long? RAFID)
        {
            RiskAssessmentFormHazard response = new RiskAssessmentFormHazard();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                response = dbContext.RiskAssessmentFormHazards.Where(x => x.RAFID == RAFID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentRiskAssessmentFormHazardBYID Error : " + ex.Message);
            }
            return response;
        }

        public List<RiskAssessmentFormReviewer> FilterRiskAssessmentReviewer(List<RiskAssessmentFormReviewer> Documents)
        {
            try
            {
                List<RiskAssessmentFormReviewer> FilteredDocuments = new List<RiskAssessmentFormReviewer>();
                foreach (var item in Documents)
                {
                    RiskAssessmentFormReviewer dbDoc = GetDocumentRiskassessmentReviewerBYID(item.RAFID);
                    if (dbDoc != null && dbDoc.RAFID > 0)
                    {
                        FilteredDocuments.Add(item);
                    }
                    else
                    {
                        FilteredDocuments.Add(item);
                    }
                }
                return FilteredDocuments;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("FilterDocuments Error : " + ex.Message);
                return null;
            }
        }

        public RiskAssessmentFormReviewer GetDocumentRiskassessmentReviewerBYID(long? RAFID)
        {
            RiskAssessmentFormReviewer response = new RiskAssessmentFormReviewer();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                response = dbContext.RiskAssessmentFormReviewers.Where(x => x.RAFID == RAFID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetDocumentRiskassessmentReviewerBYID Error : " + ex.Message);
            }
            return response;
        }

        public List<RiskAssessmentReviewLog> GetAllRiskAssessmentReviewLog(string shipCode = "")
        {
            List<RiskAssessmentReviewLog> DocList = new List<RiskAssessmentReviewLog>();
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);
            try
            {
                //string Query = " SELECT R.*,Stage4RiskFactor,Orders " +
                //                " FROM (SELECT R.Number,R.Title,AmendmentDate,ISNULL(R.IsApplicable,1) as IsApplicable, ReviewerDate,R.RAFID,  " +
                //                "             row_number() OVER (PARTITION BY R.Number,R.Title Order By IsNULL(UpdatedDate,CreatedDate) desc) as cnt  " +
                //                "       FROM RiskAssessmentForm  R Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log')) R	 " +
                //                " 	 Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard " +
                //                " 		Where RAFID = R.RAFID Order By Stage4RiskFactor desc )X  " +
                //                " 	 Outer Apply(Select * from (  " +
                //                " 			Select '01 - General' as Title, 1 as Orders, 'G' as Category Union Select '02 - Deck Department' as Title , 2 as Orders, 'D' as Category  " +
                //                " 			Union Select '03 - Engine Department' as Title, 3 as Orders, 'E' as Category  " +
                //                " 			Union Select '04 - Catering' as Title, 4 as Orders, 'C' as Category  " +
                //                " 			Union Select '05 - Subcontractors' as Title, 5 as Orders, 'S' as Category ) as Groups  " +
                //                " 			Where Groups.Category = SUBSTRING (LTRIM(R.Number),1,1) )Y   " +
                //                " WHERE R.cnt = 1 " +
                //                " Order By Orders,Cast(SUBSTRING (LTRIM(Number),2,LEN(Number)) as int); ";

                // JSL 11/20/2022 commented
                /*
                string Query = " SELECT R.*,Stage4RiskFactor,Orders " +
                             " FROM (SELECT R.Number,R.Title,AmendmentDate,ISNULL(R.IsApplicable,1) as IsApplicable, ReviewerDate,R.RAFID,  " +
                             "             row_number() OVER (PARTITION BY R.Number,R.Title Order By IsNULL(UpdatedDate,CreatedDate) desc) as cnt  " +
                             "       FROM RiskAssessmentForm  R Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log')) R	 " +
                             " 	 Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard " +
                             " 		Where RAFID = R.RAFID Order By Stage4RiskFactor desc )X  " +
                             " 	 Outer Apply(Select * from (  " +
                             "                  Select Title,ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS Orders, SUBSTRING (TRIM(Title),6,1) as Category from Documents where ParentID='0FC3BDB8-0D98-4614-81D3-7918D01BFFF2' ) as Groups  " +
                             " 			Where Groups.Category = SUBSTRING (LTRIM(R.Number),1,1) )Y   " +
                             " WHERE R.cnt = 1 " +
                             " Order By Orders,Cast(SUBSTRING (LTRIM(Number),2,LEN(Number)) as int); ";
                */
                // End JSL 11/20/2022 commented

                // JSL 11/20/2022
                string Query = "SELECT R.*,Stage4RiskFactor,Orders" +
                    " FROM (SELECT R.Number,R.Title,AmendmentDate,ISNULL(R.IsApplicable,1) as IsApplicable, ReviewerDate," +
                    " R.RAFID, R.RAFUniqueID," +
                    " row_number() OVER (PARTITION BY R.Number,R.Title Order By IsNULL(UpdatedDate,CreatedDate) desc) as cnt" +
                    " FROM RiskAssessmentForm  R Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log')) R" +
                    " Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard" +
                    //"Where RAFID = R.RAFID" +
                    " WHERE RAFUniqueID = R.RAFUniqueID" +
                    " Order By Stage4RiskFactor desc )X" +
                    " Outer Apply(Select * from (" +
                    " Select Title,ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS Orders, SUBSTRING (TRIM(Title),6,1) as Category from Documents where ParentID='0FC3BDB8-0D98-4614-81D3-7918D01BFFF2' ) as Groups" +
                    " Where Groups.Category = SUBSTRING (LTRIM(R.Number),1,1) )Y" +
                    " WHERE R.cnt = 1" +
                    //" Order By Orders,Cast(SUBSTRING (LTRIM(Number),2,LEN(Number)) as int);";   // JSL 01/13/2023 commented
                    " Order By Orders;";    // JSL 01/13/2023
                // End JSL 11/20/2022

                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("IsAvailable");
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        if (!string.IsNullOrEmpty(shipCode))
                            sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", shipCode);

                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DocList = dt.ToListof<RiskAssessmentReviewLog>();
                            }
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                string Query = "Select R.Number,Title,AmendmentDate,Stage4RiskFactor, ISNULL(R.IsApplicable,1) as IsApplicable,R.RAFID," +
                    " R.RAFUniqueID" +  // JSL 11/20/2022
                    " from RiskAssessmentForm  R" +
                    " Outer Apply( Select top 1 Stage4RiskFactor FROM RiskAssessmentFormHazard" +
                    //" Where RAFID = R.RAFID" +    // JSL 11/20/2022 commented
                    " Where RAFUniqueID = R.RAFUniqueID" +  // JSL 11/20/2022
                    " Order By Stage4RiskFactor desc )X" +
                    " Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log')";
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("IsAvailable");
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);

                        sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", ((object)shipCode) ?? DBNull.Value);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                DocList = dt.ToListof<RiskAssessmentReviewLog>();
                            }
                        conn.Close();
                    }
                }
                LogHelper.writelog("GetAllRiskAssessmentReviewLogFromLocalDB " + ex.Message);
            }
            return DocList;
        }
        public RiskAssessmentDataList GetRiskAssessmentDataToSeupShipApp(string shipCode)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            RiskAssessmentDataList list = new RiskAssessmentDataList();
            try
            {
                List<RiskAssessmentFormHazard> HazardList = new List<RiskAssessmentFormHazard>();
                List<RiskAssessmentFormReviewer> ReviewerList = new List<RiskAssessmentFormReviewer>();
                List<RiskAssessmentForm> AllDocs = new List<RiskAssessmentForm>();
                string connetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);
                //string Query = "Select * from RiskAssessmentForm Where ShipCode=@ShipCode ";
                string Query = "Select R.* FROM(SELECT R.*,row_number() OVER (PARTITION BY R.Number,R.Title Order By IsNULL(UpdatedDate,CreatedDate) desc) as cnt " +
                " FROM RiskAssessmentForm  R Where ShipCode = @ShipCode AND R.Title NOT IN('Review Instructions', 'Review Log')) R WHERE R.cnt = 1  ";
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        LogHelper.writelog("IsAvailable");
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", ((object)shipCode) ?? DBNull.Value);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            AllDocs = dt.ToListof<RiskAssessmentForm>();
                        }
                        conn.Close();
                    }
                }
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        var dt = new DataTable();
                        //Query = "Select H.* from RiskAssessmentFormHazard H with(nolock) Inner Join RiskAssessmentForm R with(nolock) on R.RAFID = H.RAFID " +
                        //    " Where R.ShipCode=@ShipCode ";
                        Query = " Select H.* from RiskAssessmentFormHazard H with(nolock) Inner Join (SELECT R.*,row_number() OVER (PARTITION BY R.Number,R.Title Order By IsNULL(UpdatedDate,CreatedDate) desc) as cnt   " +
                        " FROM RiskAssessmentForm  R Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log')) R	ON R.RAFID = H.RAFID WHERE R.cnt = 1   ";
                        var sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", ((object)shipCode) ?? DBNull.Value);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            HazardList = dt.ToListof<RiskAssessmentFormHazard>();
                        }
                    }
                }
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        var dt = new DataTable();
                        //Query = "Select RV.* from RiskAssessmentFormReviewer RV with(nolock) Inner Join RiskAssessmentForm R with(nolock) on R.RAFID = RV.RAFID " +
                        //    " Where R.ShipCode=@ShipCode ";
                        Query = " Select RV.* from RiskAssessmentFormReviewer RV with(nolock) Inner Join (SELECT R.*,row_number() OVER (PARTITION BY R.Number,R.Title Order By IsNULL(UpdatedDate,CreatedDate) desc) as cnt  " +
                        " FROM RiskAssessmentForm  R Where ShipCode=@ShipCode AND R.Title NOT IN('Review Instructions','Review Log')) R	ON R.RAFID = RV.RAFID WHERE R.cnt = 1 ";

                        var sqlAdp = new SqlDataAdapter(Query, conn);
                        sqlAdp.SelectCommand.Parameters.AddWithValue("@ShipCode", ((object)shipCode) ?? DBNull.Value);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            ReviewerList = dt.ToListof<RiskAssessmentFormReviewer>();
                        }
                        conn.Close();
                    }
                }
                list.RiskAssessmentList = AllDocs;
                list.RiskAssessmentFormHazardList = HazardList;
                list.RiskAssessmentFormReviewerList = ReviewerList;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetRiskAssessmentDataToSeupShipApp " + ex.Message + "\n" + ex.InnerException);
            }
            return list;
        }
    }
}
