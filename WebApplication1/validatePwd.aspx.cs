using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
namespace WebApplication1
{
    public partial class validatePwd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UMsgDefine.FM_ChgPwd_Req fmChgReq;
            UMsgDefine.FM_ChgPwd_Rsp fmChgRsp;
            string oldpwd = Request.QueryString["oldpwd"];
            string newpwd = Request.QueryString["newpwd"];
            fmChgReq.MsgHead.MsgType = 16842757;
            fmChgReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_ChgPwd_Req));
            fmChgReq.UserID = int.Parse(Session["UserID"].ToString());
            fmChgReq.OldPwd = oldpwd.PadRight(20, '\0').ToCharArray();
            fmChgReq.NewPwd = newpwd.PadRight(20, '\0').ToCharArray();
            fmChgReq.ServeFor = "".PadRight(64, '\0').ToCharArray();
            fmChgReq.TrueName = "".PadRight(24, '\0').ToCharArray();

            byte[] ChgMessage = UMsgDefine.StructToBytes(fmChgReq);
            ((Socket)Session["SocketTCP"]).Send(ChgMessage, fmChgReq.MsgHead.MsgLength, SocketFlags.None);

            byte[] RcvChgMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_ChgPwd_Rsp))];
            ((Socket)Session["SocketTCP"]).Receive(RcvChgMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_ChgPwd_Rsp)), SocketFlags.None);

            fmChgRsp = (UMsgDefine.FM_ChgPwd_Rsp)UMsgDefine.BytesToStruct(RcvChgMessage, typeof(UMsgDefine.FM_ChgPwd_Rsp));

            this.Response.Write(fmChgRsp.RspCode.ToString());
        }
    }
}