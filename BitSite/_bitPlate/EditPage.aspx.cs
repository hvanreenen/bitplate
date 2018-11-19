using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HJORM;
using BitPlate.Domain;
using BitPlate.Domain.DataCollections;
using BitPlate.Domain.Autorisation;
using BitPlate.Domain.Modules;

namespace BitSite._bitPlate
{
    public partial class EditPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.CheckLoginAndLicense();
            setPermissions();
            fillModulesMenu();
            fillLookupData();

        }



        private void setPermissions()
        {
            if (!SessionObject.HasPermission(FunctionalityEnum.PagesEdit))
            {
                throw new Exception("U heeft geen rechten voor de bewerking");
            }

            //if (SessionObject.HasPermission(FunctionalityEnum.ModulesGeneral))
            //{
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleBreadCrump))
            //    {
            //        liBitModuleBreadCrump.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleHTML))
            //    {
            //        liBitModuleHtml.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleInputForm))
            //    {
            //        liBitModuleInputForm.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleLink))
            //    {
            //        liBitModuleLink.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleMedia))
            //    {
            //        liBitModuleMedia.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModulePopup))
            //    {
            //        liBitModulePopup.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleSearch))
            //    {
            //        liBitModuleSearch.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleSearchResults))
            //    {
            //        liBitModuleSearchResults.Disabled = true;
            //    }
            //}
            //else
            //{
            //    liBitModuleBreadCrump.Disabled = true;
            //    liBitModuleHtml.Disabled = true;
            //    liBitModuleInputForm.Disabled = true;
            //    liBitModuleLink.Disabled = true;
            //    liBitModuleMedia.Disabled = true;
            //    liBitModulePopup.Disabled = true;
            //    liBitModuleSearch.Disabled = true;
            //    liBitModuleSearchResults.Disabled = true;
            //}

            //if (SessionObject.HasPermission(FunctionalityEnum.ModulesAuth))
            //{
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleAuthLogin))
            //    {
            //        liBitModuleAuthLogin.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleAuthLoginData))
            //    {
            //        liBitModuleAuthLoginData.Disabled = true;
            //    }
            //}
            //else
            //{
            //    liBitModuleAuthLogin.Disabled = true;
            //    liBitModuleAuthLoginData.Disabled = true;
            //}

            //if (SessionObject.HasPermission(FunctionalityEnum.ModulesDataCollections))
            //{
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleDataMainGroups))
            //    {
            //        liBitModuleDataMainGroups.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleDataGroups))
            //    {
            //        liBitModuleDataGroups.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleDataItems))
            //    {
            //        liBitModuleDataItems.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleDataGroupDetails))
            //    {
            //        liBitModuleDataGroupDetails.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleDataItemDetails))
            //    {
            //        liBitModuleDataItemDetails.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleDataTree))
            //    {
            //        liBitModuleDataTree.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleDataImage))
            //    {
            //        liBitModuleDataImage.Disabled = true;
            //    }
            //}
            //else
            //{
            //    liBitModuleDataMainGroups.Disabled = true;
            //    liBitModuleDataGroups.Disabled = true;
            //    liBitModuleDataItems.Disabled = true;
            //    liBitModuleDataGroupDetails.Disabled = true;
            //    liBitModuleDataItemDetails.Disabled = true;
            //    liBitModuleDataTree.Disabled = true;
            //    liBitModuleDataImage.Disabled = true;
            //}

            //if (SessionObject.HasPermission(FunctionalityEnum.ModulesWebshop))
            //{
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleWebshopCard))
            //    {
            //        liBitModuleWebshopCard.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleWebshopCheckout))
            //    {
            //        liBitModuleWebshopCheckout.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleWebshopCheckout))
            //    {
            //        liBitModuleWebshopCheckout.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleWebshopInvoice))
            //    {
            //        liBitModuleWebshopInvoice.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleWebshopOrderForm))
            //    {
            //        liBitModuleWebshopOrderForm.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleWebshopPayment))
            //    {
            //        liBitModuleWebshopPayment.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleWebshopProductDetails))
            //    {
            //        liBitModuleWebshopProductDetails.Disabled = true;
            //    }
            //    if (!SessionObject.HasPermission(FunctionalityEnum.ModuleWebshopProducts))
            //    {
            //        liBitModuleWebshopProducts.Disabled = true;
            //    }
            //}
            //else
            //{
            //    liBitModuleWebshopCard.Disabled = true;
            //    liBitModuleWebshopCheckout.Disabled = true;
            //    liBitModuleWebshopCheckout.Disabled = true;
            //    liBitModuleWebshopInvoice.Disabled = true;
            //    liBitModuleWebshopOrderForm.Disabled = true;
            //    liBitModuleWebshopPayment.Disabled = true;
            //    liBitModuleWebshopProductDetails.Disabled = true;
            //    liBitModuleWebshopProducts.Disabled = true;
            //}
        }

        private void fillModulesMenu()
        {
            foreach (ModuleDefinition moduleDef in SessionObject.AvailableModules)
            {
                if (moduleDef.MenuFolder == "General")
                {
                    if (HasPermissionsOnModule(moduleDef.ModuleType))
                    {
                        System.Web.UI.HtmlControls.HtmlGenericControl newLi = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                        newLi.InnerHtml = String.Format("<div class='moduleToDrag' data-module-type='{0}'>{1}</div>", moduleDef.ModuleType, moduleDef.MenuName);
                        ulBitModulesGeneral.Controls.Add(newLi);
                    }
                }
                else if (moduleDef.MenuFolder == "Data")
                {
                    if (HasPermissionsOnModule(moduleDef.ModuleType))
                    {
                        System.Web.UI.HtmlControls.HtmlGenericControl newLi = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                        newLi.InnerHtml = String.Format("<div class='moduleToDrag' data-module-type='{0}'>{1}</div>", moduleDef.ModuleType, moduleDef.MenuName);
                        ulBitModulesData.Controls.Add(newLi);
                    }
                }
                else if (moduleDef.MenuFolder == "Auth")
                {
                    if (HasPermissionsOnModule(moduleDef.ModuleType))
                    {
                        System.Web.UI.HtmlControls.HtmlGenericControl newLi = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                        newLi.InnerHtml = String.Format("<div class='moduleToDrag' data-module-type='{0}'>{1}</div>", moduleDef.ModuleType, moduleDef.MenuName);
                        ulBitModulesAuth.Controls.Add(newLi);
                    }
                }
            }
        }

        private bool HasPermissionsOnModule(string moduleType)
        {
            bool returnValue = false;
            if (moduleType == "HtmlModule")
            {
                returnValue = true;
            }
            else if (moduleType == "SearchModule" && SessionObject.HasPermission(FunctionalityEnum.ModuleSearch))
            {
                returnValue = true;
            }
            else if (moduleType == "SearchResultsModule" && SessionObject.HasPermission(FunctionalityEnum.ModuleSearchResults))
            {
                returnValue = true;
            }
            return true;
            //return returnValue;
        }

        private void fillLookupData()
        {
            //datacollecties
            string where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);
            BaseCollection<DataCollection> datacollections = BaseCollection<DataCollection>.Get(where, "Name");
            foreach (DataCollection col in datacollections)
            {
                this.SelectDataCollections.Items.Add(new ListItem(col.Name, col.ID.ToString()));
            }

            //drilldownPages
            where = String.Format("FK_Site='{0}'", SessionObject.CurrentSite.ID);
            BaseCollection<CmsPage> pages = BaseCollection<CmsPage>.Get(where, "Path, Name");
            foreach (CmsPage page in pages)
            {
                this.SelectDrillDownPage.Items.Add(new ListItem(page.RelativeUrl, page.ID.ToString()));
            }

        }
    }
}