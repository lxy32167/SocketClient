<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication1._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title>基于大数据中心的网上阅卷系统客户端</title>
<link href="layout.css" rel="stylesheet" type="text/css" />
<style type="text/css">
    #content
    {
        font-family: Verdana;
        font-size: 9pt;
    }
    .loginbody
    {
        font: 9pt verdana;
        background-color: lightblue;
        border: solid 3px black;
        padding: 4px;
    }
    .login_title
    {
        background-color: darkblue;
        color: white;
        font-weight: bold;
        font-family: Verdana;
    }
    .login_instructions
    {
        font-size: 12px;
        text-align: left;
        padding: 10px;
        font-family: Verdana;
    }
    .login_button
    {
        border: solid 1px black;
        padding: 3px;
        font-family: Verdana;
    }
    .FootDate
    {
        float: right;
    }
</style>
    <script type="text/javascript">
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
        <div id="header">&nbsp;<asp:Label ID="Label1" runat="server" BorderColor="#666666" BorderStyle="Outset" BorderWidth="5px" Font-Bold="True" Font-Italic="True" Font-Names="华文仿宋" Font-Size="XX-Large" ForeColor="White" Text="网上阅卷系统客户端"></asp:Label>
            <br />
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </div>
    <div id="mainContent">
        <div id="sidebar"></div>
        <div id="contentLogin">
            <asp:Login ID="Login1" runat="server" BackColor="#EEEEFF" DisplayRememberMe ="false" InstructionText="输入用户名和密码登陆阅卷系统!" CssClass="loginbody" TitleTextStyle-CssClass="login_title" InstructionTextStyle-CssClass="login_instructions" LoginButtonStyle-CssClass="login_button" TitleText="网上阅卷系统客户端登录" onauthenticate="Login1_Authenticate" DestinationPageUrl="~/Blk.aspx">
                <LoginButtonStyle CssClass="login_button" />
                <InstructionTextStyle CssClass ="login_instructions" />
                <TitleTextStyle CssClass="login_title" />
            </asp:Login>
        </div>
    </div>
    <div id="div4" style="clear:both; height: 4px;"></div>
    <div id="footer">
        <asp:Label ID="Label2" runat="server" ForeColor="#FFFFFF" Text="您尚未登录！" Font-Bold="True"></asp:Label>
        
        <b id="b_current_time" style="color:white" class="FootDate"></b>
        </div>
    </div>
    </form>
</body>
</html>
