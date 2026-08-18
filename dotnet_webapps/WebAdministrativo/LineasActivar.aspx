<%@ Page Title="Activar linea" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LineasActivar.aspx.cs" Inherits="WebAdministrativo.LineasActivar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Activar una linea recien vendida</h1>
    <section class="panel">
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
        <asp:GridView runat="server" ID="LineasGrid" AutoGenerateColumns="false" OnRowCommand="LineasGrid_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Telefono" DataField="NumeroTelefono" />
                <asp:BoundField HeaderText="Identificador tarjeta" DataField="IdentificadorTarjeta" />
                <asp:BoundField HeaderText="Tipo servicio" DataField="TipoServicio" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" Text="Seleccionar" CommandName="SeleccionarLinea" CommandArgument='<%# Eval("ServicioId") + "|" + Eval("NumeroTelefono") + "|" + Eval("IdentificadorTelefono") + "|" + Eval("IdentificadorTarjeta") + "|" + Eval("TipoServicio") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </section>
    <section class="panel" runat="server" id="ActivacionPanel" visible="false">
        <h2>Datos seleccionados</h2>
        <asp:HiddenField runat="server" ID="NumeroHidden" />
        <asp:HiddenField runat="server" ID="IdentificadorTelefonoHidden" />
        <asp:HiddenField runat="server" ID="IdentificadorTarjetaHidden" />
        <asp:HiddenField runat="server" ID="TipoHidden" />
        <p><strong>Telefono:</strong> <asp:Literal runat="server" ID="TelefonoLiteral" /></p>
        <p><strong>Tipo:</strong> <asp:Literal runat="server" ID="TipoLiteral" /></p>
        <label>Cedula del cliente</label>
        <asp:TextBox runat="server" ID="CedulaText" MaxLength="50" />
        <asp:Button runat="server" ID="ActivarButton" Text="Activar" OnClick="ActivarButton_Click" />
    </section>
</asp:Content>
