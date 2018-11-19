<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyOwnDataModuleConfigControl.ascx.cs" Inherits="BitSite._bitPlate._bitModules.MyOwnData.MyOwnDataModuleConfigControl" %>

<h1>Config module123</h1>
ID: <asp:Label ID="LabelModId" runat="server" EnableViewState="true"></asp:Label><asp:TextBox ID="TextBoxID" runat="server"></asp:TextBox><br />
Name: <asp:TextBox ID="TextBoxName" runat="server"></asp:TextBox> <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"/><br />
<asp:Button ID="ButtonSave" runat="server" Text="Opslaan" OnClick="ButtonSave_Click"/><asp:Button ID="ButtonCancel" runat="server" Text="Annuleer" OnClick="ButtonCancel_Click"/>

<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="View2" runat="server">
    </asp:View>
    <asp:View ID="View1" runat="server">
    </asp:View>
</asp:MultiView>