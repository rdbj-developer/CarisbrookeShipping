using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarisbrookeShippingAPI.Entity;
using CarisbrookeShippingAPI.BLL.Modals;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class SMRHelper
    {
        public void SubmitSMR(SMRModal Modal)
        {
            SMRForm dbModal = new SMRForm();
            dbModal.ShipID = Modal.ShipID;
            dbModal.ShipName = Modal.ShipName;
            dbModal.ReviewPeriod = Modal.ReviewPeriod;
            dbModal.Year = Modal.Year;
            dbModal.DateOfMeeting = Modal.DateOfMeeting;
            dbModal.Section1 = Modal.Section1;
            dbModal.Section2 = Modal.Section2;
            dbModal.Section3 = Modal.Section3;
            dbModal.Section4 = Modal.Section4;
            dbModal.Section5 = Modal.Section5;
            dbModal.Section6 = Modal.Section6;
            dbModal.Section7 = Modal.Section7;
            dbModal.Section7a = Modal.Section7a;
            dbModal.Section7b = Modal.Section7b;
            dbModal.Section7c = Modal.Section7c;
            dbModal.Section7d = Modal.Section7d;
            dbModal.Section7e1 = Modal.Section7e1;
            dbModal.Section7e2 = Modal.Section7e2;
            dbModal.Section7e3 = Modal.Section7e3;
            dbModal.Section7f1 = Modal.Section7f1;
            dbModal.Section7f2 = Modal.Section7f2;
            dbModal.Section7g = Modal.Section7g;
            dbModal.Section7h = Modal.Section7h;
            dbModal.Section8a = Modal.Section8a;
            dbModal.Section8b = Modal.Section8b;
            dbModal.Section8b1 = Modal.Section8b1;
            dbModal.Section8b2 = Modal.Section8b2;
            dbModal.Section8b3 = Modal.Section8b3;
            dbModal.Section8b4 = Modal.Section8b4;
            dbModal.Section8b5 = Modal.Section8b5;
            dbModal.Section9 = Modal.Section9;
            dbModal.Section10 = Modal.Section10;
            dbModal.Section11 = Modal.Section11;
            dbModal.Section12a = Modal.Section12a;
            dbModal.Section12b = Modal.Section12b;
            dbModal.Section12c = Modal.Section12c;
            dbModal.Section12d = Modal.Section12d;
            dbModal.Section12e = Modal.Section12e;
            dbModal.Section12f = Modal.Section12f;
            dbModal.Section12g = Modal.Section12g;
            dbModal.Section12h = Modal.Section12h;
            dbModal.Section12i = Modal.Section12i;
            dbModal.Section12j = Modal.Section12j;
            dbModal.Section12k = Modal.Section12k;
            dbModal.Section13 = Modal.Section13;
            dbModal.CreatedDate = Modal.CreatedDate;
            dbModal.UpdatedDate = Modal.UpdatedDate;
            if (Modal.SMRFormCrewMemberList != null && Modal.SMRFormCrewMemberList.Count > 0)
            {
                dbModal.SMRFormCrewMembers = new List<SMRFormCrewMember>();
                foreach (var item in Modal.SMRFormCrewMemberList)
                {
                    SMRFormCrewMember member = new SMRFormCrewMember();
                    member.Rank = item.Rank;
                    member.FullName = item.FullName;
                    member.ElectedAsSafety = item.ElectedAsSafety;
                    member.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    member.UpdatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbModal.SMRFormCrewMembers.Add(member);
                }
            }
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            dbContext.SMRForms.Add(dbModal);
            dbContext.SaveChanges();
        }
        public List<SMRModal> GetSMRFormsFilled(SMRFormReq value)
        {
            List<SMRForm> SMRList = new List<SMRForm>();
            List<SMRModal> SMRModalList = new List<SMRModal>();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                if (value.Year > 0 && !string.IsNullOrEmpty(value.ReviewPeriod))
                {
                    SMRList = dbContext.SMRForms.Where(x => x.Year == value.Year && x.ReviewPeriod == value.ReviewPeriod).ToList();
                }
                else if (string.IsNullOrEmpty(value.ReviewPeriod))
                    SMRList = dbContext.SMRForms.Where(x => x.Year == value.Year).ToList();

                SMRModalList = SMRList.Select(x => new SMRModal()
                {
                    SMRFormID = x.SMRFormID,
                    ShipID = x.ShipID,
                    ShipName = x.ShipName,
                    Year = x.Year,
                    ReviewPeriod = x.ReviewPeriod,
                    DateOfMeeting = x.DateOfMeeting,
                    Section1 = x.Section1,
                    Section2 = x.Section2,
                    Section3 = x.Section3,
                    Section4 = x.Section4,
                    Section5 = x.Section5,
                    Section6 = x.Section6,
                    Section7 = x.Section7,
                    Section7a = x.Section7a,
                    Section7b = x.Section7b,
                    Section7c = x.Section7c,
                    Section7d = x.Section7d,
                    Section7e1 = x.Section7e1,
                    Section7e2 = x.Section7e2,
                    Section7e3 = x.Section7e3,
                    Section7f1 = x.Section7f1,
                    Section7f2 = x.Section7f2,
                    Section7g = x.Section7g,
                    Section7h = x.Section7h,
                    Section8a = x.Section8a,
                    Section8b = x.Section8b,
                    Section8b1 = x.Section8b1,
                    Section8b2 = x.Section8b2,
                    Section8b3 = x.Section8b3,
                    Section8b4 = x.Section8b4,
                    Section8b5 = x.Section8b5,
                    Section9 = x.Section9,
                    Section10 = x.Section10,
                    Section11 = x.Section11,
                    Section12a = x.Section12a,
                    Section12b = x.Section12b,
                    Section12c = x.Section12c,
                    Section12d = x.Section12d,
                    Section12e = x.Section12e,
                    Section12f = x.Section12f,
                    Section12g = x.Section12g,
                    Section12h = x.Section12h,
                    Section12i = x.Section12i,
                    Section12j = x.Section12j,
                    Section12k = x.Section12k,
                    Section13 = x.Section13,
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate,
                    SMRFormCrewMemberList = x.SMRFormCrewMembers.Select(y => new SMRFormCrewMemberModal()
                    {
                        Rank = y.Rank,
                        FullName = y.FullName,
                        ElectedAsSafety = y.ElectedAsSafety,
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return SMRModalList;
        }
        public SMRModal GetSMRFormsFilledByID(long ID)
        {
            SMRModal Modal = new SMRModal();
            SMRForm dbModal = new SMRForm();
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            try
            {
                dbModal = dbContext.SMRForms.Where(x => x.SMRFormID == ID).FirstOrDefault();
                if (dbModal != null)
                {
                    Modal.SMRFormID = dbModal.SMRFormID;
                    Modal.ShipID = dbModal.ShipID;
                    Modal.ShipName = dbModal.ShipName;
                    Modal.Year = dbModal.Year;
                    Modal.ReviewPeriod = dbModal.ReviewPeriod;
                    Modal.DateOfMeeting = dbModal.DateOfMeeting;
                    Modal.Section1 = dbModal.Section1;
                    Modal.Section2 = dbModal.Section2;
                    Modal.Section3 = dbModal.Section3;
                    Modal.Section4 = dbModal.Section4;
                    Modal.Section5 = dbModal.Section5;
                    Modal.Section6 = dbModal.Section6;
                    Modal.Section7 = dbModal.Section7;
                    Modal.Section7a = dbModal.Section7a;
                    Modal.Section7b = dbModal.Section7b;
                    Modal.Section7c = dbModal.Section7c;
                    Modal.Section7d = dbModal.Section7d;
                    Modal.Section7e1 = dbModal.Section7e1;
                    Modal.Section7e2 = dbModal.Section7e2;
                    Modal.Section7e3 = dbModal.Section7e3;
                    Modal.Section7f1 = dbModal.Section7f1;
                    Modal.Section7f2 = dbModal.Section7f2;
                    Modal.Section7g = dbModal.Section7g;
                    Modal.Section7h = dbModal.Section7h;
                    Modal.Section8a = dbModal.Section8a;
                    Modal.Section8b = dbModal.Section8b;
                    Modal.Section8b1 = dbModal.Section8b1;
                    Modal.Section8b2 = dbModal.Section8b2;
                    Modal.Section8b3 = dbModal.Section8b3;
                    Modal.Section8b4 = dbModal.Section8b4;
                    Modal.Section8b5 = dbModal.Section8b5;
                    Modal.Section9 = dbModal.Section9;
                    Modal.Section10 = dbModal.Section10;
                    Modal.Section11 = dbModal.Section11;
                    Modal.Section12a = dbModal.Section12a;
                    Modal.Section12b = dbModal.Section12b;
                    Modal.Section12c = dbModal.Section12c;
                    Modal.Section12d = dbModal.Section12d;
                    Modal.Section12e = dbModal.Section12e;
                    Modal.Section12f = dbModal.Section12f;
                    Modal.Section12g = dbModal.Section12g;
                    Modal.Section12h = dbModal.Section12h;
                    Modal.Section12i = dbModal.Section12i;
                    Modal.Section12j = dbModal.Section12j;
                    Modal.Section12k = dbModal.Section12k;
                    Modal.Section13 = dbModal.Section13;
                    Modal.CreatedDate = dbModal.CreatedDate;
                    Modal.UpdatedDate = dbModal.UpdatedDate;
                    Modal.SMRFormCrewMemberList = dbModal.SMRFormCrewMembers.Select(y => new SMRFormCrewMemberModal()
                    {
                        Rank = y.Rank,
                        FullName = y.FullName,
                        ElectedAsSafety = y.ElectedAsSafety,
                    }).ToList();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return Modal;
        }
    }
}
