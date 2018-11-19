using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Web;

using System.IO;


namespace BitPlate.Domain.Utils
{
    
    public class GenerateScreenshotHelper
    {
        private static string _url;
        private static Bitmap _bitmap;
        private static void GenerateScreenshot()
        {
            _bitmap = GenerateScreenshot(_url, -1, -1);
        }
       

        public static void GenerateScreenshotFromHTML(Guid id, string html)
        {
            //naar tijdelijk bestand schrijven
            string path = AppDomain.CurrentDomain.BaseDirectory;
            //maak alle verwijzingen naar styles locaal
            //html = html.Replace(@"href=""", @"href=""" + path.Replace("\\", "/"));
            html = html.Replace(@"url('", @"url('file:///" + path.Replace("\\", "/"));
            html = html.Replace(@"src='", @"src='file:///" + path.Replace("\\", "/"));
            html = html.Replace(@"src=""", @"src=""file:///" + path.Replace("\\", "/"));
            html = html.Replace("../", "");
            //html = html.Replace(@"src('../", @"url('file:///" + path.Replace("\\", "/"));
            path += "_bitplate\\_temp\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = path + "template_"+ id.ToString() + ".html";

            

            File.WriteAllText(fileName, html);
            _url = "file:///" + fileName.Replace("\\", "/");
            //string host = HttpContext.Current.Request.Url.Host;
            //string port = HttpContext.Current.Request.Url.Port.ToString();
            //_url = "http://" + host + ":" + port + "/_bitplate/_temp/template_" + id.ToString() + ".html?test=123";
            //_url = "http://localhost:55488/_bitPlate/Default.aspx"; 
            //BrowserControl moet in een single thread app
            //omdat webomgeving dat niet is, moet een aparte thread worden opgestart
            //de methode GenerateScreenshot is static methode zonder variablen, deze wordt hieronder opgestart
            Thread thread = new Thread(new ThreadStart(GenerateScreenshot));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            
            //wegschrijven bitmap
            string imgpath = AppDomain.CurrentDomain.BaseDirectory;
            imgpath += "_bitplate\\_img\\screenshots\\";
            if (!Directory.Exists(imgpath))
            {
                Directory.CreateDirectory(imgpath);
            }
            ImageHelper.ResizeImage(_bitmap, imgpath + "\\template_" + id.ToString() + ".jpg", 200, 150);
 
            //_bitmap.Save(imgpath + "\\template_" + id.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            
            //tijdelijke bestand weggooien
            //File.Delete(fileName);
        }
          
        private static Bitmap GenerateScreenshot(string url, int width, int height)
        {
            // Load the webpage into a WebBrowser control
            WebBrowser wb = new WebBrowser();
            wb.ScrollBarsEnabled = false;
            wb.ScriptErrorsSuppressed = true;
            wb.Navigate(url);
            
            while (wb.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }


            // Set the size of the WebBrowser control
            wb.Width = width;
            wb.Height = height;

            if (width == -1)
            {
                // Take Screenshot of the web pages full width
                wb.Width = wb.Document.Body.ScrollRectangle.Width;
                wb.Width = 1024;
            }

            if (height == -1)
            {
                // Take Screenshot of the web pages full height
                wb.Height = wb.Document.Body.ScrollRectangle.Height;
                wb.Height = 600;
            }

            // Get a Bitmap representation of the webpage as it's rendered in the WebBrowser control
            Bitmap bitmap = new Bitmap(wb.Width, wb.Height);
            wb.DrawToBitmap(bitmap, new Rectangle(0, 0, wb.Width, wb.Height));
            wb.Dispose();

            return bitmap;
        }
    }
}
