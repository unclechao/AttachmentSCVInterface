using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttachmentSCVInterface.Common;
using AttachmentSCVInterface.DAL;
using AttachmentSCVInterface.Model;

namespace AttachmentSCVInterface.Timer
{
    public class PVTimer
    {
        public static System.Timers.Timer pvTimer;
        public PVTimer() { }

        /// <summary>
        /// 启动定时器
        /// </summary>
        public void StartPVTimer()
        {
            try
            {
                pvTimer = new System.Timers.Timer();
                DBConfigModel pvDBModel = Utils.GetPVConfigInfo();
                string pv_status = pvDBModel.Run_Status;
                string pv_time_interval = pvDBModel.Time_Interval;
                pvTimer.Interval = Int16.Parse(pv_time_interval) * 60 * 1000;
                pvTimer.Enabled = true;
                pvTimer.AutoReset = true;
#if DEBUG
                pvTimer.Interval = 1000 * 5;
#endif
                pvTimer.Elapsed += pvTimer_Elapsed;
                pvTimer.Start();
                Log.LoadInfo("定时器启动成功");
            }
            catch (Exception ex)
            {
                Log.LoadInfo(Utils.pv_name + "定时器启动错误:" + ex);
                Console.WriteLine(Utils.pv_name + "定时器启动错误:" + ex.Message);
            }
        }

        static void pvTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            pvTimer.Stop();
            try
            {
                Log.LoadInfo(Utils.pv_name + "定时器触发");
                DBConfigModel pvDBModel = Utils.GetPVConfigInfo();
                string pv_status = pvDBModel.Run_Status;
                string pv_time_interval = pvDBModel.Time_Interval;
                pvTimer.Interval = Int16.Parse(pv_time_interval) * 60 * 1000;
#if DEBUG
                if (pv_status == "1")
                {
                    pvTimer.Interval = 1000 * 5;
                }
#endif
                if (pv_status == "1")
                {
                    new PVTimerWorker().GetAndInsertFCSData();
                    new PVTimerWorker().GetAndInsertFIFSData();
                    new PVTimerWorker().GetAndInsertFIPData();
                    new PVTimerWorker().GetAndInsertFPSData();
                }
            }
            catch (Exception ex)
            {
                Log.LoadInfo(Utils.pv_name + "定时器触发错误:" + ex);
                Console.WriteLine(Utils.pv_name + "定时器触发错误:" + ex.Message);
            }
            finally
            {
                pvTimer.Start();
            }
        }
    }
}
