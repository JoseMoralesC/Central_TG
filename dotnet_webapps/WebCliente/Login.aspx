<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebCliente.Login" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Ingreso cliente</title>
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body class="login-page">
    <form id="form1" runat="server" class="login-panel">
        <h1>Central Telefonica</h1>
        <h2>Clientes</h2>
        <label>Usuario</label>
        <asp:TextBox runat="server" ID="UsuarioText" />
        <label>Contrasena</label>
        <asp:TextBox runat="server" ID="ContrasenaText" TextMode="Password" />
        <asp:Button runat="server" ID="IngresarButton" Text="Ingresar" OnClick="IngresarButton_Click" />
        <asp:HyperLink runat="server" NavigateUrl="~/Registro.aspx" Text="Registrarse" CssClass="secondary-link" />
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
    </form>
</body>
</html>
