<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="WebCliente.Registro" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Registro cliente</title>
    <link rel="icon" type="image/png" href="Assets/Logo.png" />
    <link rel="stylesheet" href="Styles/site.css" />
</head>
<body class="login-page">
    <form id="form1" runat="server" class="register-panel">
        <div class="login-brand">
            <img src="Assets/Logo.png" alt="Central TG" />
            <div>
                <h1>Registro cliente</h1>
                <h2>Portal cliente</h2>
            </div>
        </div>
        <div class="register-grid">
            <section>
                <h2 class="form-column-title">Datos personales</h2>
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
            </section>
            <section>
                <h2 class="form-column-title">Metodo de pago</h2>
                <label>Numero de tarjeta</label>
                <asp:TextBox runat="server" ID="NumeroTarjetaText" MaxLength="19" />
                <label>Nombre del dueno de la tarjeta</label>
                <asp:TextBox runat="server" ID="NombreTarjetaText" MaxLength="60" />
                <label>Fecha de vencimiento</label>
                <asp:TextBox runat="server" ID="FechaVencimientoText" MaxLength="5" />
                <label>Codigo de seguridad</label>
                <asp:TextBox runat="server" ID="CodigoSeguridadText" MaxLength="3" />
            </section>
        </div>
        <div class="form-actions">
            <asp:Button runat="server" ID="RegistrarButton" Text="Registrar" OnClick="RegistrarButton_Click" />
            <asp:HyperLink runat="server" NavigateUrl="~/Login.aspx" Text="Volver al login" CssClass="secondary-link" />
        </div>
        <asp:Label runat="server" ID="MensajeLabel" CssClass="message" />
    </form>
    <script>
        (function () {
            var tarjeta = document.getElementById('<%= NumeroTarjetaText.ClientID %>');
            if (tarjeta) {
                tarjeta.addEventListener('input', function () {
                    var digitos = tarjeta.value.replace(/[^0-9]/g, '').slice(0, 12);
                    var grupos = digitos.match(/.{1,4}/g) || [];
                    tarjeta.value = grupos.join('-');
                });
            }

            var vencimiento = document.getElementById('<%= FechaVencimientoText.ClientID %>');
            if (vencimiento) {
                vencimiento.addEventListener('input', function () {
                    var digitos = vencimiento.value.replace(/[^0-9]/g, '').slice(0, 4);
                    vencimiento.value = digitos.length >= 3
                        ? digitos.slice(0, 2) + '/' + digitos.slice(2)
                        : digitos;
                });
            }
        })();
    </script>
</body>
</html>
