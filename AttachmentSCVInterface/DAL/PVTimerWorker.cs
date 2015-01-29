using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttachmentSCVInterface.Common;
using AttachmentSCVInterface.Model;
using Oracle.DataAccess.Client;

namespace AttachmentSCVInterface.DAL
{
    public class PVTimerWorker
    {
        readonly string PVServiceConn = ConfigurationManager.ConnectionStrings["PVService"].ConnectionString;
        readonly string ScvConn = ConfigurationManager.ConnectionStrings["ScvMidConnectionString"].ConnectionString;
        OracleConnection PVConn = null;
        OracleConnection SVCConn = null;
        OracleTransaction PVtrans = null;
        OracleTransaction SCVtrans = null;
        /// <summary>
        /// FSL_CONSIGNMENT_STATUS表
        /// </summary>
        /// <returns></returns>
        public void GetAndInsertFCSData()
        {
            Log.LoadInfo("GetAndInsertFCSData Function Start");
            string updateCmd = @"update FSL_CONSIGNMENT_STATUS set send_status = '1' , RECEIVETIME = sysdate where send_status = '0'";
            string selectCmd = @"select * from FSL_CONSIGNMENT_STATUS where SEND_STATUS = '0' ";
            StringBuilder insertCmd = new StringBuilder(@"insert  into D_MDM_CONSIGNMENT_STATUS values (");
            try
            {
                PVConn = new OracleConnection(PVServiceConn);
                PVConn.Open();
                Log.LoadInfo("Open PVServiceConn success");
                PVtrans = PVConn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(selectCmd, PVConn);
                OracleDataReader reader = cmd.ExecuteReader();
                List<FSL_CONSIGNMENT_STATUS> fcsList = new List<FSL_CONSIGNMENT_STATUS>();
                //将取出的数据加到list中
                while (reader.Read())
                {
                    #region FSL_CONSIGNMENT_STATUS结构
                    FSL_CONSIGNMENT_STATUS fcs = new FSL_CONSIGNMENT_STATUS();
                    fcs.SEND_CODE = string.IsNullOrEmpty(reader["SEND_CODE"].ToString()) ? " " : reader["SEND_CODE"].ToString();
                    fcs.CONSUME_ID = string.IsNullOrEmpty(reader["CONSUME_ID"].ToString()) ? " " : reader["CONSUME_ID"].ToString();
                    fcs.SEND_STATUS = string.IsNullOrEmpty(reader["SEND_STATUS"].ToString()) ? " " : reader["SEND_STATUS"].ToString();
                    fcs.PLATE_NUMBER = string.IsNullOrEmpty(reader["PLATE_NUMBER"].ToString()) ? " " : reader["PLATE_NUMBER"].ToString();
                    fcs.SEND_TIME = string.IsNullOrEmpty(reader["SEND_TIME"].ToString()) ? " " : reader["SEND_TIME"].ToString();
                    #endregion
                    fcsList.Add(fcs);
                }
                Log.LoadInfo("Get FSL_CONSIGNMENT_STATUS data over, try to update data");
                //将数据更新
                cmd = new OracleCommand(updateCmd, PVConn);
                int deleteCount = cmd.ExecuteNonQuery();
                if (deleteCount != fcsList.Count)
                {
                    Log.LoadInfo("Rollback");
                    PVtrans.Rollback();
                }
                else
                {
                    Log.LoadInfo("Update FSL_CONSIGNMENT_STATUS data over, try to copy data");
                    int count = 0;
                    SVCConn = new OracleConnection(ScvConn);
                    //将list中的数据插入本地数据库中
                    SVCConn.Open();
                    Log.LoadInfo("Open SVCConn success");
                    SCVtrans = SVCConn.BeginTransaction();
                    if (fcsList.Count > 0)
                    {
                        foreach (var fcs in fcsList)
                        {
                            insertCmd.Append("'").Append(fcs.SEND_CODE).Append("','").Append(fcs.PLATE_NUMBER).Append("','").Append(fcs.CONSUME_ID).Append("',").Append(ConverterTool.DateConverter(fcs.SEND_TIME)).Append(",'").Append("0").Append("')");
                            cmd = new OracleCommand(insertCmd.ToString(), SVCConn);
                            int insertCount = cmd.ExecuteNonQuery();
                            count = count + insertCount;
                            insertCmd.Clear();
                            insertCmd.Append(@"insert  into D_MDM_CONSIGNMENT_STATUS values (");
                        }
                    }
                    if (count != fcsList.Count || count == 0)
                    {
                        PVtrans.Rollback();
                        SCVtrans.Rollback();
                        if (count != 0)
                            Log.LoadInfo("Rollback");
                    }
                    else
                    {
                        SCVtrans.Commit();
                        PVtrans.Commit();
                        Log.LoadInfo("Commit");
                        foreach (var item in fcsList)
                        {
                            Log.LoadInfo("Item CONSUME_ID:" + item.CONSUME_ID + " PLATE_NUMBER:" + item.PLATE_NUMBER + " SEND_CODE:" + item.SEND_CODE + " SEND_TIME:" + item.SEND_TIME + " excuted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (PVtrans != null)
                    PVtrans.Rollback();
                if (SCVtrans != null)
                    SCVtrans.Rollback();
                Log.LoadInfo("Expection! Rollback");
                Log.LoadInfo("GetAndInsertFCSData Error, " + ex);
                Console.WriteLine("GetAndInsertFCSData Error, " + ex.Message);
            }
            finally
            {
                if (PVConn != null)
                    PVConn.Dispose();
                if (SVCConn != null)
                    SVCConn.Dispose();
                Log.LoadInfo("GetAndInsertFCSData Function End");
            }
        }

        /// <summary>
        /// FSL_INDENT_FACTCHECK_STATUS表
        /// </summary>
        /// <returns></returns>
        public void GetAndInsertFIFSData()
        {
            Log.LoadInfo("GetAndInsertFIFSData Function Start");
            string updateCmd = @"update FSL_INDENT_FACTCHECK_STATUS set send_status = '1' , RECEIVETIME = sysdate where send_status = '0'";
            string selectCmd = @"select * from FSL_INDENT_FACTCHECK_STATUS where SEND_STATUS = '0'";
            StringBuilder insertCmd = new StringBuilder(@"insert  into D_MDM_I_FACTCHECK_STATUS values (");
            try
            {
                PVConn = new OracleConnection(PVServiceConn);
                PVConn.Open();
                Log.LoadInfo("Open PVServiceConn success");
                PVtrans = PVConn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(selectCmd, PVConn);
                OracleDataReader reader = cmd.ExecuteReader();
                List<FSL_INDENT_FACTCHECK_STATUS> fifsList = new List<FSL_INDENT_FACTCHECK_STATUS>();
                //将取出的数据加到list中
                while (reader.Read())
                {
                    FSL_INDENT_FACTCHECK_STATUS fifs = new FSL_INDENT_FACTCHECK_STATUS();
                    #region FSL_INDENT_FACTCHECK_STATUS结构

                    fifs.FACTCHECK_TIME = string.IsNullOrEmpty(reader["FACTCHECK_TIME"].ToString()) ? "" : reader["FACTCHECK_TIME"].ToString();
                    fifs.CONSUME_ID = string.IsNullOrEmpty(reader["CONSUME_ID"].ToString()) ? "" : reader["CONSUME_ID"].ToString();
                    fifs.OUT_CODE = string.IsNullOrEmpty(reader["OUT_CODE"].ToString()) ? "" : reader["OUT_CODE"].ToString();
                    fifs.SEND_STATUS = string.IsNullOrEmpty(reader["SEND_STATUS"].ToString()) ? "" : reader["SEND_STATUS"].ToString();
                    fifs.PART_CODE = string.IsNullOrEmpty(reader["PART_CODE"].ToString()) ? "" : reader["PART_CODE"].ToString();
                    fifs.PREPARE_CODE = string.IsNullOrEmpty(reader["PREPARE_CODE"].ToString()) ? "" : reader["PREPARE_CODE"].ToString();
                    fifs.PART_NUM = string.IsNullOrEmpty(reader["PART_NUM"].ToString()) ? "" : reader["PART_NUM"].ToString();
                    #endregion
                    fifsList.Add(fifs);
                }
                Log.LoadInfo("Get FSL_INDENT_FACTCHECK_STATUS data over, try to update data");
                //将数据更新
                cmd = new OracleCommand(updateCmd, PVConn);
                int deleteCount = cmd.ExecuteNonQuery();
                if (deleteCount != fifsList.Count)
                {
                    Log.LoadInfo("Rollback");
                    PVtrans.Rollback();
                }
                else
                {
                    Log.LoadInfo("Update FSL_INDENT_FACTCHECK_STATUS data over, try to copy data");
                    int count = 0;
                    //将list中的数据插入本地数据库中
                    SVCConn = new OracleConnection(ScvConn);
                    SVCConn.Open();
                    Log.LoadInfo("Open SVCConn success");
                    SCVtrans = SVCConn.BeginTransaction();
                    if (fifsList.Count > 0)
                    {
                        foreach (var fifs in fifsList)
                        {
                            insertCmd.Append("'").Append(fifs.CONSUME_ID).Append("','").Append(fifs.PART_CODE).Append("','").Append(fifs.PART_NUM).Append("',").Append(ConverterTool.DateConverter(fifs.FACTCHECK_TIME)).Append(",'").Append(fifs.PREPARE_CODE).Append("','").Append(fifs.OUT_CODE).Append("','").Append("0").Append("')");
                            cmd = new OracleCommand(insertCmd.ToString(), SVCConn);
                            int insertCount = cmd.ExecuteNonQuery();
                            count = count + insertCount;
                            insertCmd.Clear();
                            insertCmd.Append(@"insert  into D_MDM_I_FACTCHECK_STATUS values (");
                        }
                    }
                    if (count != fifsList.Count || count == 0)
                    {
                        PVtrans.Rollback();
                        SCVtrans.Rollback();
                        if (count != 0)
                            Log.LoadInfo("Rollback");
                    }
                    else
                    {
                        SCVtrans.Commit();
                        PVtrans.Commit();
                        Log.LoadInfo("Commit");
                        foreach (var item in fifsList)
                        {
                            Log.LoadInfo("Item CONSUME_ID:" + item.CONSUME_ID + " FACTCHECK_TIME:" + item.FACTCHECK_TIME + " OUT_CODE:" + item.OUT_CODE + " PART_CODE:" + item.PART_CODE + " PART_NUM:" + item.PART_NUM + " PREPARE_CODE:" + item.PREPARE_CODE + " excuted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (PVtrans != null)
                    PVtrans.Rollback();
                if (SCVtrans != null)
                    SCVtrans.Rollback();
                Log.LoadInfo("Expection! Rollback");
                Log.LoadInfo("GetAndInsertFIFSData Error, " + ex);
                Console.WriteLine("GetAndInsertFIFSData Error, " + ex.Message);
            }
            finally
            {
                if (PVConn != null)
                    PVConn.Dispose();
                if (SVCConn != null)
                    SVCConn.Dispose();
                Log.LoadInfo("GetAndInsertFIFSData Function End");
            }
        }
        /// <summary>
        /// FSL_PREPARE_STATUS表
        /// </summary>
        /// <returns></returns>
        public void GetAndInsertFPSData()
        {
            Log.LoadInfo("GetAndInsertFPSData Function Start");
            string updateCmd = @"update FSL_PREPARE_STATUS set send_status = '1' , RECEIVETIME = sysdate where send_status = '0'";
            string selectCmd = @"select * from FSL_PREPARE_STATUS  where SEND_STATUS = '0'";
            StringBuilder insertCmd = new StringBuilder(@"insert  into D_MDM_PREPARE_STATUS values (");
            try
            {
                PVConn = new OracleConnection(PVServiceConn);
                PVConn.Open();
                Log.LoadInfo("Open PVServiceConn success");
                PVtrans = PVConn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(selectCmd, PVConn);
                OracleDataReader reader = cmd.ExecuteReader();
                List<FSL_PREPARE_STATUS> fpsList = new List<FSL_PREPARE_STATUS>();
                //将取出的数据加到list中
                while (reader.Read())
                {
                    FSL_PREPARE_STATUS fps = new FSL_PREPARE_STATUS();
                    #region FSL_PREPARE_STATUS结构
                    fps.PREPARE_CODE = string.IsNullOrEmpty(reader["PREPARE_CODE"].ToString()) ? "" : reader["PREPARE_CODE"].ToString();
                    fps.FACTPREPARE_TIME = string.IsNullOrEmpty(reader["FACTPREPARE_TIME"].ToString()) ? "" : reader["FACTPREPARE_TIME"].ToString();
                    fps.SEND_STATUS = string.IsNullOrEmpty(reader["SEND_STATUS"].ToString()) ? "" : reader["SEND_STATUS"].ToString();
                    fps.FACTOUT_TIME = string.IsNullOrEmpty(reader["FACTOUT_TIME"].ToString()) ? "" : reader["FACTOUT_TIME"].ToString();
                    #endregion
                    fpsList.Add(fps);
                }
                Log.LoadInfo("Get FSL_PREPARE_STATUS data over, try to update data");
                //将数据更新
                cmd = new OracleCommand(updateCmd, PVConn);
                int deleteCount = cmd.ExecuteNonQuery();
                if (deleteCount != fpsList.Count)
                {
                    Log.LoadInfo("Rollback");
                    PVtrans.Rollback();
                }
                else
                {
                    Log.LoadInfo("Update FSL_PREPARE_STATUS data over, try to copy data");
                    int count = 0;
                    SVCConn = new OracleConnection(ScvConn);
                    //将list中的数据插入本地数据库中
                    SVCConn.Open();
                    Log.LoadInfo("Open SVCConn success");
                    SCVtrans = SVCConn.BeginTransaction();
                    if (fpsList.Count > 0)
                    {
                        foreach (var fps in fpsList)
                        {
                            insertCmd.Append("'").Append(fps.PREPARE_CODE).Append("',").Append(ConverterTool.DateConverter(fps.FACTPREPARE_TIME)).Append(",").Append(ConverterTool.DateConverter(fps.FACTOUT_TIME)).Append(",'").Append("0").Append("')");
                            cmd = new OracleCommand(insertCmd.ToString(), SVCConn);
                            int insertCount = cmd.ExecuteNonQuery();
                            count = count + insertCount;
                            insertCmd.Clear();
                            insertCmd.Append(@"insert  into D_MDM_PREPARE_STATUS values (");
                        }
                    }
                    if (count != fpsList.Count || count == 0)
                    {
                        PVtrans.Rollback();
                        SCVtrans.Rollback();
                        if (count != 0)
                            Log.LoadInfo("Rollback");
                    }
                    else
                    {
                        SCVtrans.Commit();
                        PVtrans.Commit();
                        Log.LoadInfo("Commit");
                        foreach (var item in fpsList)
                        {
                            Log.LoadInfo("Item FACTOUT_TIME:" + item.FACTOUT_TIME + " FACTPREPARE_TIME:" + item.FACTPREPARE_TIME + " PREPARE_CODE:" + item.PREPARE_CODE + " excuted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (PVtrans != null)
                    PVtrans.Rollback();
                if (SCVtrans != null)
                    SCVtrans.Rollback();
                Log.LoadInfo("Expection! Rollback");
                Log.LoadInfo("GetAndInsertFPSData Error, " + ex);
                Console.WriteLine("GetAndInsertFPSData Error, " + ex.Message);
            }
            finally
            {
                if (PVConn != null)
                    PVConn.Dispose();
                if (SVCConn != null)
                    SVCConn.Dispose();
                Log.LoadInfo("GetAndInsertFPSData Function End");
            }
        }

        /// <summary>
        /// FSL_INDENT_PREPARE表
        /// </summary>
        /// <returns></returns>
        public void GetAndInsertFIPData()
        {
            Log.LoadInfo("GetAndInsertFIPData Function Start");
            string updateCmd = @"update FSL_INDENT_PREPARE set send_status = '1' , RECEIVETIME = sysdate where send_status = '0'";
            string selectCmd = @"select * from FSL_INDENT_PREPARE  where SEND_STATUS = '0'";
            StringBuilder insertCmd = new StringBuilder(@"insert  into D_MDM_INDENT_PREPARE values (");
            try
            {
                PVConn = new OracleConnection(PVServiceConn);
                Log.LoadInfo("Open PVServiceConn success");
                PVConn.Open();
                PVtrans = PVConn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(selectCmd, PVConn);
                OracleDataReader reader = cmd.ExecuteReader();
                List<FSL_INDENT_PREPARE> fipList = new List<FSL_INDENT_PREPARE>();
                //将取出的数据加到list中
                while (reader.Read())
                {
                    FSL_INDENT_PREPARE fip = new FSL_INDENT_PREPARE();
                    #region FSL_INDENT_PREPARE结构
                    fip.INDENT_CODE = string.IsNullOrEmpty(reader["INDENT_CODE"].ToString()) ? "" : reader["INDENT_CODE"].ToString();
                    fip.PREPARE_CODE = string.IsNullOrEmpty(reader["PREPARE_CODE"].ToString()) ? "" : reader["PREPARE_CODE"].ToString();
                    fip.NET_CODE = string.IsNullOrEmpty(reader["NET_CODE"].ToString()) ? "" : reader["NET_CODE"].ToString();
                    fip.SEND_STATUS = string.IsNullOrEmpty(reader["SEND_STATUS"].ToString()) ? "" : reader["SEND_STATUS"].ToString();
                    fip.PARTS_CODE = string.IsNullOrEmpty(reader["PARTS_CODE"].ToString()) ? "" : reader["PARTS_CODE"].ToString();
                    fip.PART_NUM = string.IsNullOrEmpty(reader["PART_NUM"].ToString()) ? "" : reader["PART_NUM"].ToString();
                    fip.PREPARE_CODE = string.IsNullOrEmpty(reader["PREPARE_CODE"].ToString()) ? "" : reader["PREPARE_CODE"].ToString();
                    fip.PARTS_NAME = string.IsNullOrEmpty(reader["PARTS_NAME"].ToString()) ? "" : reader["PARTS_NAME"].ToString();
                    fip.INDENT_TYPE = string.IsNullOrEmpty(reader["INDENT_TYPE"].ToString()) ? "" : reader["INDENT_TYPE"].ToString();
                    fip.PVAUDITING_TIME = string.IsNullOrEmpty(reader["PVAUDITING_TIME"].ToString()) ? "" : reader["PVAUDITING_TIME"].ToString();
                    fip.PREPARE_TIME = string.IsNullOrEmpty(reader["PREPARE_TIME"].ToString()) ? "" : reader["PREPARE_TIME"].ToString();
                    fip.YS_FS = string.IsNullOrEmpty(reader["YS_FS"].ToString()) ? "" : reader["YS_FS"].ToString();
                    fip.STORAGE_PLACE = string.IsNullOrEmpty(reader["STORAGE_PLACE"].ToString()) ? "" : reader["STORAGE_PLACE"].ToString();
                    fip.OUT_CKCODE = string.IsNullOrEmpty(reader["OUT_CKCODE"].ToString()) ? "" : reader["OUT_CKCODE"].ToString();
                    fip.END_CODE = string.IsNullOrEmpty(reader["END_CODE"].ToString()) ? "" : reader["END_CODE"].ToString();
                    #endregion
                    fipList.Add(fip);
                }
                Log.LoadInfo("Get FSL_INDENT_PREPARE data over, try to update data");
                //将数据更新
                cmd = new OracleCommand(updateCmd, PVConn);
                int deleteCount = cmd.ExecuteNonQuery();
                if (deleteCount != fipList.Count)
                {
                    Log.LoadInfo("Rollback");
                    PVtrans.Rollback();
                }
                else
                {
                    Log.LoadInfo("Update FSL_INDENT_PREPARE data over, try to copy data");
                    int count = 0;
                    //将list中的数据插入本地数据库中
                    SVCConn = new OracleConnection(ScvConn);
                    SVCConn.Open();
                    Log.LoadInfo("Open SVCConn success");
                    SCVtrans = SVCConn.BeginTransaction();
                    if (fipList.Count > 0)
                    {
                        foreach (var fip in fipList)
                        {
                            insertCmd.Append("'").Append(fip.INDENT_CODE).Append("','").Append(fip.PREPARE_CODE).Append("','").Append(fip.NET_CODE).Append("','").Append(fip.PARTS_CODE).Append("','").Append(fip.PARTS_NAME).Append("','").Append(fip.PART_NUM).Append("','").Append(fip.INDENT_TYPE).Append("',").Append(ConverterTool.DateConverter(fip.PVAUDITING_TIME)).Append(",").Append(ConverterTool.DateConverter(fip.PREPARE_TIME)).Append(",'").Append(fip.YS_FS).Append("','").Append(fip.STORAGE_PLACE).Append("','").Append(fip.OUT_CKCODE).Append("','").Append(fip.END_CODE).Append("','").Append("0").Append("')");
                            cmd = new OracleCommand(insertCmd.ToString(), SVCConn);
                            int insertCount = cmd.ExecuteNonQuery();
                            count = count + insertCount;
                            insertCmd.Clear();
                            insertCmd.Append(@"insert  into D_MDM_INDENT_PREPARE values (");
                        }
                    }
                    if (count != fipList.Count || count == 0)
                    {
                        PVtrans.Rollback();
                        SCVtrans.Rollback();
                        if (count != 0)
                            Log.LoadInfo("Rollback");
                    }
                    else
                    {
                        SCVtrans.Commit();
                        PVtrans.Commit();
                        Log.LoadInfo("Commit");
                        foreach (var item in fipList)
                        {
                            Log.LoadInfo("Item END_CODE:" + item.END_CODE + " INDENT_CODE:" + item.INDENT_CODE + " INDENT_TYPE:" + item.INDENT_TYPE + " NET_CODE:" + item.NET_CODE + " PART_NUM:" + item.PART_NUM + " PREPARE_CODE:" + item.PREPARE_CODE + " OUT_CKCODE:" + item.OUT_CKCODE + " PARTS_CODE:" + item.PARTS_CODE + " PARTS_NAME:" + item.PARTS_NAME + " PREPARE_TIME:" + item.PREPARE_TIME + " PVAUDITING_TIME:" + item.PVAUDITING_TIME + " STORAGE_PLACE:" + item.STORAGE_PLACE + " YS_FS:" + item.YS_FS + " excuted");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (PVtrans != null)
                    PVtrans.Rollback();
                if (SCVtrans != null)
                    SCVtrans.Rollback();
                Log.LoadInfo("Expection! Rollback");
                Log.LoadInfo("GetAndInsertFIPData Error, " + ex);
                Console.WriteLine("GetAndInsertFIPData Error, " + ex.Message);
            }
            finally
            {
                if (PVConn != null)
                    PVConn.Dispose();
                if (SVCConn != null)
                    SVCConn.Dispose();
                Log.LoadInfo("GetAndInsertFIPData Function End");
            }
        }
    }
}
