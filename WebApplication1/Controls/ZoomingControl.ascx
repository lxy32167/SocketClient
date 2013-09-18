<%@ Control Language="C#" AutoEventWireup="true" %>
    
<menu class="sub" label="Zoom">

            <asp:ImageButton CssClass="button" ID="Button_ZoomIn" runat="server" ImageUrl="~/Icons/Page-ZoomIn.png"
                ToolTip="Zoom In" Height="24px" Width="24px" OnClientClick="btnZoomIn(); return false;" />
            <asp:ImageButton CssClass="button" ID="Button_ZoomOut" runat="server" ImageUrl="~/Icons/Page-ZoomOut.png"
                ToolTip="Zoom Out" Height="24px" Width="24px" OnClientClick="btnZoomOut(); return false;" />

            <asp:ImageButton CssClass="button" ID="Button_FitWidth" runat="server" ImageUrl="~/Icons/Fit-Width.png"
                ToolTip="Fit to Width" Height="24px" Width="24px" OnClientClick="btnFitWidth(); return false;" />

            <asp:ImageButton CssClass="button" ID="Button_BestFit" runat="server" ImageUrl="~/Icons/Fit-Best.png"
                ToolTip="Best Fit" Height="24px" Width="24px" OnClientClick="btnBestFit(); return false;" />

            <asp:ImageButton CssClass="button" ID="Button_ZoomFull" runat="server" ImageUrl="~/Icons/Page-ZoomFull.png"
                ToolTip="Shows the image at it's full size." Height="24px" Width="24px" OnClientClick="btnZoomFull(); return false;" />

</menu>
<script type="text/javascript">

    // Increments and decrements the zoom level by the given amount
    function Zoom(inOut) {
        var z = WebAnnotationViewer1.getZoom();
        var zp = WebAnnotationViewer1.getZoomInOutPercentage();

        z += inOut * z * zp / 100;

        WebAnnotationViewer1.setZoom(z);
    }

    function btnZoomIn() {
        WebAnnotationViewer1.setAutoZoom(AutoZoomMode.None);
        Zoom(1);
    }

    function btnZoomOut() {
        WebAnnotationViewer1.setAutoZoom(AutoZoomMode.None);
        Zoom(-1);
    }

    function btnFitWidth() {
        WebAnnotationViewer1.setAutoZoom(AutoZoomMode.FitToWidth);
    }

    function btnBestFit() {
        WebAnnotationViewer1.setAutoZoom(AutoZoomMode.BestFit);
    }

    function btnZoomFull() {
        WebAnnotationViewer1.setAutoZoom(AutoZoomMode.None);
        WebAnnotationViewer1.setZoom(1);
    }

</script>
