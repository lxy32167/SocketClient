using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public UMsgDefine.FM_GetPaper_Req fmGetPaperReq;
        public UMsgDefine.FM_GetPaper_Rsp fmGetPaperRsp;
     
        public UMsgDefine.FM_Beacon_Client BeaconReq;
        public UMsgDefine.FM_Beacon_Server BeaconRsp;

        public byte[] BeaconMessage, RcvBeaconMessage;

        public UMsgDefine.FM_GETHISTORYPAPER_RSP GetHistoryPaperRsp;

        public UMsgDefine.stHistoryPaper[] HistoryPaper;

        public bool beChecked = false;

        private const int _firstEditCellIndex = 2;

        public string[] Score,SaveScore,FullScore;
        protected void Page_Load(object sender, EventArgs e)
        {
            Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            if (Session["CourseID"] == null || Session["BlkNo"] == null || Session["ClassID"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            LockUserData();
           
            if (!this.IsPostBack)
            {            
                Button1.Attributes["onclick"] = "Javascript:return window.confirm('确定返回登陆界面?')";
         //       Button5.Attributes.Add("onclick", "return checkScore();");
                ///启动信标线程
                RcvBeaconMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_Beacon_Server))];
                Thread BeaconThread = new Thread(new ThreadStart(TBeaconThread));
                BeaconThread.Start();
                Session["BeaconThread"] = BeaconThread;

                ///取用户历史阅卷数据
                GetHistoryPaper(GetHistoryPaperRsp);

                GetBlockInfo();
                if (Session["beChecked"] == null || (bool)Session["beChecked"] == false)
                {
                    GetPaperInfo();
                }
                else if ((bool)Session["beChecked"] == true)
                {
                    beChecked = (bool)Session["beChecked"];
                    AfterGivingScore(beChecked);
                }
                //这部分是显示的Score
                Score = new string[2];
                Session["Score"] = Score;
                //这部分是用于保存的Score
                SaveScore = new string[2];
                Session["SaveScore"] = SaveScore;
                Session["beChecked"] = beChecked;
                //满分
                FullScore = new string[2];
                Session["FullScore"] = FullScore;
                Label4.Text = "阅卷教师:" + new String(((UMsgDefine.FM_Auth_Rsp)Session["fmLogRsp"]).UserInfo.TrueName);
            }

            //加载存分数据和详查数据
            this.GridView1.Attributes.Add("SortExpression", "试卷号");
            this.GridView1.Attributes.Add("SortDirection", "ASC");
            LoadGridViewCheckData();

            //存分GridView的单击编辑响应
            if (this.GridView2.SelectedIndex > -1)
            {
                // 调用GridView的UpdateRow方法
                this.GridView2.UpdateRow(this.GridView2.SelectedIndex, false);
            }

            if (getPostBackControlName() == "Button5" || getPostBackControlName() == "LinkButton1")
            {
                for (int i = 0; i < 2; i++)
                {
                    if (getPostBackControlName() == "Button5")
                    {
                        ((string[])Session["SaveScore"])[i] = string.Copy(((string[])Session["Score"])[i]);
                    }
                    ((string[])Session["Score"])[i] = null;
                }
            }
            LoadGridViewScoreData();
        
        }
        /// <summary>
        /// 给锁定窗口的解锁用户名和密码赋值
        /// </summary>
        protected void LockUserData()
        {
            string username, userpwd;
            username = new String(((UMsgDefine.FM_Auth_Req)Session["fmLogReq"]).UserName).TrimEnd('\0');
            userpwd = new String(((UMsgDefine.FM_Auth_Req)Session["fmLogReq"]).UserPwd).TrimEnd('\0');
            hidden.Value = username;
            hidden1.Value = userpwd;
        }
        /// <summary>
        /// 取题块信息
        /// </summary>
        protected void GetBlockInfo()
        {
            UMsgDefine.FM_GetBlkInfo_Req fmGetBlkReq;
            UMsgDefine.FM_GetBlkInfo_Rsp fmGetBlkRsp;

            //RcvBlk
            fmGetBlkReq.MsgHead.MsgType = 16842765;
            fmGetBlkReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GetBlkInfo_Req));
            fmGetBlkReq.CourseID = int.Parse(Session["CourseID"].ToString());
            fmGetBlkReq.BlkID = int.Parse(Session["BlkNo"].ToString());
            fmGetBlkReq.ClassID = Session["ClassID"].ToString().PadRight(16, '\0').ToCharArray();
            fmGetBlkReq.UserName = new String(((UMsgDefine.FM_Auth_Rsp)Session["fmLogRsp"]).UserInfo.LoginName).PadRight(24, '\0').ToCharArray();

            byte[] BlkMessage = UMsgDefine.StructToBytes(fmGetBlkReq);
            ((Socket)Session["SocketTCP"]).Send(BlkMessage, fmGetBlkReq.MsgHead.MsgLength, SocketFlags.None);

            byte[] RcvBlkMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetBlkInfo_Rsp))];
            ((Socket)Session["SocketTCP"]).Receive(RcvBlkMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GetBlkInfo_Rsp)), SocketFlags.None);

            fmGetBlkRsp = (UMsgDefine.FM_GetBlkInfo_Rsp)UMsgDefine.BytesToStruct(RcvBlkMessage, typeof(UMsgDefine.FM_GetBlkInfo_Rsp));
            ///保存CheckRule待解析
            Session["CheckRule"] = fmGetBlkRsp.BlockInfo.CheckRule;
        }
        /// <summary>
        /// 注销用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {           
            UMsgDefine.FM_UserLogout fmLogout;
            fmLogout.UserName = (char[])(((UMsgDefine.FM_Auth_Req)Session["fmLogReq"]).UserName).Clone();
            fmLogout.MsgHead.MsgType = 16842796;
            fmLogout.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_UserLogout));
            byte[] LogoutMessage = UMsgDefine.StructToBytes(fmLogout);
            System.IO.Directory.Delete(Server.MapPath(".") + @"/PicTemp" + Session["UserID"].ToString(), true);
            ((Socket)Session["SocketTCP"]).Send(LogoutMessage, fmLogout.MsgHead.MsgLength, SocketFlags.None);
            ((Socket)Session["SocketTCP"]).Close();
            ((Thread)Session["BeaconThread"]).Abort();
            ((Socket)Session["SocketUDP"]).Close();
            Response.Redirect("~/Login.aspx");
        }       
        /// <summary>
        /// 登录时自动获取试卷数据
        /// </summary>
        protected void GetPaperInfo()
        {
           
            //RcvPaper
            ///
            fmGetPaperReq.MsgHead.MsgType = 16842771;
            fmGetPaperReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Req));
            fmGetPaperReq.GetPaperTask.UserID = int.Parse(Session["UserID"].ToString());
            fmGetPaperReq.GetPaperTask.CourseID = int.Parse(Session["CourseID"].ToString());
            fmGetPaperReq.GetPaperTask.ClassID = Session["ClassID"].ToString().PadRight(16, '\0').ToCharArray();
            fmGetPaperReq.GetPaperTask.BlkNo = int.Parse(Session["BlkNo"].ToString());
            fmGetPaperReq.GetPaperTask.PaperNo = 0;
            fmGetPaperReq.GetPaperTask.PicNo = 2;
            fmGetPaperReq.CheckFlag = 0;


            byte[] PaperMessage = UMsgDefine.StructToBytes(fmGetPaperReq);
            ((Socket)Session["SocketTCP"]).Send(PaperMessage, fmGetPaperReq.MsgHead.MsgLength, SocketFlags.None);

            byte[] RcvPaperMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp))];
            ((Socket)Session["SocketTCP"]).Receive(RcvPaperMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp)), SocketFlags.None);

            fmGetPaperRsp = (UMsgDefine.FM_GetPaper_Rsp)UMsgDefine.BytesToStruct(RcvPaperMessage, typeof(UMsgDefine.FM_GetPaper_Rsp));

            if(fmGetPaperRsp.RspCode == 16908303)
            {
                //logout
                UMsgDefine.FM_UserLogout fmLogout;
                fmLogout.UserName = (char[])(((UMsgDefine.FM_Auth_Req)Session["fmLogReq"]).UserName).Clone();
                fmLogout.MsgHead.MsgType = 16842796;
                fmLogout.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_UserLogout));
                byte[] LogoutMessage = UMsgDefine.StructToBytes(fmLogout);
                
                if(System.IO.Directory.Exists(Server.MapPath(".") + @"/PicTemp" + Session["UserID"].ToString()) == true)
                {
                    System.IO.Directory.Delete(Server.MapPath(".") + @"/PicTemp" + Session["UserID"].ToString(), true);
                }
                ((Socket)Session["SocketTCP"]).Send(LogoutMessage, fmLogout.MsgHead.MsgLength, SocketFlags.None);
                ((Socket)Session["SocketTCP"]).Close();
                ((Thread)Session["BeaconThread"]).Abort();
                ((Socket)Session["SocketUDP"]).Close();              
                //login     
                     
                lblMsg.InnerHtml = "<script type='text/javascript'>alert('试卷已阅完!');location.href='Login.aspx'</script>";
                return;
            }
            SaveToFile(fmGetPaperRsp);
            
            Image1.ImageUrl = "~/PicTemp" + Session["UserID"].ToString() + "/" + fmGetPaperRsp.PaperData.PaperNo.ToString() +".jpg";
           
            Label3.Text = "学生姓名:" + new string(fmGetPaperRsp.PaperData.StudentName);
            Label5.Text = "试卷号:" + (fmGetPaperRsp.PaperData.PaperNo).ToString();
            
            Session["fmGetPaperReq"] = fmGetPaperReq;
            Session["fmGetPaperRsp"] = fmGetPaperRsp;
        }
        /// <summary>
        /// 保存图片信息
        /// </summary>
        /// <param name="fmGetPaperRsp"></param>
        protected void SaveToFile(UMsgDefine.FM_GetPaper_Rsp fmGetPaperRsp)
        {
            //SaveToFile 
            byte[] RcvMS = new byte[4096];
            int len = 0;

            FileStream fs = new FileStream(Server.MapPath(".") + @"/PicTemp" + Session["UserID"].ToString() + @"/" + fmGetPaperRsp.PaperData.PaperNo.ToString() + @".jpg", FileMode.OpenOrCreate);
            BufferedStream bs = new BufferedStream(fs);
            using (fs)
            {
                using (bs)
                {
                    while (len < fmGetPaperRsp.PaperData.ImageLen)
                    {
                        int count = ((Socket)Session["SocketTCP"]).Receive(RcvMS, RcvMS.Length, SocketFlags.None);
                        bs.Write(RcvMS, 0, count);
                        len += count;
                    }
                    bs.Flush();
                    fs.Flush();
                    bs.Close();
                    fs.Close();
                }
            }
        }
        /// <summary>
        /// 给分之后的处理,取下一张卷,回到上次阅卷
        /// </summary>
        /// <param name="beChecked"></param>
        protected void AfterGivingScore(bool beChecked)
        {
            if (beChecked == false)
            {
                fmGetPaperReq = (UMsgDefine.FM_GetPaper_Req)Session["fmGetPaperReq"];

                byte[] PaperMessage = UMsgDefine.StructToBytes(fmGetPaperReq);
                ((Socket)Session["SocketTCP"]).Send(PaperMessage, fmGetPaperReq.MsgHead.MsgLength, SocketFlags.None);

                byte[] RcvPaperMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp))];
                ((Socket)Session["SocketTCP"]).Receive(RcvPaperMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp)), SocketFlags.None);

                fmGetPaperRsp = (UMsgDefine.FM_GetPaper_Rsp)UMsgDefine.BytesToStruct(RcvPaperMessage, typeof(UMsgDefine.FM_GetPaper_Rsp));

                //SaveToFile 
                SaveToFile(fmGetPaperRsp);

                Image1.ImageUrl = "~/PicTemp" + Session["UserID"].ToString() + "/" + fmGetPaperRsp.PaperData.PaperNo.ToString() + ".jpg";

                Session["fmGetPaperRsp"] = fmGetPaperRsp;
            }
            else
            {
                fmGetPaperReq = (UMsgDefine.FM_GetPaper_Req)Session["fmGetPaperReq"];
                
                fmGetPaperReq.GetPaperTask.PaperNo = (int)Session["OldPaperNo"];

                byte[] PaperMessage = UMsgDefine.StructToBytes(fmGetPaperReq);
                ((Socket)Session["SocketTCP"]).Send(PaperMessage, fmGetPaperReq.MsgHead.MsgLength, SocketFlags.None);

                byte[] RcvPaperMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp))];
                ((Socket)Session["SocketTCP"]).Receive(RcvPaperMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp)), SocketFlags.None);

                fmGetPaperRsp = (UMsgDefine.FM_GetPaper_Rsp)UMsgDefine.BytesToStruct(RcvPaperMessage, typeof(UMsgDefine.FM_GetPaper_Rsp));

                //SaveToFile 
                SaveToFile(fmGetPaperRsp);

                Image1.ImageUrl = "~/PicTemp" + Session["UserID"].ToString() + "/" + fmGetPaperRsp.PaperData.PaperNo.ToString() + ".jpg";

                Session["fmGetPaperRsp"] = fmGetPaperRsp;

                beChecked = false;

                Session["beChecked"] = beChecked;
            }
        }

        /// <summary>
        /// 存分
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button5_Click(object sender, EventArgs e)
        {
            UMsgDefine.FM_SaveScore_Req fmSaveReq;
            UMsgDefine.FM_SaveScore_Rsp fmSaveRsp;
            UMsgDefine.FM_SaveMarkInfo_Req fmSaveMarkInfoReq;
            UMsgDefine.FM_SaveMarkInfo_Rsp fmSaveMarkInfoRsp;

            int i = 0;
            byte[] SaveMarkMessage, RcvSaveMarkMessage;
            fmSaveReq.Score = new UMsgDefine.stSaveScore[5];
            fmSaveReq.MsgHead.MsgType = 16842788;
            fmSaveReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Req));
            fmSaveReq.RecCount = 1;

            fmSaveRsp.RspCode = 0;
           
            for (i = 0; i < fmSaveReq.RecCount; i++)
            {
                fmSaveReq.Score[i].CourseID = int.Parse(Session["CourseID"].ToString());
                fmSaveReq.Score[i].UserID = (int)Session["UserID"];
                fmSaveReq.Score[i].PaperNo = ((UMsgDefine.FM_GetPaper_Rsp)Session["fmGetPaperRsp"]).PaperData.PaperNo;
                fmSaveReq.Score[i].BLKNO = int.Parse(Session["BlkNo"].ToString());

                fmSaveReq.Score[i].TotalScore = (float)(Convert.ToDouble(((string[])Session["SaveScore"])[0]) + Convert.ToDouble(((string[])Session["SaveScore"])[1]));//75.0F;
                fmSaveReq.Score[i].DetailScore = (((string[])Session["SaveScore"])[0] + "," + ((string[])Session["SaveScore"])[1]).PadRight(64, '\0').ToCharArray(); //"25,30".PadRight(64, '\0').ToCharArray();

                fmSaveReq.Score[i].TimeStamp = (uint)((((DateTime.Now).AddDays(UnitGlobalV.sysTimeDis) - UnitGlobalV.javaTime).TotalDays * 86400) - 8 * 60 * 60); //1366389082 - 8 * 60 * 60;
                fmSaveReq.Score[i].isExample = 0;
                fmSaveReq.Score[i].Check = 0;
                //       fmSaveReq.Score[0].MarkInfo = "";
            }

            while (fmSaveRsp.RspCode != 16908289 && i < 3)
            {
                byte[] SaveMessage = UMsgDefine.StructToBytes(fmSaveReq);
                ((Socket)Session["SocketTCP"]).Send(SaveMessage, fmSaveReq.MsgHead.MsgLength, SocketFlags.None);

                byte[] RcvSaveMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Rsp))];
                ((Socket)Session["SocketTCP"]).Receive(RcvSaveMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_SaveScore_Rsp)), SocketFlags.None);
                fmSaveRsp = (UMsgDefine.FM_SaveScore_Rsp)UMsgDefine.BytesToStruct(RcvSaveMessage, typeof(UMsgDefine.FM_SaveScore_Rsp));
                i++;
            }

            fmSaveRsp.RspCode = 16908327;
            for (i = 0; i < fmSaveReq.RecCount; i++)
            {
                //fmSaveMarkInfoReq
                fmSaveMarkInfoReq.MsgHead.MsgType = 16842790;
                fmSaveMarkInfoReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_SaveMarkInfo_Req));
                fmSaveMarkInfoReq.MarkInfo.CourseID = fmSaveReq.Score[i].CourseID;
                fmSaveMarkInfoReq.MarkInfo.UserID = fmSaveReq.Score[i].UserID;
                fmSaveMarkInfoReq.MarkInfo.PaperNo = fmSaveReq.Score[i].PaperNo;
                fmSaveMarkInfoReq.MarkInfo.BLKNO = fmSaveReq.Score[i].BLKNO;
                fmSaveMarkInfoReq.MarkInfo.Check = fmSaveReq.Score[i].Check;
                fmSaveMarkInfoReq.MarkInfo.MarkInfoLen = 0; //fmSaveReq.Score[0].MarkInfo.Length;
                string tmpstr = "";// fmSaveReq.Score[0].MarkInfo;

                if (tmpstr.Length == 0)
                {

                    fmSaveMarkInfoReq.MarkInfo.MarkInfo = "".PadRight(8159, '\0').ToCharArray();
                    SaveMarkMessage = UMsgDefine.StructToBytes(fmSaveMarkInfoReq);
                    ((Socket)Session["SocketTCP"]).Send(SaveMarkMessage, fmSaveMarkInfoReq.MsgHead.MsgLength, SocketFlags.None);

                    RcvSaveMarkMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_SaveMarkInfo_Rsp))];
                    ((Socket)Session["SocketTCP"]).Receive(RcvSaveMarkMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_SaveMarkInfo_Rsp)), SocketFlags.None);
                    fmSaveMarkInfoRsp = (UMsgDefine.FM_SaveMarkInfo_Rsp)UMsgDefine.BytesToStruct(RcvSaveMarkMessage, typeof(UMsgDefine.FM_SaveMarkInfo_Rsp));
                    beChecked = (bool)Session["beChecked"];
                    AfterGivingScore(beChecked);
                    return;
                }

                while (tmpstr.Length > 8159)
                {
                    fmSaveMarkInfoReq.MarkInfo.MarkInfo = tmpstr.Substring(0, 8159).ToCharArray();
                    SaveMarkMessage = UMsgDefine.StructToBytes(fmSaveMarkInfoReq);
                    ((Socket)Session["SocketTCP"]).Send(SaveMarkMessage, fmSaveMarkInfoReq.MsgHead.MsgLength, SocketFlags.None);
                    tmpstr = tmpstr.Remove(0, 8159);

                    RcvSaveMarkMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_SaveMarkInfo_Rsp))];
                    ((Socket)Session["SocketTCP"]).Receive(RcvSaveMarkMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_SaveMarkInfo_Rsp)), SocketFlags.None);
                }

                fmSaveMarkInfoReq.MarkInfo.MarkInfo = tmpstr.ToCharArray();
                fmSaveMarkInfoReq.MsgHead.MsgLength = fmSaveMarkInfoReq.MsgHead.MsgLength - 8159 + tmpstr.Length;

                SaveMarkMessage = UMsgDefine.StructToBytes(fmSaveMarkInfoReq);
                ((Socket)Session["SocketTCP"]).Send(SaveMarkMessage, fmSaveMarkInfoReq.MsgHead.MsgLength, SocketFlags.None);

                RcvSaveMarkMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_SaveMarkInfo_Rsp))];
                ((Socket)Session["SocketTCP"]).Receive(RcvSaveMarkMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_SaveMarkInfo_Rsp)), SocketFlags.None);

            }
            beChecked = (bool)Session["beChecked"];
            AfterGivingScore(beChecked);
            for (i = 0; i < 2; i++)
            {
                ((string[])Session["SaveScore"])[i] = null;
            }
        }
        /// <summary>
        /// 信标线程
        /// </summary>
        public void TBeaconThread()
        {
            double v = 0, v1 = 0;
            while (true)
            {
                BeaconReq.MsgHead.MsgType = 16842755;
                BeaconReq.MsgHead.MsgLength = Marshal.SizeOf(BeaconReq);
                BeaconReq.UserID = (int)Session["UserID"];
                BeaconRsp.MsgHead.MsgType = 0;
                for (int i = 0; i < 2; i++)
                {
                    BeaconMessage = UMsgDefine.StructToBytes(BeaconReq);
                    ((Socket)Session["SocketUDP"]).Send(BeaconMessage, BeaconReq.MsgHead.MsgLength, SocketFlags.None);

                    ((Socket)Session["SocketUDP"]).Receive(RcvBeaconMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_Beacon_Server)), SocketFlags.None);
                    BeaconRsp = (UMsgDefine.FM_Beacon_Server)UMsgDefine.BytesToStruct(RcvBeaconMessage, typeof(UMsgDefine.FM_Beacon_Server));

                    if (BeaconRsp.MsgHead.MsgType == 16842756)
                    {
                        if (Math.Abs(v - (BeaconRsp.ServerTime + 8 * 60 * 60)) > 1)
                        {
                            v = UnitGlobalV.ts.TotalDays + (double)(BeaconRsp.ServerTime + 8 * 60 * 60) / 86400;
                            v1 = (DateTime.Now - UnitGlobalV.delphiTime).TotalDays;
                            UnitGlobalV.sysTimeDis = v - v1;
                            FileStream ds = File.Open(@"d:\1.txt",FileMode.Append,FileAccess.Write,FileShare.None);
                            StreamWriter sw = new StreamWriter(ds);
                            sw.WriteLine("{0}\t{1}", UnitGlobalV.sysTimeDis,DateTime.Now);
                            sw.Dispose();
                            sw.Close();
                            ds.Dispose();
                            ds.Close();
                        }
                        break;
                    }
                }
                Thread.Sleep(UnitGlobalV.Beacontime);
            }
        }
        /// <summary>
        /// 取历史卷
        /// </summary>
        /// <param name="GetHistoryPaperRsp"></param>
        public void GetHistoryPaper(UMsgDefine.FM_GETHISTORYPAPER_RSP GetHistoryPaperRsp)
        {
            UMsgDefine.FM_GETHISTORYPAPER_REQ GetHistoryPaperReq;
            int  l,i, CurDataSize = 1;
            byte[] RcvMS = new byte[10000];
            System.IO.MemoryStream RcvStream = new MemoryStream();
     
            GetHistoryPaperReq.MsgHead.MsgType = 16842773;
            GetHistoryPaperReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GETHISTORYPAPER_REQ));
            GetHistoryPaperReq.UserID = int.Parse(Session["UserID"].ToString());
            GetHistoryPaperReq.CourseID = int.Parse(Session["CourseID"].ToString());
            GetHistoryPaperReq.ClassID = Session["ClassID"].ToString().PadRight(16, '\0').ToCharArray();
            GetHistoryPaperReq.BlkNo = int.Parse(Session["BlkNo"].ToString());

            byte[] HistoryMessage = UMsgDefine.StructToBytes(GetHistoryPaperReq);
            ((Socket)Session["SocketTCP"]).Send(HistoryMessage, GetHistoryPaperReq.MsgHead.MsgLength, SocketFlags.None);

            byte[] RcvHistoryMessage = new byte[Marshal.SizeOf(GetHistoryPaperRsp)];
         //   while (true)
            {
                l = ((Socket)Session["SocketTCP"]).Receive(RcvHistoryMessage, Marshal.SizeOf(GetHistoryPaperRsp), SocketFlags.None);
                GetHistoryPaperRsp = (UMsgDefine.FM_GETHISTORYPAPER_RSP)UMsgDefine.BytesToStruct(RcvHistoryMessage,typeof(UMsgDefine.FM_GETHISTORYPAPER_RSP));
               
                HistoryPaper = new UMsgDefine.stHistoryPaper[GetHistoryPaperRsp.Count];
              
                if(GetHistoryPaperRsp.MsgHead.MsgType == 16842774)
                {
                    if(GetHistoryPaperRsp.RspCode == 16908289)
                    {
                        while(true)
                        {
                            CurDataSize = ((Socket)Session["SocketTCP"]).Receive(RcvMS, RcvMS.Length, SocketFlags.None);
                          
                            RcvStream.Write(RcvMS, 0, CurDataSize);
                            if(CurDataSize == GetHistoryPaperRsp.Count * Marshal.SizeOf(typeof(UMsgDefine.stHistoryPaper)))
                                break;  
                        }                       
                    }
                }
            }
            byte[] temp = new byte[Marshal.SizeOf(typeof(UMsgDefine.stHistoryPaper))];
            RcvStream.Seek(0, SeekOrigin.Begin);
            for(i = 0; i< GetHistoryPaperRsp.Count;i++)
            {
                RcvStream.Read(temp,0,Marshal.SizeOf(typeof(UMsgDefine.stHistoryPaper)));
                HistoryPaper[i] = (UMsgDefine.stHistoryPaper)UMsgDefine.BytesToStruct(temp,typeof(UMsgDefine.stHistoryPaper));
            }
            Session["HistoryPaper"] = HistoryPaper;
   
            RcvStream.Dispose();
        }
        /// <summary>
        /// 选择历史卷查看
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            if ((bool)Session["beChecked"] == false)
            {
                Session["OldPaperNo"] = ((UMsgDefine.FM_GetPaper_Rsp)Session["fmGetPaperRsp"]).PaperData.PaperNo;
            }
            LinkButton b = (LinkButton)sender;
            GridViewRow row = (GridViewRow)b.Parent.Parent;
            //RcvPaper
            ///
            fmGetPaperReq.MsgHead.MsgType = 16842771;
            fmGetPaperReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Req));
            fmGetPaperReq.GetPaperTask.UserID = int.Parse(Session["UserID"].ToString());
            fmGetPaperReq.GetPaperTask.CourseID = int.Parse(Session["CourseID"].ToString());
            fmGetPaperReq.GetPaperTask.ClassID = Session["ClassID"].ToString().PadRight(16, '\0').ToCharArray();
            fmGetPaperReq.GetPaperTask.BlkNo = int.Parse(Session["BlkNo"].ToString());
            fmGetPaperReq.GetPaperTask.PaperNo = Convert.ToInt32((row.FindControl("Label1") as Label).Text);
            fmGetPaperReq.GetPaperTask.PicNo = 2;
            fmGetPaperReq.CheckFlag = 1;

            byte[] PaperMessage = UMsgDefine.StructToBytes(fmGetPaperReq);
            ((Socket)Session["SocketTCP"]).Send(PaperMessage, fmGetPaperReq.MsgHead.MsgLength, SocketFlags.None);

            byte[] RcvPaperMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp))];
            ((Socket)Session["SocketTCP"]).Receive(RcvPaperMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_GetPaper_Rsp)), SocketFlags.None);

            fmGetPaperRsp = (UMsgDefine.FM_GetPaper_Rsp)UMsgDefine.BytesToStruct(RcvPaperMessage, typeof(UMsgDefine.FM_GetPaper_Rsp));

            //SaveToFile 
            SaveToFile(fmGetPaperRsp);

            Image1.ImageUrl = "~/PicTemp" + Session["UserID"].ToString() + "/" + fmGetPaperRsp.PaperData.PaperNo.ToString() + ".jpg";

            beChecked = true;

            Label3.Text = "学生姓名:" + new string(fmGetPaperRsp.PaperData.StudentName);
            Label5.Text = "试卷号:" + (fmGetPaperRsp.PaperData.PaperNo).ToString();

            Session["beChecked"] = beChecked;
            Session["fmGetPaperRsp"] = fmGetPaperRsp;
        }
        /// <summary>
        /// 填写详查表单
        /// </summary>
        void LoadGridViewCheckData()
        {
            HistoryPaper = new UMsgDefine.stHistoryPaper[((UMsgDefine.stHistoryPaper[])Session["HistoryPaper"]).Length];
            for (int i = 0; i < ((UMsgDefine.stHistoryPaper[])Session["HistoryPaper"]).Length; i++)
            {
                HistoryPaper[i] = ((UMsgDefine.stHistoryPaper[])Session["HistoryPaper"])[i];
            }

            string sortExpression = this.GridView1.Attributes["SortExpression"];
            string sortDirection = this.GridView1.Attributes["SortDirection"];

            DataTable dt = CreateSampleCheckData(HistoryPaper);
 //           FillBlankRow(dt, GridView1.PageSize);

            // 根据GridView排序数据列及排序方向设置显示的默认数据视图
            if ((!string.IsNullOrEmpty(sortExpression)) && (!string.IsNullOrEmpty(sortDirection)))
            {
                dt.DefaultView.Sort = string.Format("{0} {1}", sortExpression, sortDirection);
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        
        static DataTable CreateSampleCheckData(UMsgDefine.stHistoryPaper[] HistoryPaper)
        {

            DataTable tbl = new DataTable("Mark");

            tbl.Columns.Add("试卷号", typeof(int));
            tbl.Columns.Add("学号", typeof(string));
            tbl.Columns.Add("分数", typeof(float));
            tbl.Columns.Add("详细分数", typeof(string));

            for (int i = 0; i < HistoryPaper.Length; i++)
            {
                tbl.Rows.Add(HistoryPaper[i].PaperNo, new String(HistoryPaper[i].StudentID), HistoryPaper[i].Score, new String(HistoryPaper[i].DetailScore));
            }

            return tbl;
        }
        /// <summary>
        /// 填写存分表单
        /// </summary>
        void LoadGridViewScoreData()
        {
            char[] CheckRule = (char[])Session["CheckRule"];
            Score = (string[])Session["Score"];
            FullScore = (string[])Session["FullScore"];
            DataTable dt = CreateScoreData(CheckRule,Score,FullScore);
            Session["FullScore"] = FullScore;
            GridView2.DataSource = dt;
            GridView2.DataBind();
        }
        static DataTable CreateScoreData(char[] CheckRule,string[] Score,string[] FullScore)
        {
            string[] DecodeData = new String[2 * FindChar(new string(CheckRule).TrimEnd('\0') + "")];

          
            
            DataTable tbl = new DataTable("Score");
            DecodeData = DecodeInfo(CheckRule, DecodeData, FullScore);
          
            tbl.Columns.Add("步骤", typeof(string));
            tbl.Columns.Add("分数", typeof(string));
            tbl.Columns.Add("满分", typeof(string));
            
            for (int i = 0; i < DecodeData.Length / 2; i++)
            {
                tbl.Rows.Add(DecodeData[2 * i],Score[i],DecodeData[2 * i + 1]);
            }
            
           
            return tbl;
            // for(int i = 0;i < 
        }
        /// <summary>
        /// 解析规则字符串
        /// </summary>
        /// <param name="CheckRule"></param>
        /// <param name="DecodeData"></param>
        /// <param name="FullScore"></param>
        /// <returns></returns>
        static string[] DecodeInfo(char[] CheckRule, string[] DecodeData,string[] FullScore)
        {
            string str, Points;

            str = new string(CheckRule).TrimEnd('\0') + "";

            str = str.Substring(str.IndexOf('('));

            for (int i = 0; i < DecodeData.Length / 2; i++)
            {
                Points = str.Substring(str.IndexOf('(') + 1, str.IndexOf(')') - 1);

                DecodeData[2 * i] = String.Copy(Points.Substring(0, Points.IndexOf(',')));
                DecodeData[2 * i + 1] = String.Copy(Points.Substring(Points.IndexOf(',') + 1));
                FullScore[i] = DecodeData[2 * i + 1];
                str = str.Substring(str.IndexOf(')') + 2);
            }
            
            return DecodeData;
        }
        static int FindChar(string str)
        {
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 存分表单的单击选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // 从第一个单元格内获得LinkButton控件
                LinkButton _singleClickButton = (LinkButton)e.Row.Cells[0].Controls[0];
                // 返回一个字符串，表示对包含目标控件的 ID 和事件参数的回发函数的 JavaScript 调用
                string _jsSingle = ClientScript.GetPostBackClientHyperlink(_singleClickButton, "");

                // 给每一个可编辑的单元格增加事件
                for (int columnIndex = _firstEditCellIndex; columnIndex < e.Row.Cells.Count - 1; columnIndex++)
                {
                    // 增加列索引作为事件参数
                    string js = _jsSingle.Insert(_jsSingle.Length - 2, columnIndex.ToString());
                    // 给单元格增加onclick事件
                    e.Row.Cells[columnIndex].Attributes["onclick"] = js;
                    // 给单元格增加鼠标经过时指针样式
                    e.Row.Cells[columnIndex].Attributes["style"] += "cursor:pointer;cursor:hand;";
                }
            }
            
        }

        protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridView _gridView = (GridView)sender;

            switch (e.CommandName)
            {
                case ("SingleClick"):
                    // 获得行索引
                    int _rowIndex = int.Parse(e.CommandArgument.ToString());
                    // 解析事件参数（在RowDataBound中增加的），从而获得被选中的列的索引
                    int _columnIndex = int.Parse(Request.Form["__EVENTARGUMENT"]);

                    // 设置GridView被选中的行的索引（每次回发后判断GridView1.SelectedIndex > -1则更新）
                    _gridView.SelectedIndex = _rowIndex;
                    // 绑定
                    _gridView.DataBind();

                   
                    // 获得被选中单元格的显示控件并设置其不可见
                    Control _displayControl = _gridView.Rows[_rowIndex].Cells[_columnIndex].Controls[1];
                    _displayControl.Visible = false;
                    // 获得被选中单元格的编辑控件并设置其可见
                    Control _editControl = _gridView.Rows[_rowIndex].Cells[_columnIndex].Controls[3];
                    _editControl.Visible = true;
                    // 清除被选中单元格属性以删除click事件
                    _gridView.Rows[_rowIndex].Cells[_columnIndex].Attributes.Clear();

                    // 设置焦点到被选中的编辑控件
                    ClientScript.RegisterStartupScript(GetType(), "SetFocus",
                        "<script>document.getElementById('" + _editControl.ClientID + "').focus();</script>");
                    // 如果编辑控件是DropDownList的话，那么把SelectedValue设置为显示控件的值
                    if (_editControl is DropDownList && _displayControl is Label)
                    {
                        ((DropDownList)_editControl).SelectedValue = ((Label)_displayControl).Text;
                    }
                    // 如果编辑控件是TextBox的话则选中文本框内文本
                    if (_editControl is TextBox)
                    {
                        ((TextBox)_editControl).Attributes.Add("onfocus", "this.select()");
                    }

                    break;
            }
        }
        /// <summary>
        /// 把值（values）从EditItemTemplate转移到NewValues集合里（使用数据源控件的话就需要这步）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView2_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridView _gridView = (GridView)sender;
            string key = "";
            string value = "";
            // NewValues集合里的key
            string[] _columnKeys = new string[] { "分数" };

            if (e.RowIndex > -1)
            {
                // 循环每一列
                for (int i = _firstEditCellIndex; i < _gridView.Columns.Count - 1; i++)
                {
                    // 获得单元格里的控件
                    Control _displayControl = _gridView.Rows[e.RowIndex].Cells[i].Controls[1];
                    Control _editControl = _gridView.Rows[e.RowIndex].Cells[i].Controls[3];
                    
                    // 获得列的key
                    key = _columnKeys[i - _firstEditCellIndex];

                    // 如果单元格处于编辑模式的话，那么从编辑控件中获取值
                    if (_editControl.Visible)
                    {
                        if (_editControl is TextBox)
                        {
                            value = ((TextBox)_editControl).Text;
                            
                            if (value.Length != 0 && Convert.ToInt32(value) > Convert.ToInt32(((string[])Session["FullScore"])[e.RowIndex]))
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "错误", "alert('所给分数超过满分!');", true);
                                return;
                            }
                            ((string[])Session["Score"])[e.RowIndex] = value; 


                        }
                        else if (_editControl is DropDownList)
                        {
                            value = ((DropDownList)_editControl).SelectedValue;
                        }

                        // 增加key/value对到NewValues集合
                        e.NewValues.Add(key, value);
                    }
                    // 否则从显示控件中获取值
                    else
                    {
                        value = ((Label)_displayControl).Text.ToString();

                        // 增加key/value对到NewValues集合
                        e.NewValues.Add(key, value);
                        
                    }
                   
                }
            }
        }

        // 注册动态创建的客户端脚本
        protected override void Render(HtmlTextWriter writer)
        {
           
            // 在RowDataBound中创建的自定义事件必须要在页中注册
            // 通过重写Render方法来调用ClientScriptManager.RegisterForEventValidation。
            // 通过GridViewRow.UniqueID返回行的唯一ID，按纽的唯一ID通过在行的唯一ID后附加“$ct100”而生成。
            foreach (GridViewRow r in GridView2.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    for (int columnIndex = _firstEditCellIndex; columnIndex < r.Cells.Count; columnIndex++)
                    {
                        Page.ClientScript.RegisterForEventValidation(r.UniqueID + "$ctl00", columnIndex.ToString());
                    }
                }
            }

            base.Render(writer);
        }
        private string getPostBackControlName()
        {
            Control control = null;
            string ctrlname = Page.Request.Params["__EVENTTARGET"];
            if (ctrlname != null && ctrlname != String.Empty)
            {
                control = Page.FindControl(ctrlname);
            }
            else
            {
                Control c;
                foreach (string ctl in Page.Request.Form)
                {
                     if (ctl.EndsWith(".x") || ctl.EndsWith(".y"))
                    {
                        c = Page.FindControl(ctl.Substring(0, ctl.Length - 2));
                    }
                    else
                    {
                        c = Page.FindControl(ctl);
                    }
                    if (c is System.Web.UI.WebControls.Button ||
                             c is System.Web.UI.WebControls.ImageButton)
                    {
                        control = c;
                        break;
                    }
                }
            }
            if (control != null)
                return control.ID;
            else
                return string.Empty;
        }
        /// <summary>
        /// 详查表单的切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            LoadGridViewCheckData();
        }

        public static void FillBlankRow(DataTable dt, int dataTableSize)
        {
            int additionCount = dt.Rows.Count;
            if (dataTableSize <0)
            throw new ArgumentOutOfRangeException("方法参数dataTableSize不能是负数。");
            while (additionCount > dataTableSize)
                additionCount = dt.Rows.Count - dataTableSize;
            if (additionCount < dataTableSize)
            {
                additionCount = dataTableSize - additionCount;
                for (int i =0; i < additionCount; i++)
                    {
                        DataRow row = dt.NewRow();
                        dt.Rows.Add(row);
                    }
            }
        }
        /// <summary>
        /// 详查表单的排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            // 从事件参数获取排序数据列
            string sortExpression = e.SortExpression.ToString();

            // 假定为排序方向为“顺序”
            string sortDirection = "ASC";

            // “ASC”与事件参数获取到的排序方向进行比较，进行GridView排序方向参数的修改
            if (sortExpression == this.GridView1.Attributes["SortExpression"])
            {
                //获得下一次的排序状态
                sortDirection = (this.GridView1.Attributes["SortDirection"].ToString() == sortDirection ? "DESC" : "ASC");
            }

            // 重新设定GridView排序数据列及排序方向
            this.GridView1.Attributes["SortExpression"] = sortExpression;
            this.GridView1.Attributes["SortDirection"] = sortDirection;

            LoadGridViewCheckData();
        }
        /// <summary>
        /// 系统菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lButtonCompany_Click(object sender, EventArgs e)
        {
            mvCompany.ActiveViewIndex = 0;
            Cell1.Attributes["class"] = "SelectedTopBorder";
            Cell2.Attributes["class"] = "TopBorder";
            Cell3.Attributes["class"] = "TopBorder";
        }
        protected void lButtonProduct_Click(object sender, EventArgs e)
        {
            mvCompany.ActiveViewIndex = 1;
            Cell1.Attributes["class"] = "TopBorder";
            Cell2.Attributes["class"] = "SelectedTopBorder";
            Cell3.Attributes["class"] = "TopBorder";
        }
        protected void lButtonContact_Click(object sender, EventArgs e)
        {
            mvCompany.ActiveViewIndex = 2;
            Cell1.Attributes["class"] = "TopBorder";
            Cell2.Attributes["class"] = "TopBorder";
            Cell3.Attributes["class"] = "SelectedTopBorder";
        }
        /// <summary>
        /// 重选题块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button7_Click(object sender, EventArgs e)        
        {
            UMsgDefine.FM_Auth_Req fmLogReq;
            UMsgDefine.FM_Auth_Rsp fmLogRsp;
            UMsgDefine.FM_UserLogout fmLogout;
            double v, v1, sysTimeDis;
            Socket clientSocketTCP, clientSocketUDP;
            IPAddress ip = IPAddress.Parse("192.168.244.128");
            fmLogout.UserName = (char[])(((UMsgDefine.FM_Auth_Req)Session["fmLogReq"]).UserName).Clone();
            fmLogout.MsgHead.MsgType = 16842796;
            fmLogout.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_UserLogout));
            byte[] LogoutMessage = UMsgDefine.StructToBytes(fmLogout);

            ((Socket)Session["SocketTCP"]).Send(LogoutMessage, fmLogout.MsgHead.MsgLength, SocketFlags.None);
            ((Socket)Session["SocketTCP"]).Close();
            ((Thread)Session["BeaconThread"]).Abort();
            ((Socket)Session["SocketUDP"]).Close();
            Thread.Sleep(3000);


            clientSocketTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocketTCP.Connect(new IPEndPoint(ip, 27520)); //配置服务器IP与端口  

            clientSocketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            clientSocketUDP.Connect(new IPEndPoint(ip, 27510)); //配置服务器IP与端口  
            fmLogReq = (UMsgDefine.FM_Auth_Req)Session["fmLogReq"];
            byte[] Message = UMsgDefine.StructToBytes(fmLogReq);
            clientSocketTCP.Send(Message, fmLogReq.MsgHead.MsgLength, SocketFlags.None);
            byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_Auth_Rsp))];
            clientSocketTCP.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_Auth_Rsp)), SocketFlags.None);

            fmLogRsp = (UMsgDefine.FM_Auth_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_Auth_Rsp));

            if (fmLogRsp.MsgHead.MsgType == 16842754 && fmLogRsp.Rspcode == 16908289)
            {
                v = UnitGlobalV.ts.TotalDays + (double)(fmLogRsp.ServerTime + 8 * 60 * 60) / 86400;
                v1 = (DateTime.Now - UnitGlobalV.delphiTime).TotalDays;
                sysTimeDis = Math.Round(v - v1, 6);

                Session["UserID"] = fmLogRsp.UserInfo.UserID;
                Session["fmLogReq"] = fmLogReq;
                Session["fmLogRsp"] = fmLogRsp;
                Session["sysTimeDis"] = sysTimeDis;
                Session["SocketTCP"] = clientSocketTCP;
                Session["SocketUDP"] = clientSocketUDP;
            }
            Response.Redirect("~/Blk.aspx");
        }

        protected void GridView2_PreRender(object sender, EventArgs e)
        {
  //          ClientScriptManager cs = Page.ClientScript;
 //           foreach(GridViewRow gvrow in GridView2)
  //          {
   //             L
        }

    }
}

