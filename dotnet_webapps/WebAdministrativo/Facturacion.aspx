<%@ Page Title="Calcular facturacion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Facturacion.aspx.cs" Inherits="WebAdministrativo.Facturacion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Calcular facturacion</h1>
    <section class="panel">
        <h2>Ultimo calculo</h2>
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
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
    </section>
</asp:Content>
