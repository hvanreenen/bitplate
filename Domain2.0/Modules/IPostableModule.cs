using BitPlate.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public interface IPostableModule
    {
        PostResult HandlePost(CmsPage page, Dictionary<string, string> FormParameters);
    }
}
