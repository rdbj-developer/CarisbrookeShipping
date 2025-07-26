using System.Collections.Generic;

namespace AWS_DB_UpdateService.Modals
{
    public class APIRequest
    {
        public dsisy01 objdsisy01 { get; set; }
        public TriggerLogs objTriggerLogs { get; set; }
        public APIRequest()
        {
            objdsisy01 = new dsisy01();
            objTriggerLogs = new TriggerLogs();
        }
    }
    public class LoginRes
    {
        public LoginResponse Response { get; set; }
    }
    public class LoginResponse
    {
        public string Status { get; set; }
        public object ResponseData { get; set; }
    }

    public class UploadFileRes
    {
        public UploadFileResStatus Response { get; set; }
    }
    public class UploadFileResStatus
    {
        public string Status { get; set; }
        public UploadFileResData ResponseData { get; set; }
    }
    public class UploadFileResData
    {
        public UploadFileResult Result { get; set; }
        public Error_Failed Error { get; set; }
    }
    public class UploadFileResult
    {
        public string Type { get; set; }
        public string Format { get; set; }
        public string Content { get; set; }
    }
    public class Failed_Result
    {
        public Error_Failed Error { get; set; }
    }
    public class Error_Failed
    {
        public string Code { get; set; }
        public string Description { get; set; }
        //public ContentItem Content { get; set; }
        public string Content { get; set; }
    }

    public class ContentItem
    {
        public List<CItem> Item { get; set; }
    }

    public class CItem
    {
        public string ExternalId { get; set; }
        public string Error { get; set; }
    }
}
