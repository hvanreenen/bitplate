using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    //public enum PostResultDataType
    //{
    //    Page = 1,
    //    Module = 2,
    //    Link = 3,
    //    Text = 4
    //}

    public class PostResult
    {
        public PostResult()
        {
            Success = true;
        }

        public PostResult(string ErrorMsg)
        {
            Success = false;
            ErrorMessage = ErrorMsg;
        }
        public bool Success { get; set; }
        private string _errMessage = "";
        public string ErrorMessage
        {
            get
            {
                return _errMessage;
            }
            set
            {
                _errMessage = value;
                if (_errMessage != String.Empty)
                {
                    Success = false;
                }
            }
        }
        public string HtmlResult { get; set; }
        //om eventueel een object in te stoppen (user na inloggen of zo), wordt niet gebruikt
        public object PostDataObject { get; set; }
        //voor redirects
        //public ModuleNavigationAction NavigationAction { get; set; }
        public NavigationTypeEnum NavigationType { get; set; }
        public string NavigationUrl { get; set; }
        public string RefreshModules { get; set; }
    }
}
