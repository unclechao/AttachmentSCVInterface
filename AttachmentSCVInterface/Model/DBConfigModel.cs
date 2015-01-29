using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttachmentSCVInterface.Model
{
    public class DBConfigModel
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 间隔时间
        /// </summary>
        public string Time_Interval { get; set; }

        /// <summary>
        /// 运行状态
        /// </summary>
        public string Run_Status { get; set; }
    }
}
