<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Blk.aspx.cs" Inherits="WebApplication1.Blk" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title>基于大数据中心的网上阅卷系统客户端</title>
<link href="layout.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .multieditbox{
            border: 1px solid #b7b7b7;
            background: #f8f8f8;
            color: #000000;
            cursor: text;
            font-family: "arial";
            font-size: 9pt;
            padding: 1px;
            margin-top: 6px;
            margin-left: 0px;
        }
        .FootDate{
        float: right;
        }
    </style>
    <script type="text/javascript">
    
      function pageLoad() {
      }
      function checkBlk() {
          var Obj1 = document.getElementById("DropDownList1").value;
          var Obj2 = document.getElementById("DropDownList2").value;
          var Obj3 = document.getElementById("DropDownList3").value;
          if (Obj1 == "-1" || Obj2 == "-1" || Obj3 == "-1") {
              alert("请选择题块!");
              return false;
          }
          else return true;
      }
      Date.prototype.DatePart = function (interval) {
          var myDate = new Date();
          var partStr = '';
          var Week = ['日', '一', '二', '三', '四', '五', '六'];
          switch (interval) {
              case 'y': partStr = myDate.getFullYear(); break;
              case 'm': partStr = myDate.getMonth() + 1; break;
              case 'd': partStr = myDate.getDate(); break;
              case 'w': partStr = Week[myDate.getDay()]; break;
              case 'ww': partStr = myDate.WeekNumOfYear(); break;
              case 'h': partStr = checkTime(myDate.getHours()); break;
              case 'n': partStr = checkTime(myDate.getMinutes()); break;
              case 's': partStr = checkTime(myDate.getSeconds()); break;
          }
          return partStr;
      }
      function checkTime(i) {
          if (i < 10) {
              i = "0" + i;
          }
          return i;
      }
      function getCurrentTime() {

          var cur_time = "";
          cur_time += Date.prototype.DatePart("y") + "年";
          cur_time += Date.prototype.DatePart("m") + "月";
          cur_time += Date.prototype.DatePart("d") + "日 ";
          cur_time += "星期" + Date.prototype.DatePart("w") + " ";
          cur_time += Date.prototype.DatePart("h") + ":";
          cur_time += Date.prototype.DatePart("n") + ":";
          cur_time += Date.prototype.DatePart("s");
          document.getElementById("b_current_time").innerHTML = cur_time;
          setTimeout(getCurrentTime, 1000);
      }
    </script>
</head>
<body onload="getCurrentTime();">
    <form id="form1" runat="server">
    <div id="container">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div id="header">
            <div id="Div2">&nbsp;<asp:Label ID="Label8" runat="server" BorderColor="#666666" BorderStyle="Outset" BorderWidth="5px" Font-Bold="True" Font-Italic="True" Font-Names="华文仿宋" Font-Size="XX-Large" ForeColor="White" Text="网上阅卷系统客户端"></asp:Label>
            <br />
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </div>
        </div>
        <div id="mainContent">           
        <div id="sidebar"></div>
        <div id="content">
            <div id="div1" style="clear:both;"></div>
            <div id="content1">
                <asp:TextBox ID="TextBox1" runat="server" Textmode="MultiLine" BackColor="White" ReadOnly="True" CssClass="multieditbox" Height="199px" Width="203px"></asp:TextBox>
            </div>
            <div id="content2">
            <asp:Label ID="Label1" runat="server" Text="欢迎"></asp:Label>
            <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
            <asp:Label ID="Label3" runat="server" Text="老师登陆"></asp:Label>
            <br />
            <asp:Label ID="Label4" runat="server" Text="登陆ID为："></asp:Label>
            <asp:Label ID="Label5" runat="server" Text="Label"></asp:Label>
            <br />
            <asp:Label ID="Label7" runat="server" Text="登录时间为："></asp:Label>
            <asp:Label ID="Label6" runat="server" Text="Label"></asp:Label>
            <br />       
            <br />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>                    
                    <asp:Label ID="Label19" runat="server" Text="考试信息:"></asp:Label>
                    <asp:DropDownList ID="DropDownList1" runat="server" 
                            onselectedindexchanged="DropDownList1_SelectedIndexChanged" AutoPostBack = "true" >
                    </asp:DropDownList>
                    &nbsp;
                    <br /><br />                   
                    <asp:Label ID="Label20" runat="server" Text="班级信息:"></asp:Label>
                    <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack = "true">
                    </asp:DropDownList>
                    <br /><br />
                    <asp:Label ID="Label21" runat="server" Text="题块信息:"></asp:Label>
                    <asp:DropDownList ID="DropDownList3" runat="server" AutoPostBack = "true">
                    </asp:DropDownList>
                    <br /><br />
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="阅卷" />
                &nbsp;&nbsp;&nbsp;<asp:Button ID="Button3" runat="server" onclick="Button2_Click" style="margin-bottom: 0px" Text="返回" />
            </div>
         </div>
         <div id="div4" style="clear:both; height: 4px;"></div>
         <div id="footer">
             <asp:Label ID="Label9" runat="server" ForeColor="#FFFFFF" Text="您尚未登录！" Font-Bold="True"></asp:Label>
             
             <b id="b_current_time" style="color:white" class="FootDate"></b>
         </div>
         </div>
    </div>
    </form>
</body>
</html>
