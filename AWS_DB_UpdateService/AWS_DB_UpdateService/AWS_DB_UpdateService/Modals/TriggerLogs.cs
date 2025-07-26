using System;

namespace AWS_DB_UpdateService.Modals
{
    public class TriggerLogs
    {
        public long ID { get; set; }
        public string empnry01 { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public string TaskID { get; set; }
        public string UploadStatus { get; set; }
        public bool IsUploaded { get; set; }
    }
}
