<%@ Page Title="Administradores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Administradores.aspx.cs" Inherits="WebAdministrativo.Administradores" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Mantenimiento usuario administrador</h1>
    <section class="panel">
        <asp:Button runat="server" ID="NuevoButton" Text="Nuevo" OnClick="NuevoButton_Click" />
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
        <asp:GridView runat="server" ID="AdministradoresGrid" AutoGenerateColumns="false" OnRowCommand="AdministradoresGrid_RowCommand">
            <Columns>
                <asp:BoundField HeaderText="Identificacion" DataField="Identificacion" />
                <asp:BoundField HeaderText="Nombre" DataField="Nombre" />
                <asp:BoundField HeaderText="Primer apellido" DataField="PrimerApellido" />
                <asp:BoundField HeaderText="Segundo apellido" DataField="SegundoApellido" />
                <asp:BoundField HeaderText="Correo" DataField="CorreoElectronico" />
                <asp:BoundField HeaderText="Usuario" DataField="UsuarioVisible" />
                <asp:BoundField HeaderText="Contrasena" DataField="ContrasenaVisible" />
                <asp:BoundField HeaderText="Estado" DataField="Estado" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" Text="Editar" CommandName="EditarAdmin" CommandArgument='<%# Eval("Identificacion") %>' />
                        <asp:LinkButton runat="server" Text='<%# EstadoActivo(Eval("Estado")) ? "inactivar" : "activar" %>' CommandName="CambiarEstado" CommandArgument='<%# Eval("Identificacion") + "|" + Eval("Estado") %>' />
                        <asp:LinkButton runat="server" Text="Eliminar" CommandName="EliminarAdmin" CommandArgument='<%# Eval("Identificacion") %>' OnClientClick="return confirm('Esta seguro de eliminar el registro del usuario de forma definitiva?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </section>
    <section class="panel" runat="server" id="FormularioPanel" visible="false">
        <h2>Datos del administrador</h2>
        <asp:HiddenField runat="server" ID="ModoHidden" Value="nuevo" />
        <asp:HiddenField runat="server" ID="UsuarioEncriptadoHidden" />
        <label>Identificacion</label>
        <asp:TextBox runat="server" ID="IdentificacionText" />
        <label>Nombre</label>
        <asp:TextBox runat="server" ID="NombreText" />
        <label>Primer apellido</label>
        <asp:TextBox runat="server" ID="PrimerApellidoText" />
        <label>Segundo apellido</label>
        <asp:TextBox runat="server" ID="SegundoApellidoText" />
        <label>Correo electronico</label>
        <asp:TextBox runat="server" ID="CorreoText" />
        <label>Usuario</label>
        <asp:TextBox runat="server" ID="UsuarioText" />
        <label>Contrasena</label>
        <asp:TextBox runat="server" ID="ContrasenaText" TextMode="Password" />
        <asp:Button runat="server" ID="GuardarButton" Text="Guardar" OnClick="GuardarButton_Click" />
    </section>
</asp:Content>
