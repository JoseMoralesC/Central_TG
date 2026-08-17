<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebAdministrativo.Login" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <title>Ingreso administrativo</title>
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body class="login-page">
    <form id="form1" runat="server" class="login-panel">
        <h1>Central Telefonica</h1>
        <h2>Administrativo</h2>
        <label>Usuario</label>
        <asp:TextBox runat="server" ID="UsuarioText" />
        <label>Contrasena</label>
        <asp:TextBox runat="server" ID="ContrasenaText" TextMode="Password" />
        <asp:Button runat="server" ID="IngresarButton" Text="Ingresar" OnClick="IngresarButton_Click" />
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
    </form>
</body>
</html>
