using System;

namespace ShipApplication.BLL.Modals
{
    public class FeedbackFormModal
    {
        public string ShipName { get; set; }
        public string ShipCode { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsMailSent { get; set; }
        public Nullable<bool> IsSynced { get; set; }
        public string ToEmail { get; set; }
        public string AttachmentPath { get; set; }
        public string AttachmentFileName { get; set; }
        public string FeedbackBy { get; set; }
    }
}
