<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Details.master"
    AutoEventWireup="true" CodeBehind="Publish.aspx.cs" Inherits="BitSite._bitPlate.Site.Publish" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="BITPUBLISH.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            //tijdelijk uitgezet
            //tijdelijk alleen mogelijk hele site te publiceren
            //BITPUBLISH.loadUnpublishedItems();
            $('#bitAjaxLoaderContainer').fadeIn();
            $('#bitAjaxLoaderContainer').fadeOut();
            BITPUBLISH.changeEnvironment('<%=BitSite.SessionObject.CurrentSite.CurrentWorkingEnvironment.ID.ToString()%>');
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack" runat="server" id="BackLink">back</a>
            <div>Terug</div>
        </li>
        <li><a href="javascript:BITPUBLISH.publish();" class="bitBoardPublish">Publiceer
        </a>
            <div>Publiceer</div>
        </li>
        <!--<li id="liSaveConfig" runat="server"><a id="aSaveConfig" runat="server" href="javascript:BITPUBLISH.saveConfig();" class="bitNavBarButtonSavePage">Opslaan</a>
            <div>Opslaan</div>
        </li>-->
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Publiceer
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetailsPlaceHolder" runat="server">
    <br />
    <fieldset>
        <legend>Kies omgeving</legend>
        <asp:Literal runat="server" ID="literalEnvironments"></asp:Literal>
    </fieldset>

    <fieldset>
        <legend>Opties</legend>
        <!--<i>Tijdelijk is het alleen mogelijk de volgende onderdelen van een site in zijn geheel te publiceren, in plaats van afzonderlijke items</i><br />-->
        <!--<input type="checkbox" id="checkPublishComplete" checked="checked" disabled="disabled" onclick="javascript: BITPUBLISH.selectAll(this, 'CompleteSite');" />
        <label for="checkPublishComplete">
            Hele site publiceren (ongeacht of er een wijziging
            heeft plaatsgevonden)</label>
        <br />-->
        <input type="checkbox" id="checkAll" onclick="javascript: BITPUBLISH.selectAll(this, 'All');" style="display: none;" />
        <label for="checkAll" style="display: none;">
            Alles aan/uit
        </label>
        <br />
        <div id="divExternalEnvironment" style="display: none;">
            <input type="checkbox" id="checkCleanUp" style="display: none;" />
            <label id="labelCleanUp" for="checkCleanUp" style="display: none;">
                Opschonen: alles eerst verwijderen
            </label>
            <br />

            <input type="checkbox" id="checkPages" />
            <label for="checkPages">
                Publiceer Pagina's, nieuwsbrieven, templates, folders, scripts & stylesheets.
            </label>
            <br />

            <input type="checkbox" id="checkPublishFilesAndImages" />
            <label for="checkPublishFilesAndImages">
                Publiceer bestanden.
            </label>
            <br />
            <input type="checkbox" id="checkData" />
            <label for="checkData">
                Publiceer datacollecties.
            </label>
            <br />
            <input type="checkbox" id="checkPublishBin" />
            <label for="checkPublishBin" >Kopieer laatste versie van bitplate & binairies (alleen van belang als er nieuwe updates zijn geweest)</label>
        </div>
        <input type="checkbox" id="checkReGenerateSearchIndex" />
        <label for="checkReGenerateSearchIndex">Zoekindex opnieuw aanmaken (langzaam)</label>
        <br />
        <input type="checkbox" id="checkSitemap" />
        <label for="checkSitemap">Sitemap.xml & robots.txt</label>
        <br />
        <!--<br />
        <br />
        <input type="checkbox" id="checkAll" onclick="javascript: BITPUBLISH.selectAll(this, 'All');"
            checked="checked" />
        <label for="checkAll">Alles aan/uit</label>-->
    </fieldset>
    <br />
    <%-- 
    <table id="tableUnpublishedItems" class="bitGrid">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 15px"></td>
                <td class="bitTableColumn" style="width: 70px" data-sort-field="Type">Type</td>
                <td class="bitTableColumn" style="width: 150px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 100px" data-sort-field="LastPublishedDate">Laatst gepubliceerd</td>
                <td class="bitTableColumn" style="width: 50px" data-sort-field="ChangeStatus">Status
                </td>
                <td class="bitTableColumn" style="width: 100px" data-sort-field="ModifiedDate">Laatst
                    gewijzigd</td>
                <td class="bitTableColumn" style="width: 150px" data-sort-field="UserName">Door wie
                </td>
                <td class="bitTableColumn" style="width: 50px">Wijzigingen</td>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                    <input type="checkbox" checked="checked" data-field="Checked" data-title-field="FK_Object"
                        class="checkItem" />
                </td>
                <td data-field="Type"></td>
                <td class="bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                <td data-field="LastPublishedDate" data-format="dd-MM-yyyy HH:mm:ss"></td>
                <td data-field="ChangeStatusString"></td>
                <td data-field="ModifiedDate" data-format="dd-MM-yyyy HH:mm:ss"></td>
                <td data-field="UserName"></td>
                <td class="bitTableActionButton"><a href="#" onclick="javascript:BITPUBLISH.openUnpublishedChangesPopup(this);"
                    class="bitInfo">wijzigingen
                </a>
                    <div style="display: none;" data-field="Actions"></div>
                </td>
            </tr>
        </tbody>
        <tfoot>
            <tr>
                <td colspan="3" data-control-type="paging"></td>
            </tr>
        </tfoot>
    </table>
    <div class="bitEndTable"></div>
    --%>
    <div id="bitPopups">
        <!--WIJZIGINGEN INFO DIALOG -->
        <div id="bitChangesInfoDialog" title="Wijzigingen" style="display: none;">
        </div>
        <!--PUBLICEREN GEREED DIALOG -->
        <div id="bitPublishReadyDialog" title="Publiceren" style="display: none;">
            Het publiceren is gereed!
        </div>
    </div>
</asp:Content>
