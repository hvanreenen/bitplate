using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public interface IRefreshableModule
    {
        //returns html
        string Reload(CmsPage page, Dictionary<string, object> Parameters);
    }
}
