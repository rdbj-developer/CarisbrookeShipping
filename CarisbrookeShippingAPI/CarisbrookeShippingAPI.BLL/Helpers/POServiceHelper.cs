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
    public class POServiceHelper
    {
        #region PO service
        public List<CSShipsPOModal> GetCSShipsPOData()
        {
            List<CSShipsPOModal> data = new List<CSShipsPOModal>();
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["CASHIP_WebGUIConnectionString"]);
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT [SITENAME],[PONO],[VENDOR_ADDR_NAME],[ACCOUNT_CODE],[ACCOUNT_DESCR],[PORECVDATE],[POTOTAL],"
                            + "[POEXCHRATE],[POTOTAL_BASE],[FORWARDER_RECVD_DATE],[INVOICENO] AS 'INVOICE_PRESENT',[DEPT_CODE],[CURR_CODE],[EQUIP_NAME],[PODATE],"
                            + "[POTITLE] FROM [CAShip_WebGUI].[dbo].[SMV_VIMS_PO_EPC] LEFT OUTER JOIN [CAShip_WebGUI].[dbo].[SM_REQN_INVOICE] "
                            + "ON SMV_VIMS_PO_EPC.VRID=SM_REQN_INVOICE.VRID WHERE SMV_VIMS_PO_EPC.POSTATUS<> 11  AND SMV_VIMS_PO_EPC.SITEID "
                            + "IN(86, 93, 92, 90, 100, 102,103,105, 89, 85, 75, 74, 72, 73, 76, 61, 63, 64, 62, 60, 65, 66, 68, 67, 59, 78, 70,"
                            + "91, 84, 82, 94, 69, 71, 1, 79, 88, 87, 95, 98, 97, 96, 83, 58, 52, 51, 53, 57, 54, 55, 56, 81, 106, 107, 108) "
                            + " AND INVOICENO IS NULL AND (PODATE >= '01/01/2019' AND PODATE <= GETDATE()) ORDER BY PODATE DESC ";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            data = dt.ToListof<CSShipsPOModal>();
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
            return data;

        }
        public List<CodafinPOModal> GetCodaFinPOData()
        {
            List<CodafinPOModal> data = new List<CodafinPOModal>();
            string connetionString = Convert.ToString(ConfigurationManager.AppSettings["CodaFinConnectionString"]);
            try
            {
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "SELECT [codafin].dbo.oas_docline.cmpcode, [codafin].dbo.oas_docline.doccode, "
                            + "[codafin].dbo.oas_docline.docnum, [codafin].dbo.oas_docline.moddate, [codafin].dbo.oas_docline.el1, "
                            + "[codafin].dbo.oas_docline.valuedoc, [codafin].dbo.oas_docline.descr "
                            + "FROM[codafin].[dbo].[oas_docline] where[moddate] > '2019-01-01' and "
                            + "[descr] COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%[A-Z][A-Z][A-Z]-[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]%'";
                        //+ "SELECT[codafin].dbo.oas_docline.cmpcode, [codafin].dbo.oas_docline.doccode, [codafin].dbo.oas_docline.docnum, [codafin].dbo.oas_docline.moddate, "
                        //+ "[codafin].dbo.oas_docline.el1,[codafin].dbo.oas_docline.valuedoc, [codafin].dbo.oas_docline.descr "
                        //+ "FROM[codafin].[dbo].[oas_docline] where[moddate] > '2019-01-01' and"
                        //+ "[descr] COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%[A-Z][A-Z][A-Z]-[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]%'";

                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            data = dt.ToListof<CodafinPOModal>();
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("CodaFin Connection is not available !!!");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return data;
        }

        public bool AddCodaPurchaseOrder(CodaPurchaseOrder modal)
        {
            bool res = false;
            try
            {
                if (modal == null)
                    return false;
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    modal.Id = Guid.NewGuid();
                    modal.CreatedDate = Utility.ToDateTimeUtcNow(); //DateTime.Now; //RDBJ 10/27/2021 set UtcTime
                    dbContext.CodaPurchaseOrders.Add(modal);
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
        public bool RemoveCodaPurchaseOrder(CodaPurchaseOrder modal)
        {
            bool res = false;
            try
            {
                if (modal == null)
                    return false;
                using (CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities())
                {
                    var dbValue = dbContext.CodaPurchaseOrders.Where(x => x.Descr == modal.Descr).ToList();
                    if (dbValue != null && dbValue.Count > 0)
                    {
                        foreach (var item in dbValue)
                        {
                            dbContext.CodaPurchaseOrders.Remove(item);
                        }
                        dbContext.SaveChanges();
                        res = true;
                    }
                    res = false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
            }
            return res;
        }

        public void DeleteCSShippsPOData()
        {
            try
            {
                CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();

                dbContext.Database.ExecuteSqlCommand("delete from CodaPurchaseOrders");
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.writelog("DeletCSShippsPOData :" + ex.Message);
            }
        }
        #endregion

        #region PurchasingDept Report
        public List<PurchasingDeptModel> GetAllPurchasingDeptDataReports(PurchasingDeptModel Modal)
        {
            CarisbrookeShippingEntities dbContext = new CarisbrookeShippingEntities();
            List<PurchasingDeptModel> listDatas = new List<PurchasingDeptModel>();
            try
            {
                List<PurchasingDeptModel> data = new List<PurchasingDeptModel>();
                int POyear = Convert.ToInt32(Modal.POYear);
                int POmonth = Convert.ToInt32(Modal.POMonth);
                string connetionString = Convert.ToString(ConfigurationManager.AppSettings["ShipConnectionString"]);
                using (var conn = new SqlConnection(connetionString))
                {
                    if (conn.IsAvailable())
                    {
                        string selectQuery = "select cp.*,FleetId from CodaPurchaseOrders cp "
                        + "left Join CSShips cs on cs.Name = cp.SiteName ";
                        bool isAddWhre = true;
                        if (POyear != 0)
                        {
                            if (isAddWhre)
                            {
                                selectQuery += " Where ";
                                isAddWhre = false;
                            }
                            selectQuery += " year(cp.PODate)= " + POyear;
                        }
                        if (POmonth != 0)
                        {
                            if (isAddWhre)
                            {
                                selectQuery += " Where ";
                                isAddWhre = false;
                            }
                            else
                            {
                                selectQuery += " AND ";
                            }
                            selectQuery += " month(cp.PODate)= " + POmonth;
                        }
                        if (Modal.FleetId != null)
                        {
                            if (isAddWhre)
                            {
                                selectQuery += " Where ";
                                isAddWhre = false;
                            }
                            else
                            {
                                selectQuery += " AND ";
                            }
                            selectQuery += " cs.FleetId= " + Modal.FleetId;
                        }
                        LogHelper.writelog("Query : " + selectQuery);
                        //+ " Where year(cp.PODate)= 2020 AND month(cp.PODate)= 2 AND cs.FleetId = 1";
                        conn.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter sqlAdp = new SqlDataAdapter(selectQuery, conn);
                        sqlAdp.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                            data = dt.ToListof<PurchasingDeptModel>();
                        conn.Close();
                    }
                    else
                    {
                        LogHelper.writelog("AWS Connection is not available !!!");
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("GetAllPurchasingDeptDataReports :" + ex.Message);
                return listDatas;
            }
        }
        #endregion
    }
}
