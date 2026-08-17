<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="WebCliente.Registro" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Registro cliente</title>
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body class="login-page">
    <form id="form1" runat="server" class="register-panel">
        <h1>Registro cliente</h1>
        <label>Identificacion del cliente</label>
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
        <asp:Button runat="server" ID="RegistrarButton" Text="Registrar" OnClick="RegistrarButton_Click" />
        <asp:HyperLink runat="server" NavigateUrl="~/Login.aspx" Text="Volver al login" CssClass="secondary-link" />
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
    </form>
</body>
</html>
