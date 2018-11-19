using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BitPlate.Domain.Autorisation;

namespace BitPlate.Domain.Modules
{
    public class ModuleDefinition
    {
        public string FriendlyName { get; set; }
        public string ModuleType { get; set; }
        public string XmlFileLocation { get; set; }
        public string MenuFolder { get; set; }
        public string MenuName { get; set; }
        public int FunctionalityEnumNumber { get; set; }
        public bool PageProof { get; set; }
        public bool NewsletterProof { get; set; }
        public string AssemblyName { get; set; }
        public string ModuleClass { get; set; }
        public string UserControlClass { get; set; }
        public string ConfigPageUrl { get; set; }
        
    }
}
