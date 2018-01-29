
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSMONEY
{
    public static class DataTa
    {
        public static DataTable GetTable()
        {
            DataTable dt = new DataTable();
            MySqlConnectionStringBuilder mysqlCSB;
            mysqlCSB = new MySqlConnectionStringBuilder();
            mysqlCSB.Server = "xnode.mysql.ukraine.com.ua";
            mysqlCSB.Database = "xnode_loo";
            mysqlCSB.UserID = "xnode_loo";
            mysqlCSB.Password = "mt926ea8";
            using (MySqlConnection con = new MySqlConnection())
            {
                string queryString = @"SELECT name,
                               url,
                               sleep,
                               version,
                               site
                        FROM   inst
                        ";
                con.ConnectionString = mysqlCSB.ConnectionString;
                MySqlCommand com = new MySqlCommand(queryString, con);
                try
                {
                    con.Open();
                    using (MySqlDataReader dr = com.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            dt.Load(dr);
                        }
                    }
                }

                catch (Exception ex)
                {

                }
            }
            return dt;
        }
    
    }
}
