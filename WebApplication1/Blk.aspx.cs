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
using System.Collections;

namespace WebApplication1 
{
    public partial class Blk : System.Web.UI.Page
    {
        public UMsgDefine.tag_ExamTask[] ExamTask;
        public UMsgDefine.FM_UserLogout fmLogout;
        int i, j, tmpint;
        Boolean AddFlag;
        String tmpstr, tmpstr1,tmpstr2 ="";

        List<BlkUseClass.BlkArray> BlkArray = new List<BlkUseClass.BlkArray>();
        List<BlkUseClass.ClassArray> ClassArray = new List<BlkUseClass.ClassArray>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
               Response.Redirect("~/Login.aspx");
            }

            //RcvTask

            if (this.IsPostBack == false)
            {
                Button1.Attributes.Add("onclick", "return checkBlk();");
                Button3.Attributes["onclick"] = "Javascript:return window.confirm('确定返回登陆界面?')";
                TextBox1.Attributes.Add("readonly", "true");
                Label2.Text = new String(((UMsgDefine.FM_Auth_Rsp)Session["fmLogRsp"]).UserInfo.TrueName);
                Label5.Text = new String(((UMsgDefine.FM_Auth_Rsp)Session["fmLogRsp"]).UserInfo.LoginName);
                Label6.Text = (DateTime.Now).AddDays((double)Session["sysTimeDis"]).ToString();
                DropDownList1.Items.Add(new ListItem("Select", "-1"));
                DropDownList2.Items.Add(new ListItem("Select", "-1"));
                DropDownList3.Items.Add(new ListItem("Select", "-1"));

                byte[] RcvTaskMessage = new byte[Marshal.SizeOf(typeof(UMsgDefine.tag_ExamTask))];

                ExamTask = new UMsgDefine.tag_ExamTask[((UMsgDefine.FM_Auth_Rsp)Session["fmLogRsp"]).UserInfo.Tasknum];
                BlkUseClass.ClassArray CArray = new BlkUseClass.ClassArray();
                BlkUseClass.BlkArray BArray = new BlkUseClass.BlkArray();
              
                for (i = 0; i < ((UMsgDefine.FM_Auth_Rsp)Session["fmLogRsp"]).UserInfo.Tasknum; i++)
                {
                    
  //                  if (Session["ReLogin"] == null || (bool)Session["ReLogin"] == false)
  //                  {
                    ((Socket)Session["SocketTCP"]).Receive(RcvTaskMessage, Marshal.SizeOf(typeof(UMsgDefine.tag_ExamTask)), SocketFlags.None);
                    ExamTask[i] = (UMsgDefine.tag_ExamTask)UMsgDefine.BytesToStruct(RcvTaskMessage, typeof(UMsgDefine.tag_ExamTask));
                  
   //                 }
   //                 else if ((bool)Session["ReLogin"] == true)
   //                 {
   //                     ExamTask[i] = ((UMsgDefine.tag_ExamTask[])Session["ExamTask"])[i];
                        
   //                 }
                    tmpstr2 += "考试名称: " + new String(ExamTask[i].CourseInfo.CName).TrimEnd('\0') +"\r\n";
                    tmpstr2 += "所属班级: " + new String(ExamTask[i].ClassInfo.ClassName).TrimEnd('\0') + "\r\n";
                    tmpstr2 += "题块: " + ExamTask[i].BlkNo.ToString() + "\r\n";
                    tmpstr2 += "题块已完成/总任务: " + ExamTask[i].nPart.ToString() + "/" + ExamTask[i].nTotal.ToString() + "\r\n";
                    tmpstr2 += "本人已完成: " + ExamTask[i].pPart.ToString() + "\r\n";

                    tmpstr2 += "\r\n";
                    if (DropDownList1.Items.Count == 1)
                    {
                        DropDownList1.Items.Add(new ListItem(new String(ExamTask[i].CourseInfo.CName), ExamTask[i].CourseInfo.CourseID.ToString()));
                    }
                    else
                    {
                        for (j = 0; j < DropDownList1.Items.Count; j++)
                        {
                            if (ExamTask[i].CourseInfo.CourseID.ToString().CompareTo(DropDownList1.Items[j].Value) == 0)
                                break;
                        }
                    }
                    if (j == DropDownList1.Items.Count)
                        DropDownList1.Items.Add(new ListItem(new String(ExamTask[i].CourseInfo.CName), ExamTask[i].CourseInfo.CourseID.ToString()));

                    CArray.CourseID = ExamTask[i].CourseInfo.CourseID;
                    CArray.ClassName = (char[])ExamTask[i].ClassInfo.ClassName.Clone();
                    CArray.ClassID = (char[])ExamTask[i].ClassInfo.ClassID.Clone();
                    ClassArray.Add(CArray);

                    BArray.CourseID = ExamTask[i].CourseInfo.CourseID;
                    BArray.BlkNo = ExamTask[i].BlkNo;
                    BlkArray.Add(BArray);
                }
                TextBox1.Text = tmpstr2;
                Session["ClassArray"] = ClassArray;
                Session["BlkArray"] = BlkArray;
                Session["ExamTask"] = ExamTask;
    //          Session["ReLogin"] = false;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
           Session["CourseID"] = DropDownList1.SelectedValue;
           Session["ClassID"] = DropDownList2.SelectedValue;
           Session["BlkNo"] = DropDownList3.SelectedValue;
           Response.Redirect("~/WebForm1.aspx");
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            fmLogout.UserName = (char[])(((UMsgDefine.FM_Auth_Req)Session["fmLogReq"]).UserName).Clone();
            fmLogout.MsgHead.MsgType = 16842796;
            fmLogout.MsgHead.MsgLength = Marshal.SizeOf(fmLogout);
            byte[] LogoutMessage = UMsgDefine.StructToBytes(fmLogout);
            System.IO.Directory.Delete(Server.MapPath(".") + @"/PicTemp" + Session["UserID"].ToString(),true);
            ((Socket)Session["SocketTCP"]).Send(LogoutMessage, fmLogout.MsgHead.MsgLength, SocketFlags.None);
            ((Socket)Session["SocketTCP"]).Close();
            ((Socket)Session["SocketUDP"]).Close();
            Response.Redirect("~/Login.aspx");
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList1.SelectedIndex == 0)
            {
                DropDownList2.Items.Clear();
                DropDownList3.Items.Clear();
                DropDownList2.Items.Add(new ListItem("Select", "-1"));
                DropDownList3.Items.Add(new ListItem("Select", "-1"));

                DropDownList2.SelectedIndex = 0;
                DropDownList3.SelectedIndex = 0;
            }
            else
            {
                DropDownList2.Items.Clear();
                DropDownList3.Items.Clear();
                DropDownList2.Items.Add(new ListItem("Select", "-1"));
                DropDownList3.Items.Add(new ListItem("Select", "-1"));
                for (i = 0; i < ((UMsgDefine.FM_Auth_Rsp)Session["fmLogRsp"]).UserInfo.Tasknum; i++)
                {
                    AddFlag = true;
                    tmpstr = DropDownList1.SelectedItem.Value;

                    for (j = 0; j < DropDownList3.Items.Count; j++)
                    {
                        tmpint = int.Parse(DropDownList3.Items[j].Value);
                        if (tmpint == ((List<BlkUseClass.BlkArray>)Session["BlkArray"])[i].BlkNo)
                        {
                            AddFlag = false;
                            break;
                        }
                    }

                    if (((List<BlkUseClass.BlkArray>)Session["BlkArray"])[i].CourseID == int.Parse(DropDownList1.SelectedItem.Value) && AddFlag)
                    {
                        DropDownList3.Items.Add(((List<BlkUseClass.BlkArray>)Session["BlkArray"])[i].BlkNo.ToString());

                    }

                    AddFlag = true;

                    for (j = 0; j < DropDownList2.Items.Count; j++)
                    {
                        tmpstr1 = DropDownList2.Items[j].Value;
                        if (tmpstr1.CompareTo((int.Parse(new String(((List < BlkUseClass.ClassArray >)Session["ClassArray"])[i].ClassID))).ToString()) == 0)
                        {
                            AddFlag = false;
                            break;
                        }
                    }

                    if ((tmpstr.CompareTo(((List<BlkUseClass.ClassArray>)Session["ClassArray"])[i].CourseID.ToString()) == 0) && AddFlag)
                    {
                        DropDownList2.Items.Add(new ListItem(new String(((List<BlkUseClass.ClassArray>)Session["ClassArray"])[i].ClassName), int.Parse(new string(((List<BlkUseClass.ClassArray>)Session["ClassArray"])[i].ClassID)).ToString()));
                    }
                  
                }
            }


        }
        private string get_replace(string mystr)
        {
            if (@mystr.Substring(mystr.Length - 2) == @"\0")
            {
                mystr = @mystr.Substring(0, mystr.Length - 2);
                if (@mystr.Substring(mystr.Length - 2) == @"\0")
                {
                    return (this.get_replace(mystr));
                }
                else
                {
                    return (mystr);
                }
            }
            else
            {
                return (mystr);
            }
        }

    }
}

