using System;
using System.Collections.Generic;

namespace ShipApplication.BLL.Modals
{
    public class GIRDeficienciesNote
    {
        public Guid UniqueFormID { get; set; }
        public System.Guid NoteUniqueID { get; set; }
        public System.Guid DeficienciesUniqueID { get; set; }
        public long NoteID { get; set; }
        public Nullable<long> DeficienciesID { get; set; }
        public Nullable<long> GIRFormID { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public List<GIRDeficienciesCommentFile> GIRDeficienciesCommentFile { get; set; }
        public int? isNew { get; set; } //RDBJ 10/26/2021
        public bool? IsResolution { get; set; }  // JSL 07/05/2022
    }
    public class GIRDeficienciesCommentFile
    {
        public System.Guid CommentFileUniqueID { get; set; }
        public System.Guid NoteUniqueID { get; set; }
        public long GIRCommentFileID { get; set; }
        public long NoteID { get; set; }
        public long DeficienciesID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }
}
