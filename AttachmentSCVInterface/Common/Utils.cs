using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttachmentSCVInterface.DAL;
using AttachmentSCVInterface.Model;
using Oracle.DataAccess.Client;

namespace AttachmentSCVInterface.Common
{
    public class Utils
    {
        public const string pv_name = "PV数据库";
        public static string pv_type = "DBLINK";

        /// <summary>
        /// 获取PV数据库连接信息
        /// </summary>
        /// <returns></returns>
        public static DBConfigModel GetPVConfigInfo()
        {
            DBConfigModel pvDBConfigInfo = null;
            try
            {
                pvDBConfigInfo = new DBConfigModel();
                string ScvConnStr = ConfigurationManager.ConnectionStrings["ScvConnectionString"].ConnectionString;
                using (OracleConnection SCVConn = new OracleConnection(ScvConnStr))
                {
                    SCVConn.Open();
                    string cmd = @"select * from Interface_Param where Interface_name = '" + Utils.pv_name + "'";
                    var SCVCmd = new OracleCommand(cmd, SCVConn);
                    var reader = SCVCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string ip = reader["IP_ADDRESS"].ToString();
                        string port = reader["PORT"].ToString();
                        string db_name = reader["DB_SSID"].ToString();
                        string user_id = reader["USER_ID"].ToString();
                        string pwd = reader["password"].ToString();
                        pwd = string.IsNullOrEmpty(pwd) ? string.Empty : Base64.Base64Decode(pwd);
                        string connstr = "Data Source=" + ip + "," + port + ";User ID=" + user_id + ";Password=" + pwd + ";Initial Catalog=" + db_name + ";Connect Timeout=10";
                        pvDBConfigInfo.ConnectionString = connstr;
                        pvDBConfigInfo.Time_Interval = reader["TIME_INTERVAL"].ToString();
                        pvDBConfigInfo.Run_Status = reader["RUN_STATUS"].ToString();
                        Utils.pv_type = reader["Interface_type"].ToString();
                    }
                    reader.Dispose();
                    return pvDBConfigInfo;
                }
            }
            catch (Exception ex)
            {
                Log.LoadInfo(Utils.pv_name + "GetPVConfigInfo异常:" + ex);
                Console.WriteLine(Utils.pv_name + "GetPVConfigInfo异常:" + ex.Message);
                return pvDBConfigInfo;
            }
        }
    }
}
