using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttachmentSCVInterface.Common
{
    public class ConverterTool
    {
        //2015/1/27 0:00:00
        public static string DateConverter(string date)
        {
            if (!string.IsNullOrEmpty(date) && date != " ")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("to_date('").Append(Convert.ToDateTime(date).ToString("MM/dd/yyyy")).Append("','MM/DD/yyyy')");
                return sb.ToString();
            }
            else
            {
                return "null";
            }
        }
    }
}
