using AWS_DB_UpdateService.Modals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AWS_DB_UpdateService.Helpers
{
    public class XMLHelper
    {
        public static bool Create_XML_ImportPeople(dsisy01 dbUser, out string filename)
        {
            filename = string.Empty;
            try
            {
                filename = Utility.ToSTRING(dbUser.empnry01.Trim()) + ".xml";
                XmlDocument xmlEmloyeeDoc = new XmlDocument();
                string xmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + filename;
                Directory.CreateDirectory(Path.GetDirectoryName(xmlfilePath));

                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(docNode);

                XmlNode ESDI_tblUserNode = doc.CreateElement("ESDI_tblUser");
                doc.AppendChild(ESDI_tblUserNode);

                XmlNode tblUserNode = doc.CreateElement("tblUser");

                XmlNode vUserID = doc.CreateElement("vUserID");
                vUserID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
                tblUserNode.AppendChild(vUserID);

                XmlNode vFirstname = doc.CreateElement("vFirstname");
                vFirstname.InnerText = dbUser.givenname;
                tblUserNode.AppendChild(vFirstname);

                XmlNode vLastname = doc.CreateElement("vLastname");
                vLastname.InnerText = dbUser.famname;
                tblUserNode.AppendChild(vLastname);

                XmlNode dtBirthdate = doc.CreateElement("dtBirthdate");
                dtBirthdate.InnerText = dbUser.bthdate01.HasValue ? dbUser.bthdate01.Value.ToString("yyyy-MM-dd") : "";
                tblUserNode.AppendChild(dtBirthdate);

                XmlNode uRankID = doc.CreateElement("uRankID");
                uRankID.InnerText = dbUser.uRankID;
                tblUserNode.AppendChild(uRankID);

                XmlNode vSex = doc.CreateElement("vSex");
                vSex.InnerText = dbUser.sxee01;
                tblUserNode.AppendChild(vSex);

                XmlNode uCountryID = doc.CreateElement("uCountryID");
                uCountryID.InnerText = dbUser.uCountryID;
                tblUserNode.AppendChild(uCountryID);

                XmlNode vEmail = doc.CreateElement("vEmail");
                if (!string.IsNullOrWhiteSpace(dbUser.line1a04))
                    vEmail.InnerText = dbUser.line1a04.Trim();
                tblUserNode.AppendChild(vEmail);

                //XmlNode vPlaceOfBirth = doc.CreateElement("vPlaceOfBirth");
                //vPlaceOfBirth.InnerText = "Aden";
                //tblUserNode.AppendChild(vPlaceOfBirth);

                XmlNode vDocumentNo = doc.CreateElement("vDocumentNo");
                vDocumentNo.InnerText = "4221234";
                tblUserNode.AppendChild(vDocumentNo);

                XmlNode bActive = doc.CreateElement("bActive");
                bActive.InnerText = "True";
                tblUserNode.AppendChild(bActive);

                XmlNode vExternalID = doc.CreateElement("vExternalID");
                vExternalID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim()) + DateTime.Now.ToString("ddMMyyyyHHmmss");
                tblUserNode.AppendChild(vExternalID);

                ESDI_tblUserNode.AppendChild(tblUserNode);
                doc.Save(xmlfilePath);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return false;
            }
        }
        public static bool Create_XML_ImportPeopleBulk(List<dsisy01> userList, out string filename)
        {
            filename = string.Empty;
            try
            {
                filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_Update" + ".xml";
                XmlDocument xmlEmloyeeDoc = new XmlDocument();
                string xmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + filename;
                Directory.CreateDirectory(Path.GetDirectoryName(xmlfilePath));

                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(docNode);

                XmlNode ESDI_tblUserNode = doc.CreateElement("ESDI_tblUser");
                doc.AppendChild(ESDI_tblUserNode);
                XmlNode tblUserNode = null;
                XmlNode vNode = null;

                foreach (var dbUser in userList)
                {
                    tblUserNode = doc.CreateElement("tblUser");
                    vNode = doc.CreateElement("vUserID");
                    vNode.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vFirstname");
                    vNode.InnerText = dbUser.givenname;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vLastname");
                    vNode.InnerText = dbUser.famname;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("dtBirthdate");
                    vNode.InnerText = dbUser.bthdate01.HasValue ? dbUser.bthdate01.Value.ToString("yyyy-MM-dd") : "";
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("uRankID");
                    vNode.InnerText = dbUser.uRankID;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vSex");
                    vNode.InnerText = dbUser.sxee01;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("uCountryID");
                    vNode.InnerText = dbUser.uCountryID;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vEmail");
                    if (!string.IsNullOrWhiteSpace(dbUser.line1a04))
                        vNode.InnerText = dbUser.line1a04.Trim();
                    tblUserNode.AppendChild(vNode);

                    //XmlNode vPlaceOfBirth = doc.CreateElement("vPlaceOfBirth");
                    //vPlaceOfBirth.InnerText = "Aden";
                    //tblUserNode.AppendChild(vPlaceOfBirth);

                    vNode = doc.CreateElement("vDocumentNo");
                    vNode.InnerText = "4221234";
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("bActive");
                    vNode.InnerText = "True";
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vExternalID");
                    vNode.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim()) + DateTime.Now.ToString("ddMMyyyyHHmmss");
                    tblUserNode.AppendChild(vNode);

                    ESDI_tblUserNode.AppendChild(tblUserNode);
                }
                doc.Save(xmlfilePath);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return false;
            }
        }
        public static bool Create_XML_ServiceRecordRequest(dsisy01 dbUser, out string filename)
        {
            filename = string.Empty;
            try
            {
                filename = Utility.ToSTRING(dbUser.empnry01.Trim()) + "_Update" + ".xml";
                XmlDocument xmlEmloyeeDoc = new XmlDocument();
                string xmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + filename;
                Directory.CreateDirectory(Path.GetDirectoryName(xmlfilePath));

                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(docNode);

                XmlNode ESDI_tblUserNode = doc.CreateElement("ESDI_tblUserServiceRecord");
                doc.AppendChild(ESDI_tblUserNode);

                XmlNode tblUserNode = doc.CreateElement("tblUserServiceRecord");

                XmlNode vExternalID = doc.CreateElement("vExternalID");
                vExternalID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim()) + DateTime.Now.ToString("ddMMyyyyHHmmss");
                tblUserNode.AppendChild(vExternalID);

                XmlNode vUserID = doc.CreateElement("vUserID");
                vUserID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
                tblUserNode.AppendChild(vUserID);

                XmlNode iInstallationID = doc.CreateElement("iInstallationID");
                if (!(string.IsNullOrWhiteSpace(dbUser.iInstallationCode)))
                    iInstallationID.InnerText = dbUser.iInstallationCode;
                else
                    iInstallationID.InnerText = "";
                tblUserNode.AppendChild(iInstallationID);

                XmlNode dtSignOn = doc.CreateElement("dtSignOn");
                dtSignOn.InnerText = dbUser.crew_signon.HasValue ? dbUser.crew_signon.Value.ToString("yyyy-MM-dd") : ""; //"2019-01-01";
                tblUserNode.AppendChild(dtSignOn);

                XmlNode dtSignOff = doc.CreateElement("dtSignOff");
                dtSignOff.InnerText = dbUser.crew_signoff.HasValue ? dbUser.crew_signoff.Value.ToString("yyyy-MM-dd") : ""; //"2019-03-15";
                tblUserNode.AppendChild(dtSignOff);

                XmlNode uRankID = doc.CreateElement("uRankID");
                uRankID.InnerText = dbUser.uRankID;
                tblUserNode.AppendChild(uRankID);

                //XmlNode uVesselTypeID = doc.CreateElement("uVesselTypeID");
                //uVesselTypeID.InnerText = dbUser.uVesselTypeID;
                //tblUserNode.AppendChild(uVesselTypeID);

                ESDI_tblUserNode.AppendChild(tblUserNode);
                doc.Save(xmlfilePath);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return false;
            }
        }

        public static bool Create_XML_ServiceRecordRequestBulk(List<dsisy01> userList, out string filename)
        {
            filename = string.Empty;
            try
            {
                filename = DateTime.Now.ToString("yyyyMMddHHmmss") + "_Update" + ".xml";
                XmlDocument xmlEmloyeeDoc = new XmlDocument();
                string xmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + filename;
                Directory.CreateDirectory(Path.GetDirectoryName(xmlfilePath));

                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(docNode);

                XmlNode ESDI_tblUserNode = doc.CreateElement("ESDI_tblUserServiceRecord");
                doc.AppendChild(ESDI_tblUserNode);

                XmlNode tblUserNode = null;
                XmlNode vNode = null;
                foreach (var dbUser in userList)
                {
                    tblUserNode = doc.CreateElement("tblUserServiceRecord");
                    vNode = doc.CreateElement("vExternalID");
                    vNode.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim()) + DateTime.Now.ToString("ddMMyyyyHHmmss");
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vUserID");
                    vNode.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("iInstallationID");
                    if (!(string.IsNullOrWhiteSpace(dbUser.iInstallationCode)))
                        vNode.InnerText = dbUser.iInstallationCode;
                    else
                        vNode.InnerText = "";
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("dtSignOn");
                    vNode.InnerText = dbUser.crew_signon.HasValue ? dbUser.crew_signon.Value.ToString("yyyy-MM-dd") : ""; //"2019-01-01";
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("dtSignOff");
                    vNode.InnerText = dbUser.crew_signoff.HasValue ? dbUser.crew_signoff.Value.ToString("yyyy-MM-dd") : ""; //"2019-03-15";
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("uRankID");
                    vNode.InnerText = dbUser.uRankID;
                    tblUserNode.AppendChild(vNode);

                    ESDI_tblUserNode.AppendChild(tblUserNode);
                }
                doc.Save(xmlfilePath);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return false;
            }
        }

        public static bool Create_XML_AddOrUpdatePerson(dsisy01 dbUser, out string filename)
        {
            filename = string.Empty;
            try
            {
                filename = Utility.ToSTRING(dbUser.empnry01.Trim()) + ".xml";
                XmlDocument xmlEmloyeeDoc = new XmlDocument();
                string xmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + filename;
                Directory.CreateDirectory(Path.GetDirectoryName(xmlfilePath));

                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(docNode);

                XmlNode ESDI_tblUserNode = doc.CreateElement("ESDI_tblUser");
                doc.AppendChild(ESDI_tblUserNode);

                XmlNode tblUserNode = doc.CreateElement("tblUser");

                XmlNode vUserID = doc.CreateElement("vUserID");
                vUserID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
                tblUserNode.AppendChild(vUserID);

                XmlNode vFirstname = doc.CreateElement("vFirstname");
                vFirstname.InnerText = dbUser.givenname;
                tblUserNode.AppendChild(vFirstname);

                XmlNode vLastname = doc.CreateElement("vLastname");
                vLastname.InnerText = dbUser.famname;
                tblUserNode.AppendChild(vLastname);

                XmlNode dtBirthdate = doc.CreateElement("dtBirthdate");
                dtBirthdate.InnerText = dbUser.bthdate01.HasValue ? dbUser.bthdate01.Value.ToString("yyyy-MM-dd") : "";
                tblUserNode.AppendChild(dtBirthdate);

                XmlNode uRankID = doc.CreateElement("uRankID");
                uRankID.InnerText = dbUser.uRankID;
                tblUserNode.AppendChild(uRankID);

                //XmlNode uVesselTypeID = doc.CreateElement("uVesselTypeID");
                //uVesselTypeID.InnerText = dbUser.uVesselTypeID;
                //tblUserNode.AppendChild(uVesselTypeID);

                XmlNode vSex = doc.CreateElement("vSex");
                vSex.InnerText = dbUser.sxee01;
                tblUserNode.AppendChild(vSex);

                XmlNode uCountryID = doc.CreateElement("uCountryID");
                uCountryID.InnerText = dbUser.uCountryID;
                tblUserNode.AppendChild(uCountryID);

                XmlNode vEmail = doc.CreateElement("vEmail");
                if (!string.IsNullOrWhiteSpace(dbUser.line1a04))
                    vEmail.InnerText = dbUser.line1a04.Trim();
                tblUserNode.AppendChild(vEmail);

                XmlNode vPlaceOfBirth = doc.CreateElement("vPlaceOfBirth");
                vPlaceOfBirth.InnerText = dbUser.bthctye01;
                tblUserNode.AppendChild(vPlaceOfBirth);

                //XmlNode vDocumentNo = doc.CreateElement("vDocumentNo");
                //vDocumentNo.InnerText = "4221234";
                //tblUserNode.AppendChild(vDocumentNo);

                //XmlNode iManagedByID = doc.CreateElement("iManagedByID");
                //iManagedByID.InnerText = dbUser.iManagedByID;
                //tblUserNode.AppendChild(uVesselTypeID);

                //XmlNode uPersonnelPoolID = doc.CreateElement("uPersonnelPoolID");
                //uPersonnelPoolID.InnerText = dbUser.uPersonnelPoolID;
                //tblUserNode.AppendChild(uPersonnelPoolID);

                //XmlNode iLastVesselID = doc.CreateElement("iLastVesselID");
                //iLastVesselID.InnerText = dbUser.iLastVesselID;
                //tblUserNode.AppendChild(iLastVesselID);

                XmlNode bActive = doc.CreateElement("bActive");
                bActive.InnerText = "True";
                tblUserNode.AppendChild(bActive);

                XmlNode vExternalID = doc.CreateElement("vExternalID");
                vExternalID.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim()) + DateTime.Now.ToString("ddMMyyyyHHmmss");
                tblUserNode.AppendChild(vExternalID);

                ESDI_tblUserNode.AppendChild(tblUserNode);
                doc.Save(xmlfilePath);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return false;
            }
        }

        public static bool Create_XML_AddOrUpdatePersonBulk(List<dsisy01> userList, out string filename)
        {
            filename = string.Empty;
            try
            {
                filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                XmlDocument xmlEmloyeeDoc = new XmlDocument();
                string xmlfilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFiles\\" + filename;
                Directory.CreateDirectory(Path.GetDirectoryName(xmlfilePath));

                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                doc.AppendChild(docNode);

                XmlNode ESDI_tblUserNode = doc.CreateElement("ESDI_tblUser");
                doc.AppendChild(ESDI_tblUserNode);
                XmlNode tblUserNode = null;
                XmlNode vNode = null;
                foreach (var dbUser in userList)
                {
                    tblUserNode = null;
                    vNode = null;
                    tblUserNode = doc.CreateElement("tblUser");
                    vNode = doc.CreateElement("vUserID");
                    vNode.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim());
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vFirstname");
                    vNode.InnerText = dbUser.givenname;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vLastname");
                    vNode.InnerText = dbUser.famname;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("dtBirthdate");
                    vNode.InnerText = dbUser.bthdate01.HasValue ? dbUser.bthdate01.Value.ToString("yyyy-MM-dd") : "";
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("uRankID");
                    vNode.InnerText = dbUser.uRankID;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vSex");
                    vNode.InnerText = dbUser.sxee01;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("uCountryID");
                    vNode.InnerText = dbUser.uCountryID;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vEmail");
                    if (!string.IsNullOrWhiteSpace(dbUser.line1a04))
                        vNode.InnerText = dbUser.line1a04.Trim();
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vPlaceOfBirth");
                    vNode.InnerText = dbUser.bthctye01;
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("bActive");
                    vNode.InnerText = "True";
                    tblUserNode.AppendChild(vNode);

                    vNode = doc.CreateElement("vExternalID");
                    vNode.InnerText = Utility.ToSTRING(dbUser.empnry01.Trim()) + DateTime.Now.ToString("ddMMyyyyHHmmss");
                    tblUserNode.AppendChild(vNode);

                    ESDI_tblUserNode.AppendChild(tblUserNode);
                }
                doc.Save(xmlfilePath);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog(ex.Message);
                return false;
            }
        }
    }
}
