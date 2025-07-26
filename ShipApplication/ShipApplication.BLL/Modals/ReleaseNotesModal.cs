using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipApplication.BLL.Modals
{
    //RDBJ 10/27/2021 Added this Modal
    public class ReleaseNotesModal
    {
        public List<ReleaseNotes> ReleaseNotes { get; set; }
    }

    //RDBJ 10/27/2021
    public class ReleaseNotes
    {
        public string AppVersion { get; set; }
        public string AppDescription { get; set; }
        public DateTime? AppPublishDate { get; set; }
        public int NoOfFilesAffected { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
    //End RDBJ 10/27/2021
}
