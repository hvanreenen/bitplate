using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BitSite._bitPlate;
using System.Drawing.Imaging;
using System.Web.Services;
using System.Web.Script.Services;

namespace BitSite._bitAjaxServices
{
    public partial class Captcha : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string color = "000";
            string moduleID = "";

            if (Request.QueryString.AllKeys.Contains("moduleid"))
            {
                moduleID = Request.QueryString["moduleid"];
            }

            if (Request.QueryString.AllKeys.Contains("color"))
            {
                color = Request.QueryString["color"];
            }
            string availableCharters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321";
            int captchaLenght = 6;
            string captcha = "";
            Random rand = new Random();
            for (int i = 1; i <= captchaLenght; i++)
            {
                captcha += availableCharters[rand.Next(0, availableCharters.Length)];
            }

            CaptchaImage captchaImage = new CaptchaImage(captcha, 300, 75, color);
            Response.Clear();
            Response.ContentType = "image/jpeg";
            // Write the image to the response stream in JPEG format.
            captchaImage.Image.Save(Response.OutputStream, ImageFormat.Jpeg);
            // Dispose of the CAPTCHA image object.
            captchaImage.Dispose();

            Session["captcha_code_" + moduleID] = captcha;
        }
    }
}