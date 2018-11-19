using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public enum SortDirectionEnum {
        ASC,
        DESC,
    }

    public interface ISortable
    {
        string DoSort(CmsPage page, string column, SortDirectionEnum sortDirection, int pageNumber, int totalResults, Dictionary<string, object> Parameters);
    }
}
