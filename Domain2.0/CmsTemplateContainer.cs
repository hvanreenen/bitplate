using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJORM;
using HJORM.Attributes;
namespace BitPlate.Domain
{
    [Persistent("TemplateContainer")]
    
    public class CmsTemplateContainer : BaseDomainSiteObject
    {

        private CmsTemplate _template;
        [Association("FK_Layout")]
        public CmsTemplate Template
        {
            get
            {
                if (_template != null && !_template.IsLoaded)
                {
                    _template.Load();
                }
                return _template;
            }
            set { _template = value; }
        }

        [NonPersistent()]
        public string TagName
        {
            get
            {
                return "[" + Name.ToUpper() + "]";
            }
        }
    }
}
