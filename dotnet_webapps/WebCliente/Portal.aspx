<%@ Page Title="Portal cliente" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Portal.aspx.cs" Inherits="WebCliente.Portal" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1><asp:Literal runat="server" ID="TituloLiteral" /></h1>
    <section class="panel">
        <asp:Label runat="server" ID="MensajeLabel" />
    </section>
</asp:Content>
