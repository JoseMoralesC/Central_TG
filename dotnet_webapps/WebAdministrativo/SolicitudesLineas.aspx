<%@ Page Title="Solicitudes de lineas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SolicitudesLineas.aspx.cs" Inherits="WebAdministrativo.SolicitudesLineas" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Solicitudes de lineas</h1>
    <section class="panel">
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
        <asp:GridView runat="server" ID="SolicitudesGrid" AutoGenerateColumns="false" OnRowCommand="SolicitudesGrid_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Fecha" DataField="FechaSolicitud" />
                <asp:BoundField HeaderText="Cedula cliente" DataField="IdentificacionCliente" />
                <asp:BoundField HeaderText="Cliente" DataField="NombreCliente" />
                <asp:BoundField HeaderText="Telefono solicitado" DataField="NumeroTelefono" />
                <asp:BoundField HeaderText="Tipo servicio" DataField="TipoServicio" />
                <asp:BoundField HeaderText="Estado" DataField="Estado" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <span class="grid-actions">
                            <asp:LinkButton runat="server"
                                Text="Asignar"
                                CommandName="AsignarSolicitud"
                                CommandArgument='<%# Eval("SolicitudId") + "|" + Eval("ServicioId") + "|" + Eval("IdentificacionCliente") %>'
                                OnClientClick="return confirm('Desea asignar la linea solicitada a este cliente?');" />
                            <asp:LinkButton runat="server"
                                CssClass="danger-link"
                                Text="Rechazar"
                                CommandName="RechazarSolicitud"
                                CommandArgument='<%# Eval("SolicitudId") %>'
                                OnClientClick="return confirm('Desea rechazar esta solicitud?');" />
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </section>
</asp:Content>
