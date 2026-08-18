<%@ Page Title="Devolucion linea" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LineasDevolucion.aspx.cs" Inherits="WebAdministrativo.LineasDevolucion" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Devolucion de una linea telefonica</h1>
    <section class="panel">
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
        <asp:GridView runat="server" ID="LineasGrid" AutoGenerateColumns="false" OnRowCommand="LineasGrid_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Telefono" DataField="NumeroTelefono" />
                <asp:BoundField HeaderText="Identificador tarjeta" DataField="IdentificadorTarjeta" />
                <asp:BoundField HeaderText="Identificador telefono" DataField="IdentificadorTelefono" />
                <asp:BoundField HeaderText="Identificacion cliente" DataField="IdentificacionClienteVisible" />
                <asp:BoundField HeaderText="Nombre cliente" DataField="NombreCliente" />
                <asp:BoundField HeaderText="Tipo servicio" DataField="TipoServicio" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CssClass="danger-link" Text="Desactivar" CommandName="DevolverLinea" CommandArgument='<%# Eval("ServicioId") + "|" + Eval("NumeroTelefono") + "|" + Eval("IdentificadorTelefono") + "|" + Eval("IdentificadorTarjeta") + "|" + Eval("TipoServicio") + "|" + Eval("IdentificacionCliente") %>' OnClientClick="return confirm('Esta seguro que desea desactivar la linea?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
