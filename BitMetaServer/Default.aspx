<%@ Page Title="" Language="C#" MasterPageFile="~/_MasterPages/Dashboard.master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BitMetaServer.Default" ViewStateMode="Disabled" %>

<asp:Content ID="Content1" ContentPlaceHolderID="SlideMenuPlaceHolder" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="divWrapperBoxes">
        <!-- bitBox01 -->
        <div class="bitBoxWrapper" id="bitBox01">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Recentelijk aangepast") %></div>
            <div class="bitBoxMain" id="bitBoxMain01">
                <strong>Licenties:</strong>
                <br />
               <asp:Literal ID="RecentLicensesList" runat="server">

                </asp:Literal>
                <br />
                
            </div>
        </div>
        <!-- .bitBox01 -->
        <!-- bitBox02 -->
        <div class="bitBoxWrapper bitBox02" id="bitBox02" runat="server">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Licentiebeheer") %></div>
            <div class="bitBoxMain" id="bitBoxMain02">
                <!-- icons -->
                <ul>
                    <li runat="server" id="liLicenses"><a href="Licenses/Licenses.aspx" class="bitBoardPages" runat="server" id="aPages">Pages</a>
                        <div><%=this.Translator.Translate("Licenties") %></div>
                        <div class="ui-bitTooltip hidden"></div>
                    </li>
                    <li runat="server" id="liCompanies"><a href="Licenses/Companies.aspx" class="bitBoardFileManager" runat="server" id="aFileManager">filemanager</a>
                        <div><%=this.Translator.Translate("Klanten") %></div>
                        <div class="ui-bitTooltip hidden">Beheer hier klanten</div>
                    </li>
                    <li runat="server" id="liResellers"><a href="Licenses/Resellers.aspx" class="bitBoardModules " runat="server" id="aDataCollections">datacollecties</a>
                        <div><%=this.Translator.Translate("Resellers") %></div>
                        <div class="ui-bitTooltip hidden">Beheer hier bitplate verkopers</div>
                    </li>
                    
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox02 -->
        <!-- bitBox03 -->
        <div class="bitBoxWrapper bitBox03" id="bitBox03" runat="server">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Berichtenbeheer") %></div>
            <div class="bitBoxMain" id="bitBoxMain03">
                <!-- icons -->
                <ul class="bitBoxMain-ul02">
                    <li runat="server" id="liNews"><a href="/news/news.aspx" class="bitBoardGeneral" runat="server" id="aSiteConfig">general</a>
                        <div><%=this.Translator.Translate("Nieuwsberichten") %></div>
                        <div class="ui-bitTooltip hidden"></div>
                    </li>
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox03 -->
        <!-- bitBox04 -->
        <div class="bitBoxWrapper bitBox04" id="bitBox04" runat="server">
            <div class="bitBoxTitle"><%=this.Translator.Translate("Versiebeheer") %></div>
            <div class="bitBoxMain" id="bitBoxMain04">
                <!-- icons -->
                <ul class="bitBoxMain-ul03">
                    <!--class="bitItemDisabled"-->



                    <li runat="server" id="liUpdates"><a href="Updates/Updates.aspx" class="bitBoardTemplates" runat="server" id="a2">updates</a>
                        <div><%=this.Translator.Translate("Updates") %></div>
                        <div class="ui-bitTooltip hidden"></div>
                    </li>
                </ul>
                <!-- .icons -->
            </div>
        </div>
        
        
        
        <!-- bitBox05 -->
        <div class="bitBoxWrapper bitBox05" id="bitBox05" runat="server">
            <div class="bitBoxTitle">Gebruikersbeheer</div>
            <div class="bitBoxMain" id="bitBoxMain08">
                <!-- icons -->
                <ul>
                    <li runat="server" id="liWebUsers"><a runat="server" id="aWebUsers" href="Auth/ServerUsers.aspx" class="bitBoardUsers">users</a>
                        <div><%=this.Translator.Translate("Metaserver admins") %></div>
                        <div class="ui-bitTooltip hidden">Maak gebruikers aan</div>
                    </li>
                    <li runat="server" id="liWebUserGroups"><a runat="server" id="aWebUserGroups" href="Auth/MultiSiteUsers.aspx" class="bitBoardAdmins">admins</a>
                        <div><%=this.Translator.Translate("Multi-site gebruikers") %></div>
                        <div class="ui-bitTooltip hidden">Koppel Bitplate gebruikers aan meer dan 1 sites</div>
                    </li>
                    
                </ul>
                <!-- .icons -->
            </div>
        </div>
        <!-- .bitBox08 -->
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContentPlaceHolder" runat="server">
</asp:Content>
