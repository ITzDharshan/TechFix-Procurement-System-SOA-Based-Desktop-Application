using System.Data.SqlClient;

namespace TechFixShopAPI.DataAccees
{
    public class DataAccessLayer
    {
        public static SqlConnection CreateConnection()
        {
            SqlConnection con = new SqlConnection(@"Server=DESKTOP-2VPR9IB\SQLEXPRESS;Database=TechFix;Trusted_Connection=True;");
            return con;
        }
    }
}