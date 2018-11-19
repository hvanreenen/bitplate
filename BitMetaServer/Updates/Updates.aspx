<%@ Page Title="" Language="C#" MasterPageFile="~/_masterPages/Grids.master" AutoEventWireup="true" CodeBehind="Updates.aspx.cs" Inherits="BitMetaServer.Updates.Updates" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">

    <script type="text/javascript" src="BITUPDATES.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITUPDATES.initialize();

            BITUPDATES.loadLicensedEnvironments();

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>

        <li><a href="javascript:BITUPDATES.doUpdate();" class="bitNavBarButtonAddPage">ADDLICENSE</a>
            <div>Update uitvoeren...</div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Updates uitvoeren
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <fieldset style="padding: 25px;">
        <legend></legend>
        <p>Voor uitvoeren van updates:</p>
        <ul>
            <li>Zet op de server een <u>map met naam van het versienummer</u>. De map moet staan in: <%=AppDomain.CurrentDomain.BaseDirectory %>\App_Data\Versions  </li>
            <li>Versienummer moet zijn in het <u>formaat: x.x.x.xxx</u>. Hierbij is x getal tussen 0 en 9. (bijvoorbeeld 2.0.2.012) </li>
            <li>In deze map hoeven alleen de bestanden komen te staan welke moeten worden vervangen. (dus bijvoorbeeld een .dll en/of enkele javascripts) Hou wel mappen structuur intact</li>
            <li><u>Voor database updates</u>: zet in een map een map met de naam 'DB' en daarin de bestanden met de uit te voeren updates. Moeten in formaat: .txt (In de omgeving van de licentie moeten verificatiegegevens van de database bekend zijn)</li>
            <li>Als een site <u>meerdere versies achter ligt</u>, worden automatisch <u>eerdere updates ook uitgevoerd</u>.</li>
        </ul>
        <br />
        <label>Update naar versie:
        <select id="selectVersionNumbers" onchange="BITUPDATES.loadLicensedEnvironments();"></select></label><br />
    </fieldset>
    <h2 id="noUpdatesMsg" style="display:none">Alle omgevingen zijn up-to-date</h2>
    <table id="tableLicensedEnvironments" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 20px"> <label><input type="checkbox" id="checkAll" onclick="javascript: BITUPDATES.selectAll();" checked="checked" />
        </label></td>
                <td class="bitTableColumn" style="width: 590px" data-sort-field="DomainName">Site</td>
                <td class="resellerColumn" style="width: 150px" data-sort-field="Version">Huidige versie</td>
                <!--<td class="resellerColumn" style="width: 140px" data-sort-field="FK_Company">Eigenaar</td>-->
                <td class="resellerColumn" style="width: 100px" data-sort-field="Server">Server</td>
                <td class="resellerColumn" style="width: 140px" data-sort-field="Path">Pad op server</td>
                

            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="nameColumn" ><input type="checkbox" checked="checked" class="checkLicensedEnvironment" /><input type="hidden" data-field="ID" class="siteId" /></td>
                <td class="nameColumn" data-field="DomainName"></td>
                <td class="nameColumn" data-field="Version"></td>
                <td class="nameColumn" data-field="ServerName"></td>
                <td class="nameColumn" data-field="Path"></td>
                
                
            </tr>
        </tbody>
        <tfoot>
            <tr>
                <td colspan="8">
                    <div class="bitEndTable"></div>
                </td>
            </tr>
        </tfoot>
    </table>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
</asp:Content>
