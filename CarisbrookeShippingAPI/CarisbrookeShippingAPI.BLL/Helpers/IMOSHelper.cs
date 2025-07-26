using CarisbrookeShippingAPI.BLL.Modals;
using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CarisbrookeShippingAPI.BLL.Helpers
{
    public class IMOSHelper
    {
        public List<CSShipsModal> GetAllShips()
        {
            //RDBJ 10/12/2021
            List<CSShipsModal> ShipModelList = new List<CSShipsModal>();
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var dbships = dbContext.CSShips.ToList();

                ShipModelList = dbships.Select(x => new CSShipsModal()
                {
                    ShipId = x.ShipId,
                    Name = x.Name,
                    Code = x.Code,
                }).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return ShipModelList;
            //End RDBJ 10/12/2021

            //RDBJ 10/12/2021 Commented
            /*
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["IMOSConnectionString"]);
           
            List<CSShipsModal> ShipModelList = new List<CSShipsModal>();
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT * FROM CSShips";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            ShipModelList = dt.ToListof<CSShipsModal>();
                        }
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("CSShip Connection is not available !!!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return ShipModelList;
            */
        }

        public BLL.Modals.CSShipsModal GetGeneralSettingDataByCode(string code) //RDBJ 10/07/2021 change CSShipsModal from GeneralInspectionReport
        {

            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["IMOSConnectionString"]);

            CSShipsModal ShipModelList = new CSShipsModal();
            BLL.Modals.GeneralInspectionReport generalInspectionReport = new Modals.GeneralInspectionReport();
            try
            {
                //RDBJ 10/12/2021
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
                var dbships = dbContext.CSShips.Where(x => x.Code == code).FirstOrDefault();

                ShipModelList.ShipId = dbships.ShipId;
                ShipModelList.Name = dbships.Name;
                ShipModelList.Code = dbships.Code;
                ShipModelList.ClassificationSocietyId = dbships.ClassificationSocietyId;
                ShipModelList.BuildYear = dbships.BuildYear;
                ShipModelList.IMONumber = dbships.IMONumber;
                ShipModelList.CallSign = dbships.CallSign;
                ShipModelList.MMSI = dbships.MMSI;
                ShipModelList.GrossTonnage = dbships.GrossTonnage;
                ShipModelList.NetTonnage = dbships.NetTonnage;
                ShipModelList.Email = dbships.Email;
                ShipModelList.Phone1 = dbships.Phone1;
                ShipModelList.Phone2 = dbships.Phone2;
                ShipModelList.Phone3 = dbships.Phone3;
                ShipModelList.Mobile = dbships.Mobile;
                ShipModelList.FleetId = dbships.FleetId;
                ShipModelList.PortOfRegistryId = dbships.PortOfRegistryId;
                ShipModelList.SummerDeadweight = dbships.SummerDeadweight;
                ShipModelList.SummerDraft = dbships.SummerDraft;
                ShipModelList.Lightweight = dbships.Lightweight;
                ShipModelList.Beam = dbships.Beam;
                ShipModelList.LOA = dbships.LOA;
                ShipModelList.LBP = dbships.LBP;
                ShipModelList.BowThruster = dbships.BowThruster;
                ShipModelList.BHP = dbships.BHP;
                ShipModelList.FlagStateId = dbships.FlagStateId;

                //End RDBJ 10/12/2021

                //RDBJ 10/12/2021 Commented
                /*
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT * FROM CSShips where Code='"+code+"'";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            ShipModelList = dt.ToListof<CSShipsModal>().FirstOrDefault();
                        }
                        //generalInspectionReport.MMSI = Convert.ToString(ShipModelList.MMSI);
                        generalInspectionReport.MMSI = Convert.ToString(ShipModelList.MMSI);
                        generalInspectionReport.YearofBuild = Convert.ToString(ShipModelList.BuildYear);
                        generalInspectionReport.IMOnumber = Convert.ToString(ShipModelList.IMONumber);
                        generalInspectionReport.Callsign = Convert.ToString(ShipModelList.CallSign);
                        generalInspectionReport.SummerDWT = Convert.ToString(ShipModelList.SummerDeadweight);
                        generalInspectionReport.Grosstonnage = Convert.ToString(ShipModelList.GrossTonnage);
                        generalInspectionReport.Lightweight = Convert.ToString(ShipModelList.Lightweight);
                        generalInspectionReport.Nettonnage = Convert.ToString(ShipModelList.NetTonnage);
                        generalInspectionReport.Beam = Convert.ToString(ShipModelList.Beam);
                        generalInspectionReport.LOA = Convert.ToString(ShipModelList.LOA);
                        generalInspectionReport.Summerdraft = Convert.ToString(ShipModelList.SummerDraft);
                        generalInspectionReport.LBP = Convert.ToString(ShipModelList.LBP);
                        generalInspectionReport.BHP = Convert.ToString(ShipModelList.BHP);
                        generalInspectionReport.Classsociety = Convert.ToString(ShipModelList.ClassificationSocietyId);
                        generalInspectionReport.Flag = Convert.ToString(ShipModelList.FlagStateId);
                        generalInspectionReport.Portofregistry = Convert.ToString(ShipModelList.PortOfRegistryId);
                        generalInspectionReport.Bowthruster = Convert.ToString(ShipModelList.BowThruster);
                        //generalInspectionReport.Bowthruster = Convert.ToString(ShipModelList.c);
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("CSShip Connection is not available !!!");
                    }
                }
                */
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return ShipModelList;
        }
    }
}
