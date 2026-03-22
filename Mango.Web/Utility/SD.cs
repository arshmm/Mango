namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public enum ApiType
        {
            GET, POST, PUT, DELETE
        }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";


    }
}
