<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NPIRegistry_Implementation.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search NPI Registry</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 30px; }
        .form-inline { display: flex; align-items: center; gap: 10px; }
        .well { margin-top: 20px; padding: 15px; border: 1px solid #ddd; background: #f8f8f8; min-height: 80px; white-space: pre-wrap; word-wrap: break-word; }
        .alert { padding: 10px; border-radius: 4px; }
        .alert-danger { background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
        input[type="number"] { width: 220px; padding: 8px; }
        .btn { padding: 8px 15px; background: #007bff; color: #fff; border: none; border-radius: 4px; cursor: pointer; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Search NPI Registry</h2>

        <div class="form-inline">
            <asp:TextBox ID="txtNPI" runat="server" TextMode="Number" MaxLength="10" />
            <asp:Button ID="btnSubmit" runat="server" Text="Search" CssClass="btn" OnClick="btnSubmit_Click" />
        </div>

        <div id="resultContainer" runat="server" class="well"></div>
    </form>
</body>
</html>
