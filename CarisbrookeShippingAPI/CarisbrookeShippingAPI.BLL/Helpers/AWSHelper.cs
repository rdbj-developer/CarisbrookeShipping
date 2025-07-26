using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class AWSHelper
    {
        public List<UserModal> GetAWSUsers()
        {
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["AWSConnectionString"]);
            List<UserModal> dbUsersList = new List<UserModal>();
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT [empnre01],[empste01],[fstnme01],[surnme01],[bthcnte01],[fnce01],[lane01],[rsnewe01],[sxee01],[bthdate01],[nate01] FROM dsise01 where empste01 = 'p'";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dbUsersList = dt.ToListof<UserModal>();
                        }
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("AWS Connection is not available !!!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return dbUsersList;
        }
        public bool UpdateLocalDbForUsers()
        {
            bool res = false;
            try
            {
                List<UserModal> dbUsersList = GetAWSUsers();
                if (dbUsersList != null && dbUsersList.Count > 0)
                {
                    CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                    foreach (var item in dbUsersList)
                    {
                        var dbExistUser = dbContext.Users.Where(x => x.empnre01 == item.empnre01).FirstOrDefault();
                        if (dbExistUser == null)
                        {
                            User dbUser = new User();
                            dbUser.empnre01 = item.empnre01;
                            dbUser.empste01 = item.empste01;
                            dbUser.fstnme01 = item.fstnme01;
                            dbUser.surnme01 = item.surnme01;
                            dbUser.bthcnte01 = item.bthcnte01;
                            dbUser.fnce01 = item.fnce01;
                            dbUser.lane01 = item.lane01;
                            dbUser.rsnewe01 = item.rsnewe01;
                            dbUser.sxee01 = item.sxee01;
                            dbUser.bthdate01 = item.bthdate01;
                            dbUser.nate01 = item.nate01;
                            dbContext.Users.Add(dbUser);
                        }
                        else
                        {
                            dbExistUser.empste01 = item.empste01;
                            dbExistUser.fstnme01 = item.fstnme01;
                            dbExistUser.surnme01 = item.surnme01;
                            dbExistUser.bthcnte01 = item.bthcnte01;
                            dbExistUser.fnce01 = item.fnce01;
                            dbExistUser.lane01 = item.lane01;
                            dbExistUser.rsnewe01 = item.rsnewe01;
                            dbExistUser.sxee01 = item.sxee01;
                            dbExistUser.bthdate01 = item.bthdate01;
                            dbExistUser.nate01 = item.nate01;
                            dbContext.SaveChanges();
                        }
                    }
                    dbContext.SaveChanges();
                    res = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }
        public List<UserModal> GetAWSUsersFromOfficeDBForSync()
        {
            List<UserModal> UsersList = new List<UserModal>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                List<User> dbUsers = dbContext.Users.ToList();
                UsersList = dbUsers.Select(x => new UserModal()
                {
                    UID = x.UID,
                    empnre01 = x.empnre01,
                    empste01 = x.empste01,
                    fstnme01 = x.fstnme01,
                    surnme01 = x.surnme01,
                    bthcnte01 = x.bthcnte01,
                    fnce01 = x.fnce01,
                    lane01 = x.lane01,
                    rsnewe01 = x.rsnewe01,
                    sxee01 = x.sxee01,
                    bthdate01 = x.bthdate01,
                    nate01 = x.nate01,
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return UsersList;
        }

        //RDBJ 09/16/2021
        public List<CSShipsModalAWS> GetAWSCSShipsFromOfficeDBForSync()
        {
            //RDBJ 10/17/2021 Updated source to get from CarisbrookeShipping rather than it was from AWSConnectionString
            List<CSShipsModalAWS> dbCSShipsList = new List<CSShipsModalAWS>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                //var csShips = dbContext.CSShips.ToList(); // RDBJ 03/26/2022 Commented this line
                var csShips = dbContext.CSShips.Where(x => !x.Name.ToUpper().StartsWith("XX") && x.Name.ToUpper() != "ALL").ToList(); // RDBJ 03/26/2022
                if (csShips != null)
                {
                    foreach (var dbships in csShips)
                    {
                        CSShipsModalAWS shipModal = new CSShipsModalAWS();
                        //RDBJ 10/23/2021 Added Full tables fileds
                        shipModal.ShipId = dbships.ShipId;
	                    shipModal.Name = dbships.Name;
                        shipModal.Code = dbships.Code; //RDBJ 10/25/2021
                        shipModal.ShipClassId = Convert.ToInt32(dbships.ShipClassId);
                        shipModal.BuildCountryId = Convert.ToInt32(dbships.BuildCountryId);
                        shipModal.BuildYear = Convert.ToInt32(dbships.BuildYear);
                        shipModal.ClassificationSocietyId = Convert.ToInt32(dbships.ClassificationSocietyId);
                        shipModal.FlagStateId = Convert.ToInt32(dbships.FlagStateId);
                        shipModal.IMONumber = Convert.ToInt32(dbships.IMONumber);
                        shipModal.CallSign = dbships.CallSign;
                        shipModal.MMSI = Convert.ToInt32(dbships.MMSI);
                        shipModal.GrossTonnage = Convert.ToInt32(dbships.GrossTonnage);
	                    shipModal.NetTonnage = Convert.ToInt32(dbships.NetTonnage);
	                    shipModal.OfficeId = Convert.ToInt32(dbships.OfficeId);
	                    shipModal.TechnicalManagerId = Convert.ToInt32(dbships.TechnicalManagerId);
	                    shipModal.SuperintendentId = Convert.ToInt32(dbships.SuperintendentId);
	                    shipModal.Notes = dbships.Notes;
	                    shipModal.IsDelivered = dbships.IsDelivered;
	                    shipModal.FleetId = Convert.ToInt32(dbships.FleetId);
	                    shipModal.YardNo = Convert.ToInt32(dbships.YardNo);
	                    shipModal.OfficialNumber = Convert.ToInt32(dbships.OfficialNumber);
	                    shipModal.PortOfRegistryId = Convert.ToInt32(dbships.PortOfRegistryId);
	                    shipModal.SummerDeadweight = dbships.SummerDeadweight;
	                    shipModal.Lightweight = dbships.Lightweight;
	                    shipModal.LOA = dbships.LOA;
	                    shipModal.LBP = dbships.LBP;
	                    shipModal.Beam = dbships.Beam;
	                    shipModal.SummerDraft = dbships.SummerDraft;
	                    shipModal.BHP = Convert.ToInt32(dbships.BHP);
	                    shipModal.BowThruster = Convert.ToInt32(dbships.BowThruster);
	                    shipModal.BuildNumber = Convert.ToInt32(dbships.BuildNumber);
	                    shipModal.Agent = dbships.Agent;
	                    shipModal.Ports = dbships.Ports;
	                    shipModal.TechnicalManagerNotes = dbships.TechnicalManagerNotes;
	                    shipModal.MinimumSafeManning = Convert.ToInt32(dbships.MinimumSafeManning);
	                    shipModal.MaximumPersonsLSA = Convert.ToInt32(dbships.MaximumPersonsLSA);
	                    shipModal.TotalBerths = Convert.ToInt32(dbships.TotalBerths);
	                    shipModal.RegisteredOwners = dbships.RegisteredOwners;
	                    shipModal.HullAndMachineryId = Convert.ToInt32(dbships.HullAndMachineryId);
	                    shipModal.ProtectionAndIndemnityId = Convert.ToInt32(dbships.ProtectionAndIndemnityId);
	                    shipModal.Owner = dbships.Owner;
	                    shipModal.SMCIsVerified = dbships.SMCIsVerified;
	                    shipModal.MarineSoftwareNumber = dbships.MarineSoftwareNumber;
	                    shipModal.Email = dbships.Email;
	                    shipModal.DataloyNumber = dbships.DataloyNumber;
	                    shipModal.Phone1 = dbships.Phone1;
	                    shipModal.Phone2 = dbships.Phone2;
	                    shipModal.Phone3 = dbships.Phone3;
	                    shipModal.Mobile = dbships.Mobile;
	                    shipModal.SatC1 = dbships.SatC1;
	                    shipModal.SatC2 = dbships.SatC2;
	                    shipModal.Citadel = dbships.Citadel;
                        shipModal.PandIClub = dbships.P_I_Club;
                        //End RDBJ 10/23/2021 Added Full tables fileds

                        dbCSShipsList.Add(shipModal);

                        //RDBJ 10/23/2021
                        /* Commented
                        shipModal.ShipId = dbships.ShipId;
                        shipModal.Name = dbships.Name;
                        shipModal.Code = dbships.Code;
                        shipModal.ClassificationSocietyId = Convert.ToInt32(dbships.ClassificationSocietyId);
                        shipModal.BuildYear = Convert.ToInt32(dbships.BuildYear);
                        shipModal.IMONumber = Convert.ToInt32(dbships.IMONumber);
                        shipModal.CallSign = dbships.CallSign;
                        shipModal.MMSI = Convert.ToInt32(dbships.MMSI);
                        shipModal.GrossTonnage = Convert.ToInt32(dbships.GrossTonnage);
                        shipModal.NetTonnage = Convert.ToInt32(dbships.NetTonnage);
                        shipModal.Email = dbships.Email;
                        shipModal.Phone1 = dbships.Phone1;
                        shipModal.Phone2 = dbships.Phone2;
                        shipModal.Phone3 = dbships.Phone3;
                        shipModal.Mobile = dbships.Mobile;
                        shipModal.FleetId = Convert.ToInt32(dbships.FleetId);
                        shipModal.PortOfRegistryId = Convert.ToInt32(dbships.PortOfRegistryId);
                        shipModal.SummerDeadweight = dbships.SummerDeadweight;
                        shipModal.SummerDraft = dbships.SummerDraft;
                        shipModal.Lightweight = dbships.Lightweight;
                        shipModal.Beam = dbships.Beam;
                        shipModal.LOA = dbships.LOA;
                        shipModal.LBP = dbships.LBP;
                        shipModal.BowThruster = Convert.ToInt32(dbships.BowThruster);
                        shipModal.BHP = Convert.ToInt32(dbships.BHP);
                        shipModal.FlagStateId = Convert.ToInt32(dbships.FlagStateId);
                        */
                        //RDBJ 10/23/2021
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAWSCSShipsFromOfficeDBForSync : " + ex.Message);
            }
            //RDBJ 10/17/2021 Commented
            /*
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["AWSConnectionString"]);
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        //string selectQuery = "SELECT [Name],[Code],[ShipClassId],[BuildCountryId],[BuildYear],[ClassificationSocietyId],[FlagStateId],[IMONumber]" +
                        //    ",[CallSign],[MMSI],[GrossTonnage],[NetTonnage],[OfficeId],[TechnicalManagerId],[SuperintendentId],[Notes],[IsDelivered],[FleetId],[YardNo],[OfficialNumber]" +
                        //    ",[PortOfRegistryId],[SummerDeadweight],[Lightweight],[LOA],[LBP],[Beam],[SummerDraft],[BHP],[BowThruster],[BuildNumber],[Agent],[Ports],[TechnicalManagerNotes]" +
                        //    ",[MinimumSafeManning],[MaximumPersonsLSA],[TotalBerths],[RegisteredOwners],[HullAndMachineryId],[ProtectionAndIndemnityId],[Owner],[SMCIsVerified] FROM CSShips";
                        string selectQuery = "SELECT * FROM CSShips";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dbCSShipsList = dt.ToListof<CSShipsModalAWS>();
                        }
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("AWS Connection is not available !!!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            */
            return dbCSShipsList;
        }
        //End RDBJ 09/16/2021

        //RDBJ 11/08/2021
        public List<string> GetEmailFromUserProfileTableWhereTechnicalAndISMGroup(
            string ShipCode // JSL 02/24/2023
            )
        {
            List<string> userProfileModals = new List<string>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                // JSL 02/25/2023
                var ShipFleetID = dbContext.CSShips.Where(x => x.Code == ShipCode).Select(y => y.FleetId).FirstOrDefault();
                bool blnIsShipFleetIdIs124 = (ShipFleetID == 1 || ShipFleetID == 2 || ShipFleetID == 4);
                // End JSL 02/25/2023

                //where UserGroup == 5 is ISM Users and UserGroup == 7 is Technical Group
                var userEmails = dbContext.UserProfiles.Where(x=> 
                (x.UserGroup == 5 || x.UserGroup == 7)
                && x.ShipFleetID == 5   // JSL 02/25/2023
                ).ToList();

                if ((userEmails != null && userEmails.Count > 0)
                    && blnIsShipFleetIdIs124    // JSL 02/25/2023
                    )
                {
                    foreach (var user in userEmails)
                    {
                        // JSL 02/24/2023 wrapped in if
                        if (user.Email != "daniel.lewandowski@carisbrooke.co")
                        {
                            userProfileModals.Add(user.Email);
                        }
                        // End JSL 02/24/2023 wrapped in if
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAWSCSShipsFromOfficeDBForSync : " + ex.Message);
            }
            return userProfileModals;
        }
        //End RDBJ 11/08/2021
    }
}
