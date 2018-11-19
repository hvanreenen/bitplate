using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitPlate.Domain.Modules;
using HJORM.Attributes;

namespace BitPlate.Domain.Modules.AuthModules
{
    [Persistent("Module")]
    public class LoginModule : BaseModule
    {
        public LoginModule()
            : base()
        {
            int i = 1 + 1;
        }
    }
}