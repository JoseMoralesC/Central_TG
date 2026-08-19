<%@ Page Title="Facturacion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Facturacion.aspx.cs" Inherits="WebAdministrativo.Facturacion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Facturacion postpago</h1>
    <section class="panel">
        <h2>Ultima factura generada</h2>
        <asp:Label runat="server" ID="UltimaFacturacionLabel" Text="Pendiente de consultar WS_PROVEEDOR." />
    </section>
    <section class="panel">
        <h2>Consulta individual</h2>
        <label>Linea postpago</label>
        <asp:DropDownList runat="server" ID="LineaPostpagoList" />
        <label>Fecha de calculo</label>
        <asp:TextBox runat="server" ID="FechaCalculoText" TextMode="Date" />
        <label>Fecha maxima de pago</label>
        <asp:TextBox runat="server" ID="FechaMaximaPagoText" TextMode="Date" />
        <asp:Button runat="server" ID="CalcularButton" Text="Consultar factura" OnClick="CalcularButton_Click" />
        <asp:Panel runat="server" ID="ResultadoConsultaPanel" CssClass="result-summary" Visible="false">
            <strong>Resultado de consulta</strong>
            <asp:Label runat="server" ID="ResultadoConsultaLabel" />
            <asp:Button runat="server" ID="GenerarFacturaButton" Text="Generar factura" OnClick="GenerarFacturaButton_Click" CssClass="secondary-action" />
        </asp:Panel>
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
    </section>
</asp:Content>
