<%@ Page Title="Lineas activas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lineas.aspx.cs" Inherits="WebCliente.Lineas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Lineas activas a su nombre</h1>
    <section class="panel">
        <h2>Prepago</h2>
        <asp:GridView runat="server" ID="PrepagoGrid" AutoGenerateColumns="false" EmptyDataText="No hay lineas prepago activas.">
            <Columns>
                <asp:BoundField HeaderText="Telefono" DataField="Telefono" />
                <asp:BoundField HeaderText="Saldo" DataField="Saldo" />
            </Columns>
        </asp:GridView>
    </section>
    <section class="panel">
        <h2>Postpago</h2>
        <asp:GridView runat="server" ID="PostpagoGrid" AutoGenerateColumns="false" EmptyDataText="No hay lineas postpago activas.">
            <Columns>
                <asp:BoundField HeaderText="Telefono" DataField="Telefono" />
                <asp:BoundField HeaderText="Facturacion pendiente" DataField="Pendiente" />
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
