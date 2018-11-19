using BitPlate.Domain.Modules.Auth;
using HJORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitSite._bitPlate.EditPage.Modules.AuthModules
{
    public class BaseAuthModuleUserControl : BaseModuleUserControl
    {
        //uitgezet, want module-seetings kun je uit usercontrol-parameters halen, scheelt extra call aan DB
        //protected BaseAuthModule authModule;

        //protected override void LoadModule()
        //{
        //    authModule = BaseObject.GetById<BaseAuthModule>(new Guid(ModuleID));
        //}
    }
}