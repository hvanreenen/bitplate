//Extra name space
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System;
using System.Web;
using System.Web.SessionState;
using System.Linq;

namespace BitSite._bitPlate
{
    public class CaptchaImageHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string color = "000";
            string moduleID = "";

            if (context.Request.QueryString.AllKeys.Contains("moduleid"))
            {
                moduleID = context.Request.QueryString["moduleid"];
            }

            if (context.Request.QueryString.AllKeys.Contains("color"))
            {
                color = context.Request.QueryString["color"];
            }
            string availableCharters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321";
            int captchaLenght = 6;
            string captcha = "";
            Random rand = new Random();
            for (int i = 1; i <= captchaLenght; i++)
            {
                captcha += availableCharters[rand.Next(0, availableCharters.Length)];
            }

            CaptchaImageHandler captchaImage = new CaptchaImageHandler(captcha, 300, 75, color);
            context.Response.Clear();
            context.Response.ContentType = "image/png";
            // Write the image to the response stream in PNG format.
            captchaImage.Image.Save(context.Response.OutputStream, ImageFormat.Png);
            // Dispose of the CAPTCHA image object.
            captchaImage.Dispose();

            context.Session["captcha_code_" + moduleID] = captcha;
        }

        //Default Constructor 
        public CaptchaImageHandler() { }
        //property
        public string Text
        {
            get { return this.text; }
        }
        public Bitmap Image
        {
            get { return this.image; }
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }

        public string HtmlColor { get; set; }

        //Private variable
        private string text;
        private int width;
        private int height;
        private Bitmap image;
        private Random random = new Random();
        //Methods declaration
        public CaptchaImageHandler(string s, int width, int height, string htmlColor = "")
        {
            this.text = s;
            this.HtmlColor = htmlColor;
            this.SetDimensions(width, height);
            this.GenerateImage();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                this.image.Dispose();
        }
        private void SetDimensions(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", width,
                    "Argument out of range, must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", height,
                    "Argument out of range, must be greater than zero.");
            this.width = width;
            this.height = height;
        }
        private void GenerateImage()
        {
            Bitmap bitmap = new Bitmap
              (this.width, this.height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, this.width, this.height);
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.LargeCheckerBoard,
                Color.LightGray, Color.White);
            g.FillRectangle(hatchBrush, rect);
            SizeF size;
            float fontSize = rect.Height + 1;
            Font font;

            do
            {
                fontSize--;
                font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
                size = g.MeasureString(this.text, font);
            } while (size.Width > rect.Width);
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            GraphicsPath path = new GraphicsPath();
            //path.AddString(this.text, font.FontFamily, (int) font.Style, 
            //    font.Size, rect, format);
            path.AddString(this.text, font.FontFamily, (int)font.Style, font.SizeInPoints, rect, format);
            float v = 4F;
            PointF[] points =
          {
                new PointF(this.random.Next(rect.Width) / v, this.random.Next(
                   rect.Height) / v),
                new PointF(rect.Width - this.random.Next(rect.Width) / v, 
                    this.random.Next(rect.Height) / v),
                new PointF(this.random.Next(rect.Width) / v, 
                    rect.Height - this.random.Next(rect.Height) / v),
                new PointF(rect.Width - this.random.Next(rect.Width) / v,
                    rect.Height - this.random.Next(rect.Height) / v)
          };
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            if (HtmlColor == "" || HtmlColor == null) HtmlColor = "000";
            hatchBrush = new HatchBrush(HatchStyle.Percent10, ColorTranslator.FromHtml("#" + HtmlColor), ColorTranslator.FromHtml("#" + HtmlColor));
            g.FillPath(hatchBrush, path);
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = this.random.Next(rect.Width);
                int y = this.random.Next(rect.Height);
                int w = this.random.Next(m / 50);
                int h = this.random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }
            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();
            this.image = bitmap;
        }
    }
}
