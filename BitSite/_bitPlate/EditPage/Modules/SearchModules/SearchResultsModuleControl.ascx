<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResultsModuleControl.ascx.cs" Inherits="BitSite._bitPlate.EditPage.Modules.SearchModules.SearchResultsModuleControl" %>
<asp:Repeater ID="Repeater" runat="server">
    <ItemTemplate><%#DataBinder.Eval(Container.DataItem,"Name")%> <asp:HyperLink  ID="link" runat="server" NavigateUrl='/<%#DataBinder.Eval(Container.DataItem,"Url")%>'>klik</asp:HyperLink></ItemTemplate>
</asp:Repeater>