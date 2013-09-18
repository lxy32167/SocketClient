<%@ Control Language="C#" AutoEventWireup="true" %>
<menu class="sub" label="Processing">
    <asp:ImageButton class="button" ID="Button_Rotate90" runat="server" ImageUrl="~/Icons/Rotate-Right.png"
        ToolTip="Rotates the Image Clockwise" Height="24px" Width="24px" OnClientClick="Rotate(90); return false;" />
    <asp:ImageButton class="button" ID="Button_Rotate270" runat="server" ImageUrl="~/Icons/Rotate-Left.png"
        ToolTip="Rotates the Image Counter-Clockwise" Height="24px" Width="24px" OnClientClick="Rotate(270); return false;" />
    <asp:ImageButton class="button" ID="Button_Rotate180" runat="server" ImageUrl="~/Icons/Rotate-180.png"
        ToolTip="Rotates the Image 180 Degrees" Height="24px" Width="24px" OnClientClick="Rotate(180); return false;" />
</menu>

<script type="text/javascript">

    function Rotate(degree) {
        WebAnnotationViewer1.RemoteInvoked = RefreshProcessedImage;

        var a = new Array();
        a.push("Rotate");
        a.push(degree);

        WebAnnotationViewer1.RemoteInvoke('RemoteRotateImage', a);

    }
    function RefreshProcessedImage() {
        WebAnnotationViewer1.RemoteInvoked = null;
        WebAnnotationViewer1.getSelection().setVisibility('hidden');

    }

</script>
