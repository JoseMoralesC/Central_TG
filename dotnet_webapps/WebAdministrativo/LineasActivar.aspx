<%@ Page Title="Asignar linea" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LineasActivar.aspx.cs" Inherits="WebAdministrativo.LineasActivar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Asignar una linea disponible a un cliente</h1>
    <section class="panel">
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
        <h2>Lineas disponibles</h2>
        <asp:GridView runat="server" ID="LineasGrid" AutoGenerateColumns="false" OnRowCommand="LineasGrid_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Telefono" DataField="NumeroTelefono" />
                <asp:BoundField HeaderText="Identificador telefono" DataField="IdentificadorTelefonoVisible" />
                <asp:BoundField HeaderText="Identificador tarjeta" DataField="IdentificadorTarjetaVisible" />
                <asp:BoundField HeaderText="Tipo servicio" DataField="TipoServicio" />
                <asp:BoundField HeaderText="Estado" DataField="EstadoLinea" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" Text="Asignar" CommandName="SeleccionarLinea" CommandArgument='<%# Eval("ServicioId") + "|" + Eval("NumeroTelefono") + "|" + Eval("IdentificadorTelefono") + "|" + Eval("IdentificadorTarjeta") + "|" + Eval("TipoServicio") %>' />
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
        <label>Cliente activo</label>
        <asp:DropDownList runat="server" ID="ClientesList" />
        <asp:Button runat="server" ID="ActivarButton" Text="Asignar y activar" OnClick="ActivarButton_Click" />
    </section>
</asp:Content>
