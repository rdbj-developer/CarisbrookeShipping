using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ShipApplication.BLL.Modals;
using ShipApplication.BLL.Helpers;

namespace ShipApplication.Helpers
{
    public class LocalDataHelper
    {
        public bool SaveDataInLocal(SMRModal Modal, bool synced)
        {
            bool res = false;
            try
            {
                string jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + "JsonFiles\\SMRFormData.json";
                Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath));
                if (!System.IO.File.Exists(jsonFilePath))
                {
                    System.IO.File.WriteAllText(jsonFilePath, string.Empty);
                }
                string jsonText = System.IO.File.ReadAllText(jsonFilePath);
                List<OfflineSMRFormModal> jsonDataList = new List<OfflineSMRFormModal>();
                if (!string.IsNullOrEmpty(jsonText))
                {

                    jsonDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OfflineSMRFormModal>>(jsonText).ToList();
                    OfflineSMRFormModal newModal = new OfflineSMRFormModal();
                    newModal.SMRFormID = jsonDataList.Count + 1;
                    newModal.Synced = synced;
                    newModal.SMRForm = Modal;
                    jsonDataList.Add(newModal);
                }
                else
                {
                    OfflineSMRFormModal newModal = new OfflineSMRFormModal();
                    newModal.SMRFormID = 1;
                    newModal.Synced = synced;
                    newModal.SMRForm = Modal;
                    jsonDataList.Add(newModal);
                }
                string jsonData = JsonConvert.SerializeObject(jsonDataList, Formatting.Indented);
                System.IO.File.WriteAllText(jsonFilePath, jsonData);
                res = true;
            }
            catch (Exception ex)
            {
                LogHelper.writelog("Add Local Json Error : " + ex.Message.ToString());
                res = false;
            }
            return res;
        }
    }
}