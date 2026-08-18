<%@ Page Title="Calcular facturacion" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Facturacion.aspx.cs" Inherits="WebAdministrativo.Facturacion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Calcular facturacion</h1>
    <section class="panel">
        <h2>Ultima facturacion</h2>
        <asp:Label runat="server" ID="UltimaFacturacionLabel" Text="Pendiente de consultar WS_PROVEEDOR." />
    </section>
    <section class="panel">
        <h2>Nuevo calculo</h2>
        <label>Fecha de calculo</label>
        <asp:TextBox runat="server" ID="FechaCalculoText" TextMode="Date" />
        <label>Fecha maxima de pago</label>
        <asp:TextBox runat="server" ID="FechaMaximaPagoText" TextMode="Date" />
        <asp:Button runat="server" ID="CalcularButton" Text="Ejecutar calculo" OnClick="CalcularButton_Click" />
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
    </section>
</asp:Content>
