namespace OfficeApplication.BLL.Modals
{
    public class AppStatic
    {
        public static string SUCCESS = "Success";
        public static string ERROR = "Error";
        public static string MANAGESECTION = "MANAGESECTION";

        #region DocumentsUploadType

        public static string NEW = "NEW";
        public static string UPDATED = "UPDATED";
        public static string REMOVED = "REMOVED";
        public static string COMPLETED = "COMPLETED";

        #endregion
        public enum RiskFactorType
        {
            VeryLowRisk = 1,
            LowRisk = 2,
            MediumRisk = 3,
            HighRisk = 4,
            VeryHighRisk = 5
        };
    }
}
