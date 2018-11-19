using BitPlate.Domain.Menu;
using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules.Menu
{
    [Persistent("Module")]
    public class MenuModuleV2 : BaseModule
    {
        public MenuModuleV2()
        {
            ContentSamples.Add(@"
<ul class=""bitMenu"">
	<!--{MainMenuItems}-->
	<li class=""bitMainMenuItem""><a href=""{RootItem.Url}"" target=""{RootItem.Target}"" >{RootItem.Name}</a></li>
	<!--{SubMenu}-->
    <ul>
		<!--{SubMenuItems}-->
		<li class=""bitSubMenuItem""><a href=""{SubItem.Url}"" target=""{SubItem.Target}""  >{SubItem.Name}</a></li>
		<!--{/SubMenuItems}-->
	</ul>
    <!--{/SubMenu}-->
	<!--{/MainMenuItems}-->
</ul>");
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Menu", IsExternal = true, Url = "/_bitplate/EditPage/Modules/Menu/MenuModuleTab.aspx" });
            this.CrossPagesMode = CrossPagesModeEnum.VisibleOnAllPagesInTemplate;
            string css = @"
.bitMainMenuItem {

}
.bitSubMenuItem {
    
}
";
            this.Settings.Add("MenuCss", css);

            string js = @"
$(function() {
    $( '.bitMenu' ).menu();
  });
";
            this.Settings.Add("MenuJs", js);
        }

        protected override void setNavigationActions()
        {

        }

        public override void SetAllTags()
        {
            this._tags.Add(new Tag() { Name = "<!--{MainMenuItems}-->", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "<!--{SubMenu}-->", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "<!--{SubMenuItems}-->", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "{MainMenuItems}", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "{SubMenuItems}", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "{SubMenu}", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });

            this._tags.Add(new Tag() { Name = "{RootItem.Name}" });
            this._tags.Add(new Tag() { Name = "{RootItem.Title}" });
            this._tags.Add(new Tag() { Name = "{RootItem.Url}" });
            this._tags.Add(new Tag() { Name = "{RootItem.Target}" });
            this._tags.Add(new Tag() { Name = "{RootItem.Link}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{RootItem.Image}" });
            this._tags.Add(new Tag() { Name = "{RootItem.ImageUrl}" });
            this._tags.Add(new Tag() { Name = "{RootItem.ImageHoverUrl}" });
            this._tags.Add(new Tag() { Name = "{RootItem.ImageActiveUrl}" });

            this._tags.Add(new Tag() { Name = "{SubItem.Name}" });
            this._tags.Add(new Tag() { Name = "{SubItem.Title}" });
            this._tags.Add(new Tag() { Name = "{SubItem.Url}" });
            this._tags.Add(new Tag() { Name = "{SubItem.Target}" });
            this._tags.Add(new Tag() { Name = "{SubItem.Link}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{SubItem.Image}" });
            this._tags.Add(new Tag() { Name = "{SubItem.ImageUrl}" });
            this._tags.Add(new Tag() { Name = "{SubItem.ImageHoverUrl}" });
            this._tags.Add(new Tag() { Name = "{SubItem.ImageActiveUrl}" });
        }

        public CmsMenu GetMenu()
        {
            CmsMenu menu = null;
            string menuId = base.getSetting<string>("FK_Menu");
            if (menuId != null && menuId != "")
            {
                menu = BaseObject.GetById<CmsMenu>(new Guid(menuId));
            }
            return menu;
        }

        public override string Publish2(CmsPage page)
        {
            string html = "";
            CmsMenu menu = GetMenu();
            if (menu == null)
            {
                this.Content = "Kies eerst een menu";
                html = base.Publish2(page);
            }
            else
            {
                string js = getSetting<string>("MenuJs");
                string css = getSetting<string>("MenuCss");
                if (js != null && js != String.Empty)
                {
                    this.Content = "<script type='text/javascript'>" + js + "</script>\r\n" + this.Content;
                }
                if (css != null && css != String.Empty)
                {
                    this.Content = "<style>" + css + "</style>\r\n" + this.Content;
                }
                html = base.Publish2(page);
                string mainItemsHtml = "";
                string mainItemTemplate = base.GetSubTemplate("{MainMenuItems}");
                string subMenuTemplate = base.GetSubTemplate("{SubMenu}");
                string subItemTemplate = base.GetSubTemplate("{SubMenuItems}");
                foreach (CmsMenuItem mainItem in menu.GetParentMenuItems())
                {
                    string mainItemHtml = mainItemTemplate;
                    mainItemHtml = mainItemHtml.Replace("{RootItem.Name}", mainItem.Name);
                    mainItemHtml = mainItemHtml.Replace("{RootItem.Title}", mainItem.Title);
                    mainItemHtml = mainItemHtml.Replace("{RootItem.Url}", mainItem.Url);
                    mainItemHtml = mainItemHtml.Replace("{RootItem.Target}", mainItem.Target);
                    mainItemHtml = mainItemHtml.Replace("{RootItem.Link}", createHyperLink(mainItem));
                    mainItemHtml = mainItemHtml.Replace("{/RootItem.Link}", "</a>");
                    mainItemHtml = mainItemHtml.Replace("{RootItem.ImageUrl}", mainItem.ImageUrl);
                    mainItemHtml = mainItemHtml.Replace("{RootItem.ImageHoverUrl}", mainItem.ImageHoverUrl);
                    mainItemHtml = mainItemHtml.Replace("{RootItem.ImageActiveUrl}", mainItem.ImageActiveUrl);
                    string subItemsHtml = GetSubItemsHtml(subMenuTemplate, subItemTemplate, mainItem);
                    mainItemHtml = mainItemHtml.Replace("<!--{SubMenu}-->" + subMenuTemplate + "<!--{/SubMenu}-->", subItemsHtml);
                    mainItemHtml = mainItemHtml.Replace("{SubMenu}" + subMenuTemplate + "{/SubMenu}", subItemsHtml);

                    mainItemsHtml += mainItemHtml;
                }
                html = html.Replace("<!--{MainMenuItems}-->" + mainItemTemplate + "<!--{/MainMenuItems}-->", mainItemsHtml);
                html = html.Replace("{MainMenuItems}" + mainItemTemplate + "{/MainMenuItems}", mainItemsHtml);
            }
            return html;
        }



        private string GetSubItemsHtml(string subMenuTemplate, string subItemTemplate, CmsMenuItem mainItem)
        {
            string subItemsHtml = "";
            string subMenuHtml = subMenuTemplate;
            foreach (CmsMenuItem subItem in mainItem.GetSubMenuItems())
            {
                string subItemHtml = subItemTemplate;
                subItemHtml = subItemHtml.Replace("{SubItem.Name}", subItem.Name);
                subItemHtml = subItemHtml.Replace("{SubItem.Title}", subItem.Title);
                subItemHtml = subItemHtml.Replace("{SubItem.Url}", subItem.Url);
                subItemHtml = subItemHtml.Replace("{SubItem.Target}", subItem.Target);
                subItemHtml = subItemHtml.Replace("{SubItem.Link}", createHyperLink(subItem));
                subItemHtml = subItemHtml.Replace("{/SubItem.Link}", "</a>");
                subItemHtml = subItemHtml.Replace("{SubItem.ImageUrl}", mainItem.ImageUrl);
                subItemHtml = subItemHtml.Replace("{SubItem.ImageHoverUrl}", mainItem.ImageHoverUrl);
                subItemHtml = subItemHtml.Replace("{SubItem.ImageActiveUrl}", mainItem.ImageActiveUrl);
                string subSubItemsHtml = GetSubItemsHtml(subMenuTemplate, subItemTemplate, subItem);
                if (subSubItemsHtml != "")
                {
                    subMenuHtml = subMenuHtml.Replace("<!--{SubMenuItems}-->" + subItemTemplate + "<!--{/SubMenuItems}-->", subSubItemsHtml);
                    subMenuHtml = subMenuHtml.Replace("{SubMenuItems}" + subItemTemplate + "{/SubMenuItems}", subSubItemsHtml);
                    subItemHtml += subMenuHtml;
                }
                subItemsHtml += subItemHtml;
            }
            if (subItemsHtml != "")
            {
                subMenuHtml = subMenuHtml.Replace("<!--{SubMenuItems}-->" + subItemTemplate + "<!--{/SubMenuItems}-->", subItemsHtml);
                subMenuHtml = subMenuHtml.Replace("{SubMenuItems}" + subItemTemplate + "{/SubMenuItems}", subItemsHtml);
                    
            }
            else {
                subMenuHtml = "";
            }
            return subMenuHtml;
        }

        private string createHyperLink(CmsMenuItem menuItem)
        {
            string html = "";
            if (menuItem.Target != "" || menuItem.Target != "_self")
            {
                html = String.Format("<a href='{0}' title='{1}' target='{2}' >", menuItem.Url, menuItem.Title, menuItem.Target);
            }
            else
            {
                html = String.Format("<a href='{0}' title='{1}'>", menuItem.Url, menuItem.Title);
            }
            return html;

        }

    }
}
