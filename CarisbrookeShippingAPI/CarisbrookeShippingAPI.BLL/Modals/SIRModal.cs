using CarisbrookeShippingAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarisbrookeShippingAPI.BLL.Modals
{
    public class SIRModal
    {
        public SuperintendedInspectionReport SuperintendedInspectionReport { get; set; }
        public List<SIRNote> SIRNote { get; set; }
        public List<SIRAdditionalNote> SIRAdditionalNote { get; set; }
        public bool NotesChanged { get; set; }
        public bool AdditionalNotesChanged { get; set; }
        public bool DeficienciesChanged { get; set; }
        public List<GIRDeficiencies> GIRDeficiencies { get; set; }
    }

    //RDBJ 09/25/2021 
    public class SuperintendedInspectionReport
    {
        public long SIRFormID { get; set; }
        public Nullable<long> ShipID { get; set; }
        public string ShipName { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Port { get; set; }
        public string Master { get; set; }
        public string Superintended { get; set; }
        public string Section1_1_Condition { get; set; }
        public string Section1_1_Comment { get; set; }
        public string Section1_2_Condition { get; set; }
        public string Section1_2_Comment { get; set; }
        public string Section1_3_Condition { get; set; }
        public string Section1_3_Comment { get; set; }
        public string Section1_4_Condition { get; set; }
        public string Section1_4_Comment { get; set; }
        public string Section1_5_Condition { get; set; }
        public string Section1_5_Comment { get; set; }
        public string Section1_6_Condition { get; set; }
        public string Section1_6_Comment { get; set; }
        public string Section1_7_Condition { get; set; }
        public string Section1_7_Comment { get; set; }
        public string Section1_8_Condition { get; set; }
        public string Section1_8_Comment { get; set; }
        public string Section1_9_Condition { get; set; }
        public string Section1_9_Comment { get; set; }
        public string Section1_10_Condition { get; set; }
        public string Section1_10_Comment { get; set; }
        public string Section1_11_Condition { get; set; }
        public string Section1_11_Comment { get; set; }
        public string Section2_1_Condition { get; set; }
        public string Section2_1_Comment { get; set; }
        public string Section2_2_Condition { get; set; }
        public string Section2_2_Comment { get; set; }
        public string Section2_3_Condition { get; set; }
        public string Section2_3_Comment { get; set; }
        public string Section2_4_Condition { get; set; }
        public string Section2_4_Comment { get; set; }
        public string Section2_5_Condition { get; set; }
        public string Section2_5_Comment { get; set; }
        public string Section2_6_Condition { get; set; }
        public string Section2_6_Comment { get; set; }
        public string Section2_7_Condition { get; set; }
        public string Section2_7_Comment { get; set; }
        public string Section3_1_Condition { get; set; }
        public string Section3_1_Comment { get; set; }
        public string Section3_2_Condition { get; set; }
        public string Section3_2_Comment { get; set; }
        public string Section3_3_Condition { get; set; }
        public string Section3_3_Comment { get; set; }
        public string Section3_4_Condition { get; set; }
        public string Section3_4_Comment { get; set; }
        public string Section3_5_Condition { get; set; }
        public string Section3_5_Comment { get; set; }
        public string Section4_1_Condition { get; set; }
        public string Section4_1_Comment { get; set; }
        public string Section4_2_Condition { get; set; }
        public string Section4_2_Comment { get; set; }
        public string Section4_3_Condition { get; set; }
        public string Section4_3_Comment { get; set; }
        public string Section5_1_Condition { get; set; }
        public string Section5_1_Comment { get; set; }
        public string Section5_6_Condition { get; set; }
        public string Section5_6_Comment { get; set; }
        public string Section5_8_Condition { get; set; }
        public string Section5_8_Comment { get; set; }
        public string Section5_9_Condition { get; set; }
        public string Section5_9_Comment { get; set; }
        public string Section6_1_Condition { get; set; }
        public string Section6_1_Comment { get; set; }
        public string Section6_2_Condition { get; set; }
        public string Section6_2_Comment { get; set; }
        public string Section6_3_Condition { get; set; }
        public string Section6_3_Comment { get; set; }
        public string Section6_4_Condition { get; set; }
        public string Section6_4_Comment { get; set; }
        public string Section6_5_Condition { get; set; }
        public string Section6_5_Comment { get; set; }
        public string Section6_6_Condition { get; set; }
        public string Section6_6_Comment { get; set; }
        public string Section6_7_Condition { get; set; }
        public string Section6_7_Comment { get; set; }
        public string Section6_8_Condition { get; set; }
        public string Section6_8_Comment { get; set; }
        public string Section7_1_Condition { get; set; }
        public string Section7_1_Comment { get; set; }
        public string Section7_2_Condition { get; set; }
        public string Section7_2_Comment { get; set; }
        public string Section7_3_Condition { get; set; }
        public string Section7_3_Comment { get; set; }
        public string Section7_4_Condition { get; set; }
        public string Section7_4_Comment { get; set; }
        public string Section7_5_Condition { get; set; }
        public string Section7_5_Comment { get; set; }
        public string Section7_6_Condition { get; set; }
        public string Section7_6_Comment { get; set; }
        public string Section8_1_Condition { get; set; }
        public string Section8_1_Comment { get; set; }
        public string Section8_2_Condition { get; set; }
        public string Section8_2_Comment { get; set; }
        public string Section8_3_Condition { get; set; }
        public string Section8_3_Comment { get; set; }
        public string Section8_4_Condition { get; set; }
        public string Section8_4_Comment { get; set; }
        public string Section8_5_Condition { get; set; }
        public string Section8_5_Comment { get; set; }
        public string Section8_6_Condition { get; set; }
        public string Section8_6_Comment { get; set; }
        public string Section8_7_Condition { get; set; }
        public string Section8_7_Comment { get; set; }
        public string Section8_8_Condition { get; set; }
        public string Section8_8_Comment { get; set; }
        public string Section8_9_Condition { get; set; }
        public string Section8_9_Comment { get; set; }
        public string Section8_10_Condition { get; set; }
        public string Section8_10_Comment { get; set; }
        public string Section8_11_Condition { get; set; }
        public string Section8_11_Comment { get; set; }
        public string Section8_12_Condition { get; set; }
        public string Section8_12_Comment { get; set; }
        public string Section8_13_Condition { get; set; }
        public string Section8_13_Comment { get; set; }
        public string Section8_14_Condition { get; set; }
        public string Section8_14_Comment { get; set; }
        public string Section8_15_Condition { get; set; }
        public string Section8_15_Comment { get; set; }
        public string Section8_16_Condition { get; set; }
        public string Section8_16_Comment { get; set; }
        public string Section8_17_Condition { get; set; }
        public string Section8_17_Comment { get; set; }
        public string Section8_18_Condition { get; set; }
        public string Section8_18_Comment { get; set; }
        public string Section8_19_Condition { get; set; }
        public string Section8_19_Comment { get; set; }
        public string Section8_20_Condition { get; set; }
        public string Section8_20_Comment { get; set; }
        public string Section8_21_Condition { get; set; }
        public string Section8_21_Comment { get; set; }
        public string Section8_22_Condition { get; set; }
        public string Section8_22_Comment { get; set; }
        public string Section8_23_Condition { get; set; }
        public string Section8_23_Comment { get; set; }
        public string Section8_24_Condition { get; set; }
        public string Section8_24_Comment { get; set; }
        public string Section8_25_Condition { get; set; }
        public string Section8_25_Comment { get; set; }
        public string Section9_1_Condition { get; set; }
        public string Section9_1_Comment { get; set; }
        public string Section9_2_Condition { get; set; }
        public string Section9_2_Comment { get; set; }
        public string Section9_3_Condition { get; set; }
        public string Section9_3_Comment { get; set; }
        public string Section9_4_Condition { get; set; }
        public string Section9_4_Comment { get; set; }
        public string Section9_5_Condition { get; set; }
        public string Section9_5_Comment { get; set; }
        public string Section9_6_Condition { get; set; }
        public string Section9_6_Comment { get; set; }
        public string Section9_7_Condition { get; set; }
        public string Section9_7_Comment { get; set; }
        public string Section9_8_Condition { get; set; }
        public string Section9_8_Comment { get; set; }
        public string Section9_9_Condition { get; set; }
        public string Section9_9_Comment { get; set; }
        public string Section9_10_Condition { get; set; }
        public string Section9_10_Comment { get; set; }
        public string Section9_11_Condition { get; set; }
        public string Section9_11_Comment { get; set; }
        public string Section9_12_Condition { get; set; }
        public string Section9_12_Comment { get; set; }
        public string Section9_13_Condition { get; set; }
        public string Section9_13_Comment { get; set; }
        public string Section9_14_Condition { get; set; }
        public string Section9_14_Comment { get; set; }
        public string Section9_15_Condition { get; set; }


        // RDBJ 02/15/2022
        public string Section9_16_Condition { get; set; }
        public string Section9_16_Comment { get; set; }
        public string Section9_17_Condition { get; set; }
        public string Section9_17_Comment { get; set; }
        // End RDBJ 02/15/2022
        public string Section9_15_Comment { get; set; }
        public string Section10_1_Condition { get; set; }
        public string Section10_1_Comment { get; set; }
        public string Section10_2_Condition { get; set; }
        public string Section10_2_Comment { get; set; }
        public string Section10_3_Condition { get; set; }
        public string Section10_3_Comment { get; set; }
        public string Section10_4_Condition { get; set; }
        public string Section10_4_Comment { get; set; }
        public string Section10_5_Condition { get; set; }
        public string Section10_5_Comment { get; set; }
        public string Section10_6_Condition { get; set; }
        public string Section10_6_Comment { get; set; }
        public string Section10_7_Condition { get; set; }
        public string Section10_7_Comment { get; set; }
        public string Section10_8_Condition { get; set; }
        public string Section10_8_Comment { get; set; }
        public string Section10_9_Condition { get; set; }
        public string Section10_9_Comment { get; set; }
        public string Section10_10_Condition { get; set; }
        public string Section10_10_Comment { get; set; }
        public string Section10_11_Condition { get; set; }
        public string Section10_11_Comment { get; set; }
        public string Section10_12_Condition { get; set; }
        public string Section10_12_Comment { get; set; }
        public string Section10_13_Condition { get; set; }
        public string Section10_13_Comment { get; set; }
        public string Section10_14_Condition { get; set; }
        public string Section10_14_Comment { get; set; }
        public string Section10_15_Condition { get; set; }
        public string Section10_15_Comment { get; set; }
        public string Section10_16_Condition { get; set; }
        public string Section10_16_Comment { get; set; }
        public string Section11_1_Condition { get; set; }
        public string Section11_1_Comment { get; set; }
        public string Section11_2_Condition { get; set; }
        public string Section11_2_Comment { get; set; }
        public string Section11_3_Condition { get; set; }
        public string Section11_3_Comment { get; set; }
        public string Section11_4_Condition { get; set; }
        public string Section11_4_Comment { get; set; }
        public string Section11_5_Condition { get; set; }
        public string Section11_5_Comment { get; set; }
        public string Section11_6_Condition { get; set; }
        public string Section11_6_Comment { get; set; }
        public string Section11_7_Condition { get; set; }
        public string Section11_7_Comment { get; set; }
        public string Section11_8_Condition { get; set; }
        public string Section11_8_Comment { get; set; }
        public string Section12_1_Condition { get; set; }
        public string Section12_1_Comment { get; set; }
        public string Section12_2_Condition { get; set; }
        public string Section12_2_Comment { get; set; }
        public string Section12_3_Condition { get; set; }
        public string Section12_3_Comment { get; set; }
        public string Section12_4_Condition { get; set; }
        public string Section12_4_Comment { get; set; }
        public string Section12_5_Condition { get; set; }
        public string Section12_5_Comment { get; set; }
        public string Section12_6_Condition { get; set; }
        public string Section12_6_Comment { get; set; }
        public string Section13_1_Condition { get; set; }
        public string Section13_1_Comment { get; set; }
        public string Section13_2_Condition { get; set; }
        public string Section13_2_Comment { get; set; }
        public string Section13_3_Condition { get; set; }
        public string Section13_3_Comment { get; set; }
        public string Section13_4_Condition { get; set; }
        public string Section13_4_Comment { get; set; }
        public string Section14_1_Condition { get; set; }
        public string Section14_1_Comment { get; set; }
        public string Section14_2_Condition { get; set; }
        public string Section14_2_Comment { get; set; }
        public string Section14_3_Condition { get; set; }
        public string Section14_3_Comment { get; set; }
        public string Section14_4_Condition { get; set; }
        public string Section14_4_Comment { get; set; }
        public string Section14_5_Condition { get; set; }
        public string Section14_5_Comment { get; set; }
        public string Section14_6_Condition { get; set; }
        public string Section14_6_Comment { get; set; }
        public string Section14_7_Condition { get; set; }
        public string Section14_7_Comment { get; set; }
        public string Section14_8_Condition { get; set; }
        public string Section14_8_Comment { get; set; }
        public string Section14_9_Condition { get; set; }
        public string Section14_9_Comment { get; set; }
        public string Section14_10_Condition { get; set; }
        public string Section14_10_Comment { get; set; }
        public string Section14_11_Condition { get; set; }
        public string Section14_11_Comment { get; set; }
        public string Section14_12_Condition { get; set; }
        public string Section14_12_Comment { get; set; }
        public string Section14_13_Condition { get; set; }
        public string Section14_13_Comment { get; set; }
        public string Section14_14_Condition { get; set; }
        public string Section14_14_Comment { get; set; }
        public string Section14_15_Condition { get; set; }
        public string Section14_15_Comment { get; set; }
        public string Section14_16_Condition { get; set; }
        public string Section14_16_Comment { get; set; }
        public string Section14_17_Condition { get; set; }
        public string Section14_17_Comment { get; set; }
        public string Section14_18_Condition { get; set; }
        public string Section14_18_Comment { get; set; }
        public string Section14_19_Condition { get; set; }
        public string Section14_19_Comment { get; set; }
        public string Section14_20_Condition { get; set; }
        public string Section14_20_Comment { get; set; }
        public string Section14_21_Condition { get; set; }
        public string Section14_21_Comment { get; set; }
        public string Section14_22_Condition { get; set; }
        public string Section14_22_Comment { get; set; }
        public string Section14_23_Condition { get; set; }
        public string Section14_23_Comment { get; set; }
        public string Section14_24_Condition { get; set; }
        public string Section14_24_Comment { get; set; }
        public string Section14_25_Condition { get; set; }
        public string Section14_25_Comment { get; set; }
        public string Section15_1_Condition { get; set; }
        public string Section15_1_Comment { get; set; }
        public string Section15_2_Condition { get; set; }
        public string Section15_2_Comment { get; set; }
        public string Section15_3_Condition { get; set; }
        public string Section15_3_Comment { get; set; }
        public string Section15_4_Condition { get; set; }
        public string Section15_4_Comment { get; set; }
        public string Section15_5_Condition { get; set; }
        public string Section15_5_Comment { get; set; }
        public string Section15_6_Condition { get; set; }
        public string Section15_6_Comment { get; set; }
        public string Section15_7_Condition { get; set; }
        public string Section15_7_Comment { get; set; }
        public string Section15_8_Condition { get; set; }
        public string Section15_8_Comment { get; set; }
        public string Section15_9_Condition { get; set; }
        public string Section15_9_Comment { get; set; }
        public string Section15_10_Condition { get; set; }
        public string Section15_10_Comment { get; set; }
        public string Section15_11_Condition { get; set; }
        public string Section15_11_Comment { get; set; }
        public string Section15_12_Condition { get; set; }
        public string Section15_12_Comment { get; set; }
        public string Section15_13_Condition { get; set; }
        public string Section15_13_Comment { get; set; }
        public string Section15_14_Condition { get; set; }
        public string Section15_14_Comment { get; set; }
        public string Section15_15_Condition { get; set; }
        public string Section15_15_Comment { get; set; }
        public string Section16_1_Condition { get; set; }
        public string Section16_1_Comment { get; set; }
        public string Section16_2_Condition { get; set; }
        public string Section16_2_Comment { get; set; }
        public string Section16_3_Condition { get; set; }
        public string Section16_3_Comment { get; set; }
        public string Section16_4_Condition { get; set; }
        public string Section16_4_Comment { get; set; }
        public string Section17_1_Condition { get; set; }
        public string Section17_1_Comment { get; set; }
        public string Section17_2_Condition { get; set; }
        public string Section17_2_Comment { get; set; }
        public string Section17_3_Condition { get; set; }
        public string Section17_3_Comment { get; set; }
        public string Section17_4_Condition { get; set; }
        public string Section17_4_Comment { get; set; }
        public string Section17_5_Condition { get; set; }
        public string Section17_5_Comment { get; set; }
        public string Section17_6_Condition { get; set; }
        public string Section17_6_Comment { get; set; }
        public string Section18_1_Condition { get; set; }
        public string Section18_1_Comment { get; set; }
        public string Section18_2_Condition { get; set; }
        public string Section18_2_Comment { get; set; }
        public string Section18_3_Condition { get; set; }
        public string Section18_3_Comment { get; set; }
        public string Section18_4_Condition { get; set; }
        public string Section18_4_Comment { get; set; }
        public string Section18_5_Condition { get; set; }
        public string Section18_5_Comment { get; set; }
        public string Section18_6_Condition { get; set; }
        public string Section18_6_Comment { get; set; }
        public string Section18_7_Condition { get; set; }
        public string Section18_7_Comment { get; set; }

        // RDBJ 02/15/2022
        public string Section18_8_Condition { get; set; }
        public string Section18_8_Comment { get; set; }
        public string Section18_9_Condition { get; set; }
        public string Section18_9_Comment { get; set; }
        // End RDBJ 02/15/2022

        public Nullable<bool> IsSynced { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<bool> SavedAsDraft { get; set; }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public Nullable<decimal> FormVersion { get; set; }
        public Nullable<int> isDelete { get; set; } // RDBJ 01/05/2022
    }
    //End RDBJ 09/25/2021 

    //RDBJ 09/25/2021 
    public class SIRNote
    {
        public long NoteID { get; set; }
        public Nullable<long> SIRFormID { get; set; }
        public string Number { get; set; }
        public string Note { get; set; }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public System.Guid NotesUniqueID { get; set; }  // RDBJ 04/02/2022
        public Nullable<int> IsDeleted { get; set; } // RDBJ 04/02/2022
    }
    //End RDBJ 09/25/2021 

    //RDBJ 09/25/2021 
    public class SIRAdditionalNote
    {
        public long NoteID { get; set; }
        public Nullable<long> SIRFormID { get; set; }
        public string Number { get; set; }
        public string Note { get; set; }
        public Nullable<System.Guid> UniqueFormID { get; set; }
        public System.Guid NotesUniqueID { get; set; }  // RDBJ 04/02/2022
        public Nullable<int> IsDeleted { get; set; } // RDBJ 04/02/2022
    }
    //End RDBJ 09/25/2021 

    public class SIRData
    {
        public long SIRFormID { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string Master { get; set; }
        public string Ship { get; set; }
        public string ShipName { get; set; }
        public string Superintended { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Guid? UniqueFormID { get; set; }
    }

    public class SIRDeficiencies
    {
        public SIRDeficiencies()
        {
            SIRDeficienciesFile = new List<SIRDeficienciesFile>();
        }
        public int DeficienciesID { get; set; }
        public Nullable<long> SIRFormID { get; set; }
        public Nullable<long> OfficeSIRFormID { get; set; }
        public int No { get; set; }
        public Nullable<System.DateTime> DateRaised { get; set; }
        public string Deficiency { get; set; }
        public Nullable<System.DateTime> DateClosed { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Ship { get; set; }
        public Nullable<bool> IsClose { get; set; }
        public string ReportType { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string SIRNo { get; set; }
        public List<SIRDeficienciesFile> SIRDeficienciesFile { get; set; }
        public string ItemNo { get; set; }
        public string Section { get; set; }
        public bool IsSynced { get; set; }
        public string Inspector { get; set; }
        public string Port { get; set; }
    }

    public class SIRDeficienciesFile
    {
        public int GIRDeficienciesFileID { get; set; }
        public long? DeficienciesID { get; set; }
        public string FileName { get; set; }
        public string StorePath { get; set; }
        public string IsUpload { get; set; }
    }
}
