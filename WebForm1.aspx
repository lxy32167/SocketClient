﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication1.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta http-equiv="Pragma"   content="no-cache"/>    
<meta http-equiv="Cache-Control" content="no-cache"/>    
<meta http-equiv="Expires" content="0"/>    
<title>基于大数据中心的网上阅卷系统客户端</title>
<style type ="text/css">
    .mainTitle
{
    font-size: 12pt;
    font-weight: bold;
    font-family: 宋体;
}
.commonText
{
    font-size: 11pt;
    font-family: 宋体;
}
.littleMainTitle
{
    font-size: 10pt;
    font-weight: bold;
    font-family: 宋体;
}
.TopTitle
{
    border: 0px;
    font-size: 10pt;
    font-weight: bold;
    text-decoration: none;
    color: Black;
    display: inline-block;
    width: 100%;    
}
.SelectedTopTitle
{
    border: 0px;
    font-size: 10pt;
    text-decoration: none;
    color: Black;
    display: inline-block;
    width: 100%;
    background-color: White;
}
.ContentView
{
    border: 0px;
    padding: 3px 3px 3px 3px;
    background-color: White;
    display: inline-block;
    width: 390px;
}
.SepBorder
{
    border-top-width: 0px;
    border-left-width: 0px;
    font-size: 1px;
    border-bottom: Gray 1px solid;
    border-right-width: 0px;
}
.TopBorder
{
    border-right: Gray 1px solid;
    border-top: Gray 1px solid;
    background: #DCDCDC;
    border-left: Gray 1px solid;
    color: black;
    border-bottom: Gray 1px solid;
}
.ContentBorder
{
    border-right: Gray 1px solid;
    border-top: Gray 0px solid;
    border-left: Gray 1px solid;
    border-bottom: Gray 1px solid;
    height: 100%;
    width: 100%;
}
.SelectedTopBorder
{
    border-right: Gray 1px solid;
    border-top: Gray 1px solid;
    background: none transparent scroll repeat 0% 0%;
    border-left: Gray 1px solid;
    color: black;
    border-bottom: Gray 0px solid;
}
.Button
    {
        position: relative;
        left: 60px;
    }
    .Title
    {
        text-align: center;
    }
    .FootName
    {
        position:relative;
        left: 100px;
    }
    .FootDate
    {
        float: right;
    }
</style>
<link href="layout.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jquery-1.9.1.js"></script>
    <script type="text/javascript" src="jquery.blockUI.js"></script>
    <script type="text/javascript">
