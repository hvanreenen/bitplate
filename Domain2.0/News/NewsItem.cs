using HJORM;
using HJORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.News
{
    [DataConnection("licenseDB")]
    public class NewsItem : BaseObject
    {
        public string Message { get; set; }
        public string BitplateVersion { get; set; }
    }
}
