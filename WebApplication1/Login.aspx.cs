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

    public partial class _Default : System.Web.UI.Page
    {
        static IPAddress ip = IPAddress.Parse("192.168.93.128");
        public UMsgDefine.FM_Auth_Req fmLogReq;
        public UMsgDefine.FM_Auth_Rsp fmLogRsp;
        public Socket clientSocketTCP,clientSocketUDP;
        public double v, v1, sysTimeDis;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                Session.Clear();         
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            if(CheckUsers(Login1.UserName,Login1.Password))
            {
                e.Authenticated = true;
            }
            else 
            {
                switch (fmLogRsp.Rspcode)
                {
                    case 16908314:
                        Login1.FailureText = "用户名错误!";
                        break;
                    case 16908293:
                        Login1.FailureText = "密码错误!";
                        break;
                    case 16908297:
                        Login1.FailureText = "该用户已登录!";
                        break;
                    case 16908296:
                        Login1.FailureText = "该用户的账号已被锁定!";
                        break;
                    case 16908291:
                        Login1.FailureText = "用户登录认证过程中产生未知错误!";
                        break;
                    case 16912385:
                        Login1.FailureText = "该任务尚未加载或该任务已完成!";
                        break;
                    case 16908304:
                        Login1.FailureText = "服务器用户已达上限!";
                        break;
                    case 16908295:
                        Login1.FailureText = "本账号已经被禁用!";
                        break;
                }
                e.Authenticated = false;
            }
        }
        
        protected bool CheckUsers(string UserName, string Password)
        {
            clientSocketTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocketTCP.Connect(new IPEndPoint(ip, 27520)); //配置服务器IP与端口  
            }
            catch
            {
                return false;
            }

            clientSocketUDP = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                clientSocketUDP.Connect(new IPEndPoint(ip, 27510)); //配置服务器IP与端口  
            }
            catch
            {
                return false;
            }

            fmLogReq.UserName = "".ToCharArray();
            fmLogReq.UserPwd = "".ToCharArray();
            fmLogReq.MsgHead.MsgType = WebApplication1.Data.UConstDefine.TM_AUTH_REQ;
            fmLogReq.MsgHead.MsgLength = Marshal.SizeOf(typeof(UMsgDefine.FM_Auth_Req));
            fmLogReq.UserName = UserName.PadRight(24, '\0').ToCharArray();
            fmLogReq.UserPwd = Password.PadRight(20, '\0').ToCharArray();
            byte[] Message = UMsgDefine.StructToBytes(fmLogReq);
            clientSocketTCP.Send(Message, fmLogReq.MsgHead.MsgLength, SocketFlags.None);
            byte[] RcvMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.FM_Auth_Rsp))];
            clientSocketTCP.Receive(RcvMessage, Marshal.SizeOf(typeof(UMsgDefine.FM_Auth_Rsp)), SocketFlags.None);
       
            fmLogRsp = (UMsgDefine.FM_Auth_Rsp)UMsgDefine.BytesToStruct(RcvMessage, typeof(UMsgDefine.FM_Auth_Rsp));
            
            if (fmLogRsp.MsgHead.MsgType == 16842754 && fmLogRsp.Rspcode == 16908289)
            {
                v = UnitGlobalV.ts.TotalDays + (double)(fmLogRsp.ServerTime + 8 * 60 * 60) / 86400;
                v1 = (DateTime.Now - UnitGlobalV.delphiTime).TotalDays;
                sysTimeDis = Math.Round(v - v1,6);

                System.IO.Directory.CreateDirectory(Server.MapPath(".") + @"/PicTemp" + fmLogRsp.UserInfo.UserID.ToString());

                Session["UserID"] = fmLogRsp.UserInfo.UserID;
                Session["fmLogReq"] = fmLogReq;
                Session["fmLogRsp"] = fmLogRsp;
                Session["SocketTCP"] = clientSocketTCP;
                Session["SocketUDP"] = clientSocketUDP;
              
                Session["sysTimeDis"] = sysTimeDis;
                return true;
            }
            return false;            
        }
    }
}
