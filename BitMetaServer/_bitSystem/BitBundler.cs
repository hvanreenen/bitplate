using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.IO;

namespace BitMetaServer
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
                  "~/_js/jquery-1.8.2.js",
                  "~/_js/jquery-ui-1.9.1.custom.js",
                  "~/_js/JSON.js",
                  "~/_js/BITUTILS.js",
                  "~/_js/BITCONSTANTS.js",
                  "~/_js/BITAJAX.js",
                  "~/_js/plugins/modernizr.custom.59383.js",
                  "~/_js/prototypes/centerScreen.js",
                  "~/_js/prototypes/databind.js",
                  "~/_js/prototypes/date.js",
                  "~/_js/prototypes/formEnrich.js",
                  "~/_js/prototypes/initDialog.js",
                  "~/_js/prototypes/string.js",
                  "~/_js/prototypes/tabby.js",
                  //"~/_js/prototypes/treeview.js",
                  "~/_js/prototypes/validation.js",
                  "~/_js/prototypes/searchable.js",
                  //"~/_js/_bitEditor/popup.js", 
                  "~/_js/plugins/jquery-ui-timepicker-addon.js",
                  "~/_js/BITAUTH.js",
                  "~/_js/global.js",
                  "~/_js/plugins/history.min.js"
             ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/FileUpload").Include(
                "~/_js/plugins/fileUpload/js/tmpl.min.js",
                "~/_js/plugins/fileUpload/js/load-image.min.js",
                "~/_js/plugins/fileUpload/js/canvas-to-blob.min.js",
                "~/_js/plugins/fileUpload/js/jquery.image-gallery.min.js",
                "~/_js/plugins/fileUpload/js/jquery.iframe-transport.js",
                "~/_js/plugins/fileUpload/js/jquery.fileupload.js",
                "~/_js/plugins/fileUpload/js/jquery.fileupload-fp.js",
                "~/_js/plugins/fileUpload/js/jquery.fileupload-ui.js",
                "~/_js/plugins/fileUpload/js/jquery.fileupload-jui.js",
                "~/_js/plugins/fileUpload/js/locale.js",
                "~/_js/plugins/fileUpload/js/main.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/ckeditor").Include(
                "~/_js/plugins/ckeditor/ckeditor.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/PageEditor").Include(
                "~/_js/plugins/ckeditor/ckeditor.js",
                "~/_js/prototypes/contextMenu.js",
                "~/pages/BITPAGES.js", //voor aanmaken van page-config popup
                "~/Dialogs/BITAUTORISATIONTAB.js", 
                "~/EditPage/BITEDITPAGE2.js",
                //"~/_js/pages/popups/BITHYPERLINKSPOPUP.js",
                "~/_js/prototypes/insertAtCaret.js"
            ));

            /* BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/CodeMirror").Include(
                "~/_js/plugins/CodeMirror-2.32/lib/codemirror.js",
                "~/_js/plugins/CodeMirror-2.32/lib/util/formatting.js",
                "~/_js/plugins/CodeMirror-2.32/mode/javascript/javascript.js",
                "~/_js/plugins/CodeMirror-2.32/mode/css/css.js",
                "~/_js/plugins/CodeMirror-2.32/mode/xml/xml.js"
            )); */

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/CodeMirror").Include(
                "~/_js/plugins/CodeMirror-3.14/lib/codemirror.js",
                "~/_js/plugins/CodeMirror-3.14/mode/javascript/javascript.js",
                "~/_js/plugins/CodeMirror-3.14/mode/css/css.js",
                "~/_js/plugins/CodeMirror-3.14/mode/xml/xml.js",
                //"~/_js/plugins/CodeMirror-3.14/addon/search/search.js",
                "~/_js/plugins/CodeMirror-3.14/addon/search/searchcursor.js",
                //"~/_js/plugins/CodeMirror-3.14/addon/dialog/dialog.js",
                "~/_js/plugins/CodeMirror-3.14/addon/fold/foldcode.js",
                "~/_js/plugins/CodeMirror-3.14/addon/fold/brace-fold.js",
                "~/_js/plugins/CodeMirror-3.14/addon/fold/xml-fold.js",
                "~/_js/plugins/CodeMirror-3.14/addon/hint/show-hint.js",
                "~/_js/plugins/CodeMirror-3.14/addon/hint/html-hint.js",
                "~/_js/plugins/CodeMirror-3.14/addon/hint/xml-hint.js",
                "~/_js/plugins/CodeMirror-3.14/addon/hint/javascript-hint.js",
                "~/_js/plugins/CodeMirror-3.14/addon/lint/lint.js",
                "~/_js/plugins/CodeMirror-3.14/addon/lint/coffeescript-lint.js",
                "~/_js/plugins/CodeMirror-3.14/addon/lint/javascript-lint.js",
                "~/_js/plugins/CodeMirror-3.14/addon/lint/json-lint.js",
                "~/_js/plugins/CodeMirror-3.14/addon/lint/jshint.js",
                "~/_js/plugins/CodeMirror-ui/js/codemirror-ui.js",
                "~/_js/plugins/CodeMirror-ui/js/codemirror-ui-find.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/jBreadCrumb").Include(
                "~/_js/plugins/jbreadcrump/js/jquery.easing.1.3.js",
                "~/_js/plugins/jbreadcrump/js/jquery.jBreadCrumb.1.1.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/jsTree").Include(
                "~/_js/plugins/jsTree/_lib/jquery.cookie.js",
                //"~/_js/plugins/jsTree/_lib/jquery.hotkeys.js",
                "~/_js/plugins/jsTree/jquery.jstree.js"
            ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/Jcrop").Include(
                "~/_js/plugins/Jcrop/js/jquery.Jcrop.min.js",
                "~/_js/plugins/Jcrop/js/jquery.color.js"
            ));
            
            //BundleTable.Bundles.Add(new ScriptBundle("~/bitEditor").Include
            //    ("~/_js/_bitEditor/ToolbarButton.js",
            //    "~/_js/_bitEditor/ToolbarSeperator.js",
            //    "~/_js/_bitEditor/ToolbarDropDown.js",
            //    "~/_js/_bitEditor/Toolbar.js",
            //    "~/_js/_bitEditor/Statusbar.js",
            //    "~/_js/_bitEditor/Editor.js",
            //    "~/_js/_bitEditor/Selection.js",
            //    "~/_js/_bitEditor/utils/HTMLHelper.js",
            //    "~/_js/_bitEditor/utils/StylesheetParser.js",
            //    "~/_js/_bitEditor/CommandManager.js",
            //    "~/_js/_bitEditor/BITEDITOR.js"
            //));
            //BundleTable.Bundles.Add(new ScriptBundle("~/bitModules").IncludeDirectory("~/_js/_bitModules/", "*.js"));

            BundleAllThemes();

            if (!_debug)
            {
                BundleTable.EnableOptimizations = true;
            }
        }

        public static void BundleAllThemes()
        {
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "_themes"))
            {
                return;
            }
            //HttpContext.Current.Server.MapPath("") 
            foreach (string dir in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "_themes"))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                /* CORE CSS FILES */
                BundleTable.Bundles.Add(new StyleBundle("~/_themes/" + dirInfo.Name + "/css/core").Include(
                    "~/_themes/" + dirInfo.Name + "/css/topMenu.css",
                    "~/_themes/" + dirInfo.Name + "/css/main.css",
                    "~/_themes/" + dirInfo.Name + "/css/bitPlateIcons.css",
                    "~/_themes/" + dirInfo.Name + "/jquery-ui-1.9.1/css/custom-theme/jquery-ui-1.9.1.custom.css",
                    "~/_themes/" + dirInfo.Name + "/css/form.css",
                    "~/_themes/" + dirInfo.Name + "/css/formElements.css",
                    "~/_themes/" + dirInfo.Name + "/css/bitPlateIcons.css"
                ));

                BundleTable.Bundles.Add(new StyleBundle("~/_themes/" + dirInfo.Name + "/css/dashboard").Include(
                    "~/_themes/" + dirInfo.Name + "/css/dashboard.css"
                ));

                //BundleTable.Bundles.Add(new StyleBundle("~/_themes/" + dirInfo.Name + "/css/editwysiwyg").Include(
                //    "~/_themes/" + dirInfo.Name + "/css/pageEdit.css",
                //    "~/_themes/" + dirInfo.Name + "/css/bitEditor.css",
                //    "~/_themes/" + dirInfo.Name + "/css/formElements.css",
                //    "~/_themes/" + dirInfo.Name + "/css/popup.css",
                //    "~/_themes/" + dirInfo.Name + "/css/form.css"
                //));

                //BundleTable.Bundles.Add(new StyleBundle("~/_themes/" + dirInfo.Name + "/css/CKeditwysiwyg").Include(
                //    "~/_themes/" + dirInfo.Name + "/css/pageEdit.css",
                //    "~/_themes/" + dirInfo.Name + "/css/bitCKEditor.css",
                //    "~/_themes/" + dirInfo.Name + "/css/formElements.css",
                //    "~/_themes/" + dirInfo.Name + "/css/popup.css",
                //    "~/_themes/" + dirInfo.Name + "/css/form.css"
                //));

                //BundleTable.Bundles.Add(new StyleBundle("~/_themes/" + dirInfo.Name + "/css/pageEdit").Include(
                //    "~/_themes/" + dirInfo.Name + "/jquery-ui-1.9.1/css/custom-theme/jquery-ui-1.9.1.custom.css",
                //    "~/_themes/" + dirInfo.Name + "/css/EditPageSitebar.css",
                //    "~/_themes/" + dirInfo.Name + "/css/topMenu.css",
                //    "~/_themes/" + dirInfo.Name + "/css/pageEdit.css",
                //    "~/_themes/" + dirInfo.Name + "/css/popup.css",
                //    "~/_themes/" + dirInfo.Name + "/plugins/jstree/themes/default/style.css",
                //    "~/_themes/" + dirInfo.Name + "/css/form.css",
                //    "~/_themes/" + dirInfo.Name + "/css/formElements.css",
                //    "~/_themes/" + dirInfo.Name + "/css/bitPlateIcons.css"
                //));

                BundleTable.Bundles.Add(new StyleBundle("~/_themes/" + dirInfo.Name + "/css/codemirror").Include(
                    "~/_js/plugins/CodeMirror-3.14/lib/codemirror.css",
                    "~/_js/plugins/CodeMirror-3.14/addon/hint/show-hint.css",
                    "~/_js/plugins/CodeMirror-3.14/addon/lint/lint.css",
                    "~/_js/plugins/CodeMirror-ui/css/codemirror-ui.css",
                    "~/_js/plugins/CodeMirror-ui/css/codemirror-ui-find.css"
                ));
                

                BundleTable.Bundles.Add(new StyleBundle("~/_themes/" + dirInfo.Name + "/css/grid").Include(
                    "~/_themes/" + dirInfo.Name + "/css/grid.css",
                    "~/_themes/" + dirInfo.Name + "/css/formElements.css",
                    "~/_themes/" + dirInfo.Name + "/css/popup.css",
                    "~/_themes/" + dirInfo.Name + "/css/form.css"
                ));

                BundleTable.Bundles.Add(new StyleBundle("~/_themes/" + dirInfo.Name + "/css/details").Include(
                    "~/_themes/" + dirInfo.Name + "/css/grid.css",
                    "~/_themes/" + dirInfo.Name + "/css/formElements.css",
                    "~/_themes/" + dirInfo.Name + "/css/popup.css",
                    "~/_themes/" + dirInfo.Name + "/css/form.css"
                ));


                //List<string> CssFiles = new List<string>();
                List<string> JsFiles = new List<string>();
                foreach (string file in Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories))
                {
                    FileInfo fi = new FileInfo(file);
                    int i = fi.FullName.IndexOf("_themes\\" + dirInfo.Name);
                    string virtualPath = "~/" + fi.FullName.Substring(i).Replace("\\", "/");
                    if (fi.Extension.ToLower() == ".css")
                    {
                        //CssFiles.Add(virtualPath);
                    }
                    else if (fi.Extension.ToLower() == ".js")
                    {
                        JsFiles.Add(virtualPath);
                    }
                }
                
                //BundleTable.Bundles.Add(new StyleBundle("~/themes/" + dirInfo.Name + "/css").Include(CssFiles.ToArray()));
                BundleTable.Bundles.Add(new ScriptBundle("~/_themes/" + dirInfo.Name + "/js").Include(JsFiles.ToArray()));
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