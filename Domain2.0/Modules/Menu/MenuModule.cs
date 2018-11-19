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
    public class MenuModule : BaseModule
    {
        public MenuModule()
        {
            ContentSamples.Add(@"
<!--{Menu}-->
<ul>
	<!--{MenuItems}-->
	<li><a href=""{MenuItem.Url}"" target=""{MenuItem.Target}"" class=""{MenuItem.CssClass}"">{MenuItem.Name}</a>
    {MenuItem.ChildItems}
    </li>
	<!--{/MenuItems}-->
</ul>
<!--{/Menu}-->");
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "General" });
            ConfigPageTabPages.Add(new ModuleConfigTabPage { Name = "Menu", IsExternal = true, Url = "/_bitplate/EditPage/Modules/Menu/MenuModuleTab.aspx" });
            this.CrossPagesMode = CrossPagesModeEnum.VisibleOnAllPagesInTemplate;

        }

        protected override void setNavigationActions()
        {

        }

        public override void SetAllTags()
        {
            //this._tags.Add(new Tag() { Name = "<!--{MainMenuItems}-->", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "<!--{Menu}-->", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "<!--{MenuItems}-->", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "{Menu}", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });
            this._tags.Add(new Tag() { Name = "{MenuItems}", HasCloseTag = true, ReplaceValue = "MANUAL REPLACEMENT" });

            this._tags.Add(new Tag() { Name = "{MenuItem.Name}" });
            this._tags.Add(new Tag() { Name = "{MenuItem.Title}" });
            this._tags.Add(new Tag() { Name = "{MenuItem.Url}" });
            this._tags.Add(new Tag() { Name = "{MenuItem.Target}" });
            this._tags.Add(new Tag() { Name = "{MenuItem.CssClass}" });
            this._tags.Add(new Tag() { Name = "{MenuItem.Link}", HasCloseTag = true });
            this._tags.Add(new Tag() { Name = "{MenuItem.Image}" });
            this._tags.Add(new Tag() { Name = "{MenuItem.ImageUrl}" });
            this._tags.Add(new Tag() { Name = "{MenuItem.ImageHoverUrl}" });
            this._tags.Add(new Tag() { Name = "{MenuItem.ImageActiveUrl}" });
        }
        private CmsMenu _menu;
        public CmsMenu GetMenu()
        {
            if (_menu == null)
            {
                string menuId = base.getSetting<string>("FK_Menu");
                if (menuId != null && menuId != "")
                {
                    _menu = BaseObject.GetById<CmsMenu>(new Guid(menuId));
                }
            }
            return _menu;
        }

        [NonPersistent()]
        public override List<string> IncludeScripts
        {
            get
            {

                CmsMenu menu = GetMenu();
                if (menu != null)
                {
                    _includeScripts = new List<string>();
                    foreach (CmsScript script in menu.Scripts)
                    {
                        _includeScripts.Add(script.Url);
                    }
                }

                return _includeScripts;
            }
            set
            {
                base.IncludeScripts = value;
            }
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
                html = base.Publish2(page);
                string menuItemsHtml = "";
                string menuTemplate = base.GetSubTemplate("{Menu}");
                string menuItemTemplate = base.GetSubTemplate("{MenuItems}");
                foreach (CmsMenuItem mainItem in menu.GetParentMenuItems())
                {
                    string menuItemHtml = menuItemTemplate;
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.Name}", mainItem.Name);
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.Title}", mainItem.Title);
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.Url}", mainItem.Url);
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.Target}", mainItem.Target);
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.CssClass}", mainItem.CssClass);
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.Link}", createHyperLink(mainItem));
                    menuItemHtml = menuItemHtml.Replace("{/MenuItem.Link}", "</a>");
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.ImageUrl}", mainItem.ImageUrl);
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.ImageHoverUrl}", mainItem.ImageHoverUrl);
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.ImageActiveUrl}", mainItem.ImageActiveUrl);
                    string subItemsHtml = GetSubItemsHtml(menuTemplate, menuItemTemplate, mainItem);
                    menuItemHtml = menuItemHtml.Replace("{MenuItem.ChildItems}", subItemsHtml);

                    menuItemsHtml += menuItemHtml;
                }
                html = html.Replace("<!--{MenuItems}-->" + menuItemTemplate + "<!--{/MenuItems}-->", menuItemsHtml);
                //html = html.Replace("{Menu}" + menuTemplate + "{/Menu}", menuItemsHtml);
                string cssClass = "bitMenu";
                int animationspeed = getSetting<int>("AnimationSpeed");
                string menuType = "";
                if (menu.Type == MenuTypeEnum.Accordion) cssClass = "bitAccordionMenu";
                if (menu.Type == MenuTypeEnum.HorizontalDropDown) menuType = "dropdownHorizontal";
                if (menu.Type == MenuTypeEnum.VerticalDropDown) menuType = "dropdownVertical";

                html = html.Replace("<!--{Menu}-->", "<div class='" + cssClass + "' data-menu-type='" + menuType + "' data-menu-animationspeed='" + animationspeed + "'>");
                html = html.Replace("<!--{/Menu}-->", "</div>");
                html = html.Replace("{Menu}", "<div class='bitMenu'>");
                html = html.Replace("{/Menu}", "</div>");
            }
            return html;
        }



        private string GetSubItemsHtml(string menuTemplate, string menuItemTemplate, CmsMenuItem parentItem)
        {
            string subItemsHtml = "";
            string subMenuHtml = menuTemplate;
            foreach (CmsMenuItem subItem in parentItem.GetSubMenuItems())
            {
                string subItemHtml = menuItemTemplate;
                subItemHtml = subItemHtml.Replace("{MenuItem.Name}", subItem.Name);
                subItemHtml = subItemHtml.Replace("{MenuItem.Title}", subItem.Title);
                subItemHtml = subItemHtml.Replace("{MenuItem.Url}", subItem.Url);
                subItemHtml = subItemHtml.Replace("{MenuItem.Target}", subItem.Target);
                subItemHtml = subItemHtml.Replace("{MenuItem.CssClass}", subItem.CssClass);
                subItemHtml = subItemHtml.Replace("{MenuItem.Link}", createHyperLink(subItem));
                subItemHtml = subItemHtml.Replace("{/MenuItem.Link}", "</a>");
                subItemHtml = subItemHtml.Replace("{MenuItem.ImageUrl}", parentItem.ImageUrl);
                subItemHtml = subItemHtml.Replace("{MenuItem.ImageHoverUrl}", parentItem.ImageHoverUrl);
                subItemHtml = subItemHtml.Replace("{MenuItem.ImageActiveUrl}", parentItem.ImageActiveUrl);
                string subSubItemsHtml = GetSubItemsHtml(menuTemplate, menuItemTemplate, subItem);
                subItemHtml = subItemHtml.Replace("{MenuItem.ChildItems}", subSubItemsHtml);
                //if (subSubItemsHtml != "")
                //{
                //    subMenuHtml = subMenuHtml.Replace("<!--{MenuItems}-->" + menuItemTemplate + "<!--{/MenuItems}-->", subSubItemsHtml);
                //    subMenuHtml = subMenuHtml.Replace("{MenuItems}" + menuItemTemplate + "{/MenuItems}", subSubItemsHtml);
                //    subItemHtml += subMenuHtml;
                //}
                subItemsHtml += subItemHtml;
            }
            if (subItemsHtml != "")
            {
                subMenuHtml = subMenuHtml.Replace("<!--{MenuItems}-->" + menuItemTemplate + "<!--{/MenuItems}-->", subItemsHtml);
                subMenuHtml = subMenuHtml.Replace("{MenuItems}" + menuItemTemplate + "{/MenuItems}", subItemsHtml);

            }
            else
            {
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
