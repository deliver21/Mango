namespace Mango.Web.Utilities
{
    //Static Details
    public class SD
    {
        //get the coupon Url for Coupon Api's with AppSetting
        public static string CouponAPIBase { get; set; }
        //get the Auth Url for Auth Api's with AppSetting
        public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }

        //Roles
        public static string RoleAdmin = "Admin";
        public static string RoleCustomer = "Customer";

        //Token Key
        public static string tokenCookie = "JWTToken";

        //Http Verbs for APiType
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
