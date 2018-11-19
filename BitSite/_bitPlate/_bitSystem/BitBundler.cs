using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.IO;

namespace BitSite._bitPlate
{
    public static class BitBundler
    {
        private static bool _debug;
        const string CssTemplate = "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />";
        const string JsTemplate = "<script src=\"{0}\" type=\"text/javascript\"></script>";

        public static void Init()
        {
            bool bundelScripts = false;
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["BundleScripts"], out bundelScripts);
            _debug = !bundelScripts; //Debug bool is reversed.
            
            /* CORE JS FILES */
            BundleTable.Bundles.Add(new ScriptBundle("~/BitCore").Include(
                  "~/_bitplate/_js/jquery-1.9.1.js",
                  //"~/_bitplate/_js/jquery-1.8.2.min.js",
                  "~/_bitplate/_js/jquery-ui-1.9.2.custom.min.js",
                  "~/_bitplate/_js/JSON.js",
                  "~/_bitplate/_js/BITUTILS.js",
                  "~/_bitplate/_js/BITCONSTANTS.js",
                  "~/_bitplate/_js/BITAJAX.js",
                  "~/_bitplate/_js/plugins/modernizr.custom.59383.js",
                  "~/_bitplate/_js/prototypes/centerScreen.js",
                  "~/_bitplate/_js/prototypes/databind.js",
                  "~/_bitplate/_js/prototypes/date.js",
                  "~/_bitplate/_js/prototypes/formEnrich.js",
                  "~/_bitplate/_js/prototypes/initDialog.js",
                  "~/_bitplate/_js/prototypes/string.js",
                  "~/_bitplate/_js/prototypes/tabby.js",
                  //"~/_bitplate/_js/prototypes/treeview.js",
                  "~/_bitplate/_js/prototypes/validation.js",
                  "~/_bitplate/_js/prototypes/searchable.js",
                  //"~/_bitplate/_js/_bitEditor/popup.js", 
                  "~/_bitplate/_js/plugins/jquery-ui-timepicker-addon.js",
                  "~/_bitplate/_js/BITAUTH.js",
                  "~/_bitplate/_js/global.js",
                  "~/_bitplate/_js/plugins/history.min.js"
             ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/FileUpload").Include(
                "~/_bitplate/_js/plugins/fileUpload/js/tmpl.min.js",
                "~/_bitplate/_js/plugins/fileUpload/js/load-image.min.js",
                "~/_bitplate/_js/plugins/fileUpload/js/canvas-to-blob.min.js",
                "~/_bitplate/_js/plugins/fileUpload/js/jquery.image-gallery.min.js",
                "~/_bitplate/_js/plugins/fileUpload/js/jquery.iframe-transport.js",
                "~/_bitplate/_js/plugins/fileUpload/js/jquery.fileupload.js",
                "~/_bitplate/_js/plugins/fileUpload/js/jquery.fileupload-fp.js",
                "~/_bitplate/_js/plugins/fileUpload/js/jquery.fileupload-ui.js",
                "~/_bitplate/_js/plugins/fileUpload/js/jquery.fileupload-jui.js",
                "~/_bitplate/_js/plugins/fileUpload/js/locale.js",
                "~/_bitplate/_js/plugins/fileUpload/js/main.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/ckeditor").Include(
                "~/_bitplate/_js/plugins/ckeditor/ckeditor.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/PageEditor").Include(
                "~/_bitplate/_js/plugins/ckeditor/ckeditor.js",
                "~/_bitPlate/_js/prototypes/contextMenu.js",
                "~/_bitPlate/pages/BITPAGES.js", //voor aanmaken van page-config popup
                "~/_bitPlate/Dialogs/BITAUTORISATIONTAB.js", 
                "~/_bitPlate/EditPage/BITEDITPAGE2.js",
                //"~/_bitPlate/_js/pages/popups/BITHYPERLINKSPOPUP.js",
                "~/_bitPlate/_js/prototypes/insertAtCaret.js"
            ));

