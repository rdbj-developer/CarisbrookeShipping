using System;
using System.Collections.Generic;

namespace CarisbrookeShippingService.BLL.Modals
{
    public class DeficienciesNote
    {
        public long NoteID { get; set; }
        public Nullable<long> DeficienciesID { get; set; }
        public Nullable<long> GIRFormID { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public List<GIRDeficienciesCommentFile> GIRDeficienciesCommentFile { get; set; }
    }
    public class GIRDeficienciesCommentFile
    {
        public System.Guid CommentFileUniqueID { get; set; }
        public System.Guid NoteUniqueID { get; set; }
        public long GIRCommentFileID { get; set; }
        public long NoteID { get; set; }
        public Nullable<long> DeficienciesID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }
}
