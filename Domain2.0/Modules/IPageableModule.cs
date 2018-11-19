using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public interface IPageableModule
    {
        string CreatePager(int currentPage, int totalResults, int pageSize);

        string DoPaging(CmsPage page, int pageNumber, int totalResults, Dictionary<string, object> Parameters);
    }
}
