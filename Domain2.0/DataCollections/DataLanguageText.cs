using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HJORM;
using HJORM.Attributes;
namespace BitPlate.Domain.DataCollections
{
    [Persistent("DataText")]
    public class DataLanguageText : BaseDomainSiteObject
    {
        public string Language { get; set; }
        public string Title { get; set; }
        private string _intro = "";
        private string _content = "";
        private DataItem _item;
        private DataGroup _group;

        public DataLanguageText()
        {
            Title = "";
        }
        
        public string Introduction
        {
            get
            {
                return _intro;
            }
            set { _intro = Utils.HtmlHelper.RemoveHeadAndBody(value); }
        }

        public string Content
        {
            get { return _content; }
            set { _content = Utils.HtmlHelper.RemoveHeadAndBody(value); }
        }

        [Association("FK_Item")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public DataItem Item
        {
            get
            {
                if (_item != null && _item.IsLoaded)
                {
                    _item.Load();
                }
                return _item;
            }
            set { _item = value; }
        }

        [Association("FK_Group")]
        [System.Web.Script.Serialization.ScriptIgnore()]
        public DataGroup Group
        {
            get
            {
                if (_group != null && _group.IsLoaded)
                {
                    _group.Load();
                }
                return _group;
            }
            set { _group = value; }
        }

    }
}
