<%@ Page Title="Nuevas lineas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LineasNuevas.aspx.cs" Inherits="WebAdministrativo.LineasNuevas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Poner nuevas lineas telefonicas a disposicion</h1>
    <section class="panel">
        <asp:Button runat="server" ID="NuevoButton" Text="Nuevo" OnClick="NuevoButton_Click" />
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
        <asp:GridView runat="server" ID="LineasGrid" AutoGenerateColumns="false" OnRowCommand="LineasGrid_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Telefono" DataField="NumeroTelefono" />
                <asp:BoundField HeaderText="Identificador telefono" DataField="IdentificadorTelefonoVisible" />
                <asp:BoundField HeaderText="Identificador tarjeta" DataField="IdentificadorTarjetaVisible" />
                <asp:BoundField HeaderText="Tipo servicio" DataField="TipoServicio" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <span class="grid-actions">
                            <asp:LinkButton runat="server" CssClass="danger-link" Text="Eliminar" CommandName="EliminarLinea" CommandArgument='<%# Eval("ServicioId") %>' OnClientClick="return confirm('Esta seguro de borrar la linea telefonica?');" />
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </section>
    <section class="panel" runat="server" id="FormularioPanel" visible="false">
        <h2>Nueva linea</h2>
        <label>Nuevo numero de telefono</label>
        <asp:TextBox runat="server" ID="NumeroText" MaxLength="30" />
        <label>Identificador del telefono</label>
        <asp:TextBox runat="server" ID="IdentificadorTelefonoText" MaxLength="120" ReadOnly="true" />
        <label>Identificador de la tarjeta</label>
        <asp:TextBox runat="server" ID="IdentificadorTarjetaText" MaxLength="120" ReadOnly="true" />
        <label>Tipo de servicio</label>
        <asp:DropDownList runat="server" ID="TipoServicioList">
            <asp:ListItem Text="Prepago" Value="PREPAGO" />
            <asp:ListItem Text="Postpago" Value="POSTPAGO" />
        </asp:DropDownList>
        <asp:Button runat="server" ID="GenerarIdsButton" Text="Generar IDs" OnClick="GenerarIdsButton_Click" />
        <asp:Button runat="server" ID="GuardarButton" Text="Guardar" OnClick="GuardarButton_Click" />
    </section>
</asp:Content>