            /* BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/CodeMirror").Include(
                "~/_bitplate/_js/plugins/CodeMirror-2.32/lib/codemirror.js",
                "~/_bitplate/_js/plugins/CodeMirror-2.32/lib/util/formatting.js",
                "~/_bitplate/_js/plugins/CodeMirror-2.32/mode/javascript/javascript.js",
                "~/_bitplate/_js/plugins/CodeMirror-2.32/mode/css/css.js",
                "~/_bitplate/_js/plugins/CodeMirror-2.32/mode/xml/xml.js"
            )); */

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/CodeMirror").Include(
                "~/_bitplate/_js/plugins/CodeMirror-3.14/lib/codemirror.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/mode/javascript/javascript.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/mode/css/css.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/mode/xml/xml.js",
                //"~/_bitplate/_js/plugins/CodeMirror-3.14/addon/search/search.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/search/searchcursor.js",
                //"~/_bitplate/_js/plugins/CodeMirror-3.14/addon/dialog/dialog.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/fold/foldcode.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/fold/brace-fold.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/fold/xml-fold.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/hint/show-hint.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/hint/html-hint.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/hint/xml-hint.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/hint/javascript-hint.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/lint/lint.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/lint/coffeescript-lint.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/lint/javascript-lint.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/lint/json-lint.js",
                "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/lint/jshint.js",
                "~/_bitplate/_js/plugins/CodeMirror-ui/js/codemirror-ui.js",
                "~/_bitplate/_js/plugins/CodeMirror-ui/js/codemirror-ui-find.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/jBreadCrumb").Include(
                "~/_bitplate/_js/plugins/jbreadcrump/js/jquery.easing.1.3.js",
                "~/_bitplate/_js/plugins/jbreadcrump/js/jquery.jBreadCrumb.1.1.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/jsTree").Include(
                "~/_bitplate/_js/plugins/jsTree/_lib/jquery.cookie.js",
                //"~/_bitplate/_js/plugins/jsTree/_lib/jquery.hotkeys.js",
                "~/_bitplate/_js/plugins/jsTree/jquery.jstree.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/Jcrop").Include(
                "~/_bitplate/_js/plugins/Jcrop/js/jquery.Jcrop.min.js",
                "~/_bitplate/_js/plugins/Jcrop/js/jquery.color.js"
            ));
            
            //BundleTable.Bundles.Add(new ScriptBundle("~/bitEditor").Include
            //    ("~/_bitplate/_js/_bitEditor/ToolbarButton.js",
            //    "~/_bitplate/_js/_bitEditor/ToolbarSeperator.js",
            //    "~/_bitplate/_js/_bitEditor/ToolbarDropDown.js",
            //    "~/_bitplate/_js/_bitEditor/Toolbar.js",
            //    "~/_bitplate/_js/_bitEditor/Statusbar.js",
            //    "~/_bitplate/_js/_bitEditor/Editor.js",
            //    "~/_bitplate/_js/_bitEditor/Selection.js",
            //    "~/_bitplate/_js/_bitEditor/utils/HTMLHelper.js",
            //    "~/_bitplate/_js/_bitEditor/utils/StylesheetParser.js",
            //    "~/_bitplate/_js/_bitEditor/CommandManager.js",
            //    "~/_bitplate/_js/_bitEditor/BITEDITOR.js"
            //));
            //BundleTable.Bundles.Add(new ScriptBundle("~/bitModules").IncludeDirectory("~/_bitplate/_js/_bitModules/", "*.js"));

            BundleAllThemes();

            if (!_debug)
            {
                BundleTable.EnableOptimizations = true;
            }
        }

        public static void BundleAllThemes()
        {
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "_bitplate\\_themes"))
            {
                return;
            }
            //HttpContext.Current.Server.MapPath("") 
            foreach (string dir in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "_bitplate\\_themes"))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                /* CORE CSS FILES */
                BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/_themes/" + dirInfo.Name + "/css/core").Include(
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/topMenu.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/main.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/bitPlateIcons.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/jquery-ui-1.9.1/css/custom-theme/jquery-ui-1.9.1.custom.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/form.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/formElements.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/bitPlateIcons.css"
                ));

                BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/_themes/" + dirInfo.Name + "/css/dashboard").Include(
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/dashboard.css"
                ));

                //BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/_themes/" + dirInfo.Name + "/css/editwysiwyg").Include(
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/pageEdit.css",
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/bitEditor.css",
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/formElements.css",
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/popup.css",
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/form.css"
                //));

                //BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/_themes/" + dirInfo.Name + "/css/CKeditwysiwyg").Include(
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/pageEdit.css",
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/bitCKEditor.css",
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/formElements.css",
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/popup.css",
                //    "~/_bitplate/_themes/" + dirInfo.Name + "/css/form.css"
                //));

                BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/_themes/" + dirInfo.Name + "/css/pageEdit").Include(
                    "~/_bitplate/_themes/" + dirInfo.Name + "/jquery-ui-1.9.1/css/custom-theme/jquery-ui-1.9.1.custom.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/EditPageSitebar.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/topMenu.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/pageEdit.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/popup.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/plugins/jstree/themes/default/style.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/form.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/formElements.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/bitPlateIcons.css"
                ));

                BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/_themes/" + dirInfo.Name + "/css/codemirror").Include(
                    "~/_bitplate/_js/plugins/CodeMirror-3.14/lib/codemirror.css",
                    "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/hint/show-hint.css",
                    "~/_bitplate/_js/plugins/CodeMirror-3.14/addon/lint/lint.css",
                    "~/_bitplate/_js/plugins/CodeMirror-ui/css/codemirror-ui.css",
                    "~/_bitplate/_js/plugins/CodeMirror-ui/css/codemirror-ui-find.css"
                ));
                

                BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/_themes/" + dirInfo.Name + "/css/grid").Include(
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/grid.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/formElements.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/popup.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/form.css"
                ));

                BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/_themes/" + dirInfo.Name + "/css/details").Include(
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/grid.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/formElements.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/popup.css",
                    "~/_bitplate/_themes/" + dirInfo.Name + "/css/form.css"
                ));


                //List<string> CssFiles = new List<string>();
                List<string> JsFiles = new List<string>();
                foreach (string file in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
                {
                    FileInfo fi = new FileInfo(file);
                    int i = fi.FullName.IndexOf("_themes\\" + dirInfo.Name);
                    string virtualPath = "~/_bitplate/" + fi.FullName.Substring(i).Replace("\\", "/");
                    if (fi.Extension.ToLower() == ".css")
                    {
                        //CssFiles.Add(virtualPath);
                    }
                    else if (fi.Extension.ToLower() == ".js")
                    {
                        JsFiles.Add(virtualPath);
                    }
                }
                
                //BundleTable.Bundles.Add(new StyleBundle("~/_bitplate/themes/" + dirInfo.Name + "/css").Include(CssFiles.ToArray()));
                BundleTable.Bundles.Add(new ScriptBundle("~/_bitplate/_themes/" + dirInfo.Name + "/js").Include(JsFiles.ToArray()));
            }
            
        }

        public static HtmlString ResolveBundleUrl(string bundleUrl)
        {
            return !_debug ? BundledFiles(bundleUrl) : UnbundledFiles(bundleUrl);
        }

        private static HtmlString BundledFiles(string bundleUrl)
        {
            var bundle = BundleTable.Bundles.GetBundleFor(bundleUrl);
            string bundleVirtualPath = BundleTable.Bundles.ResolveBundleUrl(bundleUrl);
            //try
            //{
                FileInfo file = bundle.EnumerateFiles(new BundleContext(new HttpContextWrapper(HttpContext.Current), BundleTable.Bundles, bundleUrl)).ToList()[0];
                if (file.Extension == ".css")
                {
                    return new HtmlString(string.Format(CssTemplate, bundleVirtualPath));
                }
                else
                {
                    return new HtmlString(string.Format(JsTemplate, bundleVirtualPath));
                }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("BitBundle exception: " + ex.Message);
            //}
            
        }

        private static HtmlString UnbundledFiles(string bundleUrl)
        {
            var bundle = BundleTable.Bundles.GetBundleFor(bundleUrl);

            StringBuilder sb = new StringBuilder();
            //var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

            foreach (var file in bundle.EnumerateFiles(new BundleContext(new HttpContextWrapper(HttpContext.Current), BundleTable.Bundles, bundleUrl)))
            {
                
                if (file.Extension == ".css")
                {
                    sb.AppendFormat(CssTemplate + Environment.NewLine, ToVirtualPath(file.FullName));
                    //sb.AppendFormat(CssTemplate + Environment.NewLine, urlHelper.Content(ToVirtualPath(file.FullName)));
                }
                else
                {
                    //sb.AppendFormat(JsTemplate + Environment.NewLine, urlHelper.Content(ToVirtualPath(file.FullName)));
                    sb.AppendFormat(JsTemplate + Environment.NewLine, ToVirtualPath(file.FullName));
                }
               
            }

            return new HtmlString(sb.ToString());
        }

        private static string ToVirtualPath(string physicalPath)
        {
            var relativePath = physicalPath.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], "");
            return relativePath.Replace("\\", "/").Insert(0, "/");
        }
    }
}