// <!CDATA[
        function userLogin() {
            $.blockUI({ message: $('#loginDiv'), css: { width: '275px' } });
        }
        function login() {
            var i = document.getElementById('<%=hidden.ClientID %>').value;
            var j = document.getElementById('<%=hidden1.ClientID %>').value;
            var uName = $("#username").val();
            var uPwd = $("#userpwd").val();
            $.blockUI({ message: "<h1>登录中...</h1>" });
            if (i == uName && j == uPwd) {
                $.unblockUI();
                document.getElementById('username').value = '';
                document.getElementById('userpwd').value = '';
            }

            else {
                $.unblockUI();
                document.getElementById('username').value = '';
                document.getElementById('userpwd').value = '';
                userLogin();
            }
        }
        function userChgPwd() {
            $.blockUI({ message: $('#ChgPwdDiv'), css: { width: '275px' } });          
        }
        function chgpwd() {
            var oldpwd = $("#OldPwd").val();
            var newpwd = $("#NewPwd").val();
            var newpwd2 = $("#NewPwd2").val();
       
            if (oldpwd.length == 0) {
                $.blockUI({ message: "<h1>原密码未输入!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd.length == 0 || newpwd2.length == 0) {
                $.blockUI({ message: "<h1>请输入新密码!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd != newpwd2) {
                $.blockUI({ message: "<h1>密码不一致!</h1>" });
                $.unblockUI();
                return false;
            }
            else if (newpwd.length < 4) {
                $.blockUI({ message: "<h1>密码至少需要4位!</h1>" });
                $.unblockUI();
                return false;
            }            
            else if (oldpwd != null && newpwd != null) {
                $.unblockUI();
                if (window.XMLHttpRequest) {
                    xmlHttp = new XMLHttpRequest();
                }
                else {
                    xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                if (xmlHttp) {
                    xmlHttp.open("GET", "validatePwd.aspx?oldpwd=" + oldpwd + "&newpwd=" + newpwd, true);
                    xmlHttp.onreadystatechange = getdata;
                    xmlHttp.send();
                }
            }
        }
        function getdata() {
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                alert(xmlHttp.responseText);
            }
        }
        function reLogin() {
            if (window.XMLHttpRequest) {
                xmlHttp = new XMLHttpRequest();
            }
            else {
                xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            if (xmlHttp) {
                xmlHttp.open("GET", "reLogin.aspx", true);
                xmlHttp.send();
            }
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
    <form id="form1" runat="server" defaultbutton ="Button5">
        <div id="loginDiv" style="display:none; cursor: default"> 
        <h3>请输入密码解锁！</h3> 
          <table style="width:300px; text-align:center">
            <tr>
                <td>用户名</td><td><input type="text" id="username" autocomplete="off" /></td>
            </tr>
            <tr>
                <td>密码</td><td><input type="password" id="userpwd" autocomplete="off"/></td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <input type="button" id="yes" onclick="login()" value="登录"/>
                </td>
            </tr>
       </table>       
    </div>
    <div id="ChgPwdDiv" style="display:none; cursor: default"> 
       <h3>请输入原密码和新密码!</h3> 
       <table style="width:300px; text-align:center">
            <tr>
                <td>原密码</td><td><input type="password" id="OldPwd" autocomplete="off"/></td>
            </tr>
            <tr>
                <td>新密码</td><td><input type="password" id="NewPwd" autocomplete="off"/></td>
            </tr>
            <tr>
                <td>确认密码</td><td><input type="password" id="NewPwd2" autocomplete="off"/></td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <input id="Button2" onclick="chgpwd()" type="button" value="确认"/>        
                </td>
            </tr>
       </table>       
    </div>
        <div id="container">
         <asp:ScriptManager ID="ScriptManager1" runat="server">
         </asp:ScriptManager>
         <label id="lblMsg" runat="server"></label>
         <input type="hidden" name="hidden" id="hidden"  runat="server"/>
         <input type="hidden" name="hidden1" id="hidden1" runat="server"/>
            <div id="header_two">
                <fieldset style="width:500px">
                <legend>系统菜单</legend>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                <table cellpadding="0" cellspacing="0" width="100%" border="0">
                <tr>
                    <td>                                                      
                        <table id="Table1" runat="server" cellpadding="0" cellspacing="0" width="100%" border="0">
                            <tr style="height:22px">
                                <td class="SelectedTopBorder" id="Cell1" align="center" style="width:80px;">
                                            <asp:LinkButton ID="lButtonCompany" runat="server" OnClick="lButtonCompany_Click">账户设置</asp:LinkButton>
                                </td>
                                <td class="SepBorder" style="width:2px; height: 22px;">  
                                </td>
                                <td class="TopBorder" id="Cell2" align="center" style="width:80px;">
                                            <asp:LinkButton ID="lButtonProduct" runat="server" OnClick="lButtonProduct_Click">系统设置</asp:LinkButton>
                                </td>
                                <td class="SepBorder" style="width:2px; height: 22px;"></td>
                                <td class="TopBorder" id="Cell3" align="center" style="width:80px;">
                                            <asp:LinkButton ID="lButtonContact" runat="server" OnClick="lButtonContact_Click">联系我们</asp:LinkButton>
                                </td>
                                <td class="SepBorder" style="width:2px; height: 22px;"></td>                                
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="ContentBorder" cellpadding="0" cellspacing="0" width="100%">
                              <tr>
                                  <td valign="top">
                                      
                                      <asp:MultiView ID="mvCompany" runat="server" ActiveViewIndex="0">
                                          <asp:View ID="View1" runat="server">
                                                      <asp:Button ID="Button7" runat="server" Text="重选题块" OnClick="Button7_Click"/>                        
                                                      <asp:Button ID="Button1" CssClass="Button" runat="server" onclick="Button1_Click" Text="注销用户" />                         
                                          </asp:View>
                                          <asp:View ID="View2" runat="server">
                                              <input id="Button6" type="button" value="锁定" onclick="userLogin()"/>
                                              <input id ="Button3" class="Button" type="button"  onclick="userChgPwd();" value="修改密码"   />
                                          </asp:View>
                                          <asp:View ID="View3" runat="server">
                                              该网上阅卷系统基于ASP.NET开发,和原delphi开发的C/S版客户端互相辅助,完成阅卷。</asp:View>
                                       </asp:MultiView>
                                  </td>
                              </tr>
                              <tr>
                                  <td valign="top"></td>
                              </tr>
                           </table>
                       </td>
                  </tr>
            </table>
            </ContentTemplate>
            <Triggers>
            <asp:AsyncPostBackTrigger ControlID="lButtonCompany" />
            <asp:AsyncPostBackTrigger ControlID="lButtonProduct" />
            <asp:AsyncPostBackTrigger ControlID="lButtonContact" />                 
            </Triggers>
            </asp:UpdatePanel>
        </fieldset>
            </div>
            <div id="mainContent">
                <div id="content" style="overflow:auto">                
                <asp:Image ID="Image1" runat="server"/><br />
                </div>
                <div id="sidebar">
                    <div id="givescore" class="Title">
                    <asp:Label ID="Label9" runat="server" ForeColor="#FFFFFF" Text="试卷给分区" Font-Bold="True"></asp:Label>
                    </div>
                    <div id="sidebarup" style="overflow:auto"> 
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                    <ContentTemplate>                 
                    <asp:GridView ID="GridView2" runat="server" EnableModelValidation="True" AllowPaging="True" AutoGenerateColumns="False"
                     DataKeyNames="步骤" CellPadding="4" ForeColor="#333333" OnRowCommand="GridView2_RowCommand"
                     OnRowDataBound="GridView2_RowDataBound" OnRowUpdating="GridView2_RowUpdating" Width="200px" OnPreRender="GridView2_PreRender">
                     <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                     <Columns>
                          <asp:ButtonField CommandName="SingleClick" Text="SingleClick" Visible="False" />
                          <asp:BoundField DataField="步骤" HeaderText="步骤" InsertVisible="False" ReadOnly="True" SortExpression="步骤" ItemStyle-HorizontalAlign="Center"/>
                          <asp:TemplateField HeaderText="分数" SortExpression="分数">
                            <ItemTemplate>
                                <asp:Label ID="lblScore" runat="server" Text='<%# Eval("分数") %>'></asp:Label>
                                <asp:TextBox ID="txtScore" runat="server" Text='<%# Eval("分数") %>' Visible="false" Height="16px" Width="25px" AutoCompleteType="Disabled"  onkeyup="if(this.value.length==1){this.value=this.value.replace(/[^1-9]/g,'')}else{this.value=this.value.replace(/\D/g,'')}" onafterpaste="if(this.value.length==1){this.value=this.value.replace(/[^1-9]/g,'')}else{this.value=this.value.replace(/\D/g,'')}"></asp:TextBox>
                            </ItemTemplate>
                          </asp:TemplateField>
                          <asp:BoundField DataField="满分" HeaderText="满分" InsertVisible="False" ReadOnly="True" SortExpression="满分" ItemStyle-HorizontalAlign="Center"/>
                     </Columns>
                     <EditRowStyle BackColor="#999999" />
                     <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                     <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                     <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                     <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                     <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                     </asp:GridView>
                     </ContentTemplate>
                     </asp:UpdatePanel>                 
                     </div> 
                     <div id="submitbutton">'
                     <asp:Button ID="Button5" runat="server" OnClick="Button5_Click" style="margin-left:10px;margin-bottom:10px" Text="分数提交" />      
                     </div> 
                     <div id="stuname" class="Title">
                     <asp:Label ID="Label3" runat="server" ForeColor="#000000" Font-Bold="True"></asp:Label>
                     </div>
                     <div id="checkname" class="Title">
                     <asp:Label ID="Label2" runat="server" ForeColor="#FFFFFF" Text="试卷重查区" Font-Bold="True"></asp:Label>
                     </div>
                     <div id ="CheckCell">
                     <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="always">
                     <ContentTemplate>
                     <table id="Table2" runat="server" cellpadding="0" cellspacing="0" width="100%" border="0">
                            <tr style="height:12px">
                                <td class="SelectedTopBorder" id="Td1" align="center" style="width:80px;">
                                            <asp:LinkButton ID="Check1" runat="server" >复查</asp:LinkButton>
                                </td>
                                <td class="SepBorder" style="width:2px; height: 12px;">  
                                </td>
                                <td class="TopBorder" id="Td2" align="center" style="width:80px;">
                                            <asp:LinkButton ID="Check2" runat="server" >重评</asp:LinkButton>
                                </td>
                                <td class="SepBorder" style="width:2px; height: 12px;"></td>
                                <td class="TopBorder" id="Td3" align="center" style="width:80px;">
                                            <asp:LinkButton ID="Check3" runat="server" >详查</asp:LinkButton>
                                </td>
                                <td class="SepBorder" style="width:2px; height: 12px;"></td>                                
                            </tr>
                     </table>
                     </ContentTemplate> 
                     </asp:UpdatePanel>
                     </div>  
                     <div id="sidebardown" style="overflow:auto">          
                     <asp:GridView ID="GridView1" runat="server" DataKeyNames="试卷号" CellPadding="4" AllowPaging="true" EnableModelValidation="True" ForeColor="#333333" AutoGenerateColumns="False" PageSize="4" OnPageIndexChanging="GridView1_PageIndexChanging" AllowSorting="True" OnSorting="GridView1_Sorting">
                     <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                     <Columns>
                         <asp:TemplateField HeaderText="试卷号" InsertVisible="False" SortExpression="试卷号" Visible="False">         
                             <ItemTemplate>
                                 <asp:Label ID="Label1" runat="server" Text='<%# Bind("试卷号") %>'></asp:Label>
                             </ItemTemplate>
                         </asp:TemplateField>
                      
                        <asp:BoundField DataField="学号" HeaderText="学号" InsertVisible="False" ReadOnly="True"/>
                        <asp:BoundField DataField="分数" HeaderText="分数" InsertVisible="False" ReadOnly="True"/>
                        <asp:BoundField DataField="详细分数" HeaderText="详细分数" InsertVisible="False" ReadOnly="True" />
                        <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="选择" onclick="LinkButton1_Click"></asp:LinkButton>
                        </ItemTemplate>
                        </asp:TemplateField>
                     </Columns>
                     <EditRowStyle BackColor="#999999" />
                     <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                     <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                     <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                     <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                     </asp:GridView>
                     </div>   
                </div>               
            <div id="div4" style="clear:both; height: 4px;"></div>
            <div id="footer">
                 <asp:Label ID="Label4" runat="server" ForeColor="#FFFFFF" Font-Bold="True"></asp:Label>
          
                 <asp:Label ID="Label5" CssClass="FootName" runat="server" ForeColor="#FFFFFF" Font-Bold="True"></asp:Label>
            
                 <b id="b_current_time" style="color:white" class="FootDate"></b>
            </div>
            </div>
        </div>

    </form>
</body>
</html>
