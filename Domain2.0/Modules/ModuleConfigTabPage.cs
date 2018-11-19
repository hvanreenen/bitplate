using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public class ModuleConfigTabPage
    {
        public string Name { get; set; }
        /// <summary>
        /// Tabpage wordt geladen als .aspx pagina. Dan moet Url zijn gevuld.
        /// Is Tabpage niet external dan is het vaste html binnen EditPageMenu.ascx
        /// </summary>
        public bool IsExternal { get; set; }
        public string Url { get; set; }
    }
}
