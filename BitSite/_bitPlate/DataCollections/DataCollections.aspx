<%@ Page Title="" Language="C#" MasterPageFile="~/_bitPlate/_MasterPages/Grids.master"
    AutoEventWireup="true" CodeBehind="DataCollections.aspx.cs" Inherits="BitSite._bitPlate.DataCollections.DataCollections" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <script type="text/javascript" src="BITDATACOLLECTIONS.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            BITDATACOLLECTIONS.initialize();
            BITDATACOLLECTIONS.loadDataCollections('');
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuBarPlaceHolder" runat="server">
    <ul>
        <li><a href="../Default.aspx" class="bitNavBarButtonBack">back</a>
            <div>Terug</div>
        </li>
        <li id="liAddDataCollection" runat="server"><a id="aAddDataCollection" runat="server" href="javascript:BITDATACOLLECTIONS.newDataCollection();" class="bitNavBarButtonAddPage"></a>
            <div>Nieuwe datacollectie</div>
        </li>

    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="TitlePlaceHolder" runat="server">
    Datacollecties
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="GridPlaceHolder" runat="server">
    <table id="tableDataCollections" class="bitGrid" data-control-type="table" cellspacing="0"
        cellpadding="2">
        <thead>
            <tr>
                <td class="bitTableColumn" style="width: 25px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 30px">&nbsp;</td>
                <td class="bitTableColumn" style="width: 240px" data-sort-field="Name">Naam</td>
                <td class="bitTableColumn" style="width: 100px" data-sort-field="Type">Soort
                </td>
                <!--<td class="bitTableColumn" style="width: 100px" data-sort-field="DateTill">Status
                </td>-->
                <td id="tdLangTitle" runat="server" visible="false" class="bitTableColumn" style="width: 60px" data-sort-field="LanguageCode">Vaste taal</td>
                
                <td class="bitTableActionButton" id="htdDataCollectionData" runat="server">Data</td>
                <td class="bitTableActionButton" id="htdDataCollectionConfig" runat="server">Config</td>
                <td class="bitTableActionButton" id="htdDataCollectionRemove" runat="server">Verwijder</td>
                <td class="bitTableActionButton" id="htdDataCollectionCopy" runat="server">Kopieer</td>
                <td class="bitTableDateColumn" style="width: 120px;" data-sort-field="CreateDate">Aanmaakdatum
                </td>
            </tr>
        </thead>
        <tbody>
            <tr data-id-field="ID">
                <td class="icon"><a  href="#" class="bitTablePageIcon"></a></td>
                <td class="iconAuth" ><a href="#" class="bitTableAuthIcon" style="display:none"></a></td>
                <td class="bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                <td data-field="Type"></td>
                <td id="tdLanguageField" runat="server" visible="false" data-field="LanguageCode"></td>
                <td class="bitTableActionButton" id="tdDataCollectionData" runat="server"><a id="aDataCollectionData" runat="server" data-field="ID" href="DataCollectionData.aspx?datacollectionid=[data-field]"
                    class="bitEditButton"></a></td>
                <td class="bitTableActionButton" id="tdDataCollectionConfig" runat="server"><a id="aDataCollectionConfig" runat="server" data-field="ID" href="javascript:BITDATACOLLECTIONS.openDetailsPopup('[data-field]');"
                    class="bitConfigButton"></a></td>
                <td class="bitTableActionButton" id="tdDataCollectionRemove" runat="server"><a id="aDataCollectionRemove" runat="server" data-field="ID" href="javascript:BITDATACOLLECTIONS.remove('[data-field]');"
                    class="bitDeleteButton"></a></td>
                <td class="bitTableActionButton" id="tdDataCollectionCopy" runat="server"><a id="aDataCollectionCopy" runat="server" data-field="ID" data-title-field="Name" href="javascript:BITDATACOLLECTIONS.copy('[data-field]', '[data-title-field]');"
                    class="bitCopyButton"></a></td>
                <td class="bitTableDateColumn" data-field="CreateDate"></td>
            </tr>
        </tbody>
    </table>
    <div class="bitEndTable"></div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="PopupsPlaceHolder" runat="server">
    <!--DATACOLLECTION CONFIG DIALOG -->
    <div id="bitDataCollectionConfigDialog" title="Gegevensverzameling eigenschappen"
        style="display: none">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageDetails1">Algemene instellingen</a></li>
                <li><a href="#tabPageDetails2">Velden voor groep</a></li>
                <li><a href="#tabPageDetails3">Velden voor item</a></li>
            </ul>
            <!-- TAB1: Home page -->
            <div id="tabPageDetails1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de naam van de verzameling. Aan de hand van deze naam kunt u de template later herkennen."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textNaam" type="text" data-field="Name" data-validation="required" class="required" maxlength="50" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Soort</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Voeg hier de soort van de verzameling. Per soort gelden er eigen standaardvelden. De velden zijn later aan te passen."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectCollectionType" data-field="Type" data-validation="required" onchange="javascript: BITDATACOLLECTIONS.changeDataCollectionType();">
                        <option value="0">Eenvoudig</option>
                        <option value="1">Standaard</option>
                        <option value="2">Nieuws</option>
                        <option value="5">Geografisch</option>
                        <option value="3" disabled="disabled">Product</option>
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div id="divLanguage" runat="server" visible="false">
                    <div class="bitPageSettingsCollumnA">Vaste taal voor deze datacollectie</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title="Voeg hier eventueel een vaste taal in. Als u geen vaste taal kiest zijn de datacollectievelden per veld meertalig te maken.">info</span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <select id="dropdownLanguages" runat="server" data-field="LanguageCode" onchange="javascript: BITDATACOLLECTIONS.changeFixedLanguage();"></select>
                    </div>
                    <br clear="all" />
                    <!-- NEXT ROW -->
                </div>
                <div class="bitPageSettingsCollumnA">Status</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title="Nieuwe items en wijzigingen zijn pas online na publiceren. Wijzigingen in actief/inactief worden wel meteen doogevoerd. 2DO: hier duidelijkheid over krijgen."></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="ChangeStatusString"></span>
                    <br />
                    <br />
                    <div>
                        <span class="radio1" title="status">
                            <input checked="checked" type="radio" name="status" data-field="Active" value="1" /><label>Actief
                            </label>
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" name="status" data-field="Active" value="2" /><label>Actief vanaf:
                            </label>
                            <input type="date" data-field="DateFrom" class="bitDatepicker" />
                            Tot:
                        <input type="date" data-field="DateTill" class="bitDatepicker" />
                        </span>
                    </div>
                    <div>
                        <span class="radio1" title="status">
                            <input type="radio" name="status" data-field="Active" value="0" /><label>Niet actief
                            </label>
                        </span>
                    </div>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
            </div>
            <!-- TAB 2 -->
            <!-- TAB 2: Velden-->
            <!-- TAB 2 -->
            <div id="tabPageDetails2" class="bitTabPage">
                <button onclick="javascript:BITDATACOLLECTIONS.newDataField('group');">Toevoegen...</button>
                <table id="tableGroupFields" class="bitGrid" data-control-type="table" cellspacing="0"
                    cellpadding="2">
                    <thead>
                        <tr>
                            <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                            <td class="bitTableColumn" style="width: 140px">Naam</td>
                            <td class="bitTableColumn" style="width: 100px">Type
                            </td>
                            <td class="bitTableColumn" style="width: 100px">Tabpage
                            </td>
                            <td class="bitTableColumn multiLanguage" style="width: 40px" runat="server" id="tdGroupFieldMultiLanguageTitle">Meertalig?
                            </td>
                            <td class="bitTableActionButton">Bewerk</td>
                            <td class="bitTableActionButton">Verwijder</td>

                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                            <td class="bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                            <td data-field="FieldTypeString"></td>
                            <td data-field="TabPage"></td>
                            <td data-field="IsMultiLanguageFieldString" class="multiLanguage" runat="server" id="tdGroupFieldMultiLanguageRow"></td>
                            <td class="bitTableActionButton"><a data-child-field="ID" href="javascript:BITDATACOLLECTIONS.openDataFieldDetailsPopup([list-index], 'group');"
                                class="bitConfigButton"></a></td>
                            <td class="bitTableActionButton"><a data-child-field="ID" href="javascript:BITDATACOLLECTIONS.removeDataField('[data-field]', [list-index], 'group');"
                                class="bitDeleteButton"></a></td>

                        </tr>
                    </tbody>
                </table>
                <div class="bitEndTable"></div>
            </div>


            <div id="tabPageDetails3" class="bitTabPage">
                <button onclick="javascript:BITDATACOLLECTIONS.newDataField('item');">Toevoegen...</button>
                <table id="tableItemFields" class="bitGrid" data-control-type="table" cellspacing="0"
                    cellpadding="2">
                    <thead>
                        <tr>
                            <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                            <td class="bitTableColumn" style="width: 140px">Naam</td>
                            <td class="bitTableColumn" style="width: 100px">Type
                            </td>
                            <td class="bitTableColumn" style="width: 100px">Tabpage
                            </td>
                            <td class="bitTableColumn multiLanguage" style="width: 40px" runat="server" id="tdItemFieldMultiLanguageTitle">Meertalig?
                            </td>
                            <td class="bitTableActionButton">Bewerk</td>
                            <td class="bitTableActionButton">Verwijder</td>

                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                            <td class="bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>
                            <td data-field="FieldTypeString"></td>
                            <td data-field="TabPage"></td>
                            <td class="multiLanguage" data-field="IsMultiLanguageFieldString" runat="server" id="tdItemFieldMultiLanguageRow"></td>
                            <td class="bitTableActionButton"><a data-child-field="ID" href="javascript:BITDATACOLLECTIONS.openDataFieldDetailsPopup([list-index], 'item');"
                                class="bitConfigButton"></a></td>
                            <td class="bitTableActionButton"><a data-child-field="ID" href="javascript:BITDATACOLLECTIONS.removeDataField('[data-field]', [list-index], 'item');"
                                class="bitDeleteButton"></a></td>

                        </tr>
                    </tbody>
                </table>
                <div class="bitEndTable"></div>
            </div>
            
        </div>
    </div>

    <!--DATAFIELD DIALOG -->
    <div id="bitDataFieldConfigDialog" title="Veld"
        style="display: none">
        <div class="bitTabs">
            <ul>
                <li><a href="#tabPageFields1">Algemene instellingen</a></li>
            </ul>
            <!-- TAB1: Home page -->
            <div id="tabPageFields1" class="bitTabPage">
                <div class="bitPageSettingsCollumnA">Naam</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textFieldName" type="text" data-field="Name" data-validation="required" class="required" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Type</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="selectFieldType" data-field="FieldType" onchange="javascript:BITDATACOLLECTIONS.changeFieldType();">
                        <option value="1">Tekst</option>
                        <option value="2">Lange tekst</option>
                        <option value="3">Html</option>
                        <option value="4">Numeriek</option>
                        <option value="5">Bedrag</option>
                        <option value="6">Ja/Nee</option>
                        <option value="7">Keuzelijst</option>
                        <option value="8">Afbeelding</option>
                        <option value="9">Bestand</option>
                        <option value="10">Afbeeldingenlijst</option>
                        <option value="11">Bestandenlijst</option>
                        <option value="12">Datum/tijd</option>
                        <option value="13">Alleen lezen</option>
                        <option value="14">Checkbox lijst</option>
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Mapping</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <span data-field="MappingColumn"></span>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div id="divMultiLanguage" runat="server" visible="false">
                    <div class="bitPageSettingsCollumnA">Meertalig?</div>
                    <div class="bitPageSettingsCollumnB">
                        <span class="bitInfo" title=""></span>
                    </div>
                    <div class="bitPageSettingsCollumnC">
                        <input type="checkbox" data-field="IsMultiLanguageField" id="checkIsMultiLanguageField" />
                    </div>
                    <br clear="all" />
                </div>
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Tabpage tijdens datainvoer</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textFieldTabPage" type="text" data-field="TabPage" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Veldvolgorde tijdens datainvoer</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input id="textFieldOrderingNumber" type="text" data-field="OrderingNumber" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Veld regel</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <select id="FieldRuleSelector" data-field="FieldRule">
                        <option value="0"></option>
                        <option value="1">Verplicht</option>s
                        <option value="2">Alleen lezen</option>
                    </select>
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div class="bitPageSettingsCollumnA">Standaard waarde</div>
                <div class="bitPageSettingsCollumnB">
                    <span class="bitInfo" title=""></span>
                </div>
                <div class="bitPageSettingsCollumnC">
                    <input type="text" value="" data-field="DefaultValue" />
                </div>
                <br clear="all" />
                <!-- NEXT ROW -->
                <div id="divLookupValues" style="display: none">
                    <fieldset style="padding: 20px">
                        <legend>Waardes</legend>
                        <button onclick="javascript:BITDATACOLLECTIONS.newDataLookupValue();">Waarde toevoegen...</button>
                        <table id="tableLookupValues" class="bitGrid" data-control-type="table" cellspacing="0" cellpadding="2">
                            <thead>
                                <tr>
                                    <td class="bitTableColumn" style="width: 40px">&nbsp;</td>
                                    <td class="bitTableColumn" style="width: 240px">Naam</td>

                                    <td class="bitTableActionButton">Bewerk</td>
                                    <td class="bitTableActionButton">Verwijder</td>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td class="icon"><a href="#" class="bitTablePageIcon"></a></td>
                                    <td class="bitTableColumnEllipsis" data-field="Name" data-title-field="Name"></td>

                                    <td class="bitTableActionButton"><a data-child-field="ID" href="javascript:BITDATACOLLECTIONS.openLookupValuePopup([list-index]);"
                                        class="bitConfigButton"></a></td>
                                    <td class="bitTableActionButton"><a data-child-field="ID" href="javascript:BITDATACOLLECTIONS.removeLookupValue([list-index], '[data-field]');"
                                        class="bitDeleteButton"></a></td>
                                </tr>
                            </tbody>
                        </table>
                        <div class="bitEndTable"></div>
                    </fieldset>
                </div>
            </div>
        </div>
    </div>

    <!--DATALOOKUPVALUE DIALOG -->
    <div id="bitDataLookupValueDialog" title="Waarde"
        style="display: none">

        <div class="bitPageSettingsCollumnA">Naam</div>
        <div class="bitPageSettingsCollumnB">
            <span class="bitInfo" title=""></span>
        </div>
        <div class="bitPageSettingsCollumnC">
            <input type="text" data-field="Name" data-validation="required" class="required" />
        </div>
        <br clear="all" />
        <!-- NEXT ROW -->
        <div id="divExtraLanguagesLookupValues">
            <asp:Literal runat="server" ID="LiteralLookupValuesLanguages"></asp:Literal>
        </div>
    </div>


   
</asp:Content>
