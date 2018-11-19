using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace BitPlate.Domain.Utils
{
    public class ImageHelper
    {
        public static void ResizeImage(string fileName, int maxwidth)
        {
            //verhouding 3/4;
            int maxHeight = (int)((double) maxwidth * (3 / 4));
            ResizeImage(fileName, fileName, maxwidth, maxHeight);
        }

        public static void ResizeImage(Stream fileStream, string fileName, int maxwidth)
        {
            //verhouding 3/4;
            int maxHeight = (int)((double)maxwidth * (3 / 4));
            ResizeImage(fileStream, fileName, maxwidth, maxHeight);
        }
        public static void ResizeImage(string fileName, string newFileName, int maxwidth, int maxheight)
        {
            //string thumbnailFilePath = fileName + ".thumb.jpg";

            using (Bitmap bmp = (Bitmap) new Bitmap(fileName).Clone())
            {
                
                if (fileName == newFileName)
                {
                    File.Delete(fileName);
                    //newFileName.Replace(".jpg", "resized.jpg");
                }
                int newWidth = bmp.Size.Width;
                int newHeight = bmp.Size.Height;
                if (maxwidth != 0)
                {
                    //width ingevuld daarom resizen
                    //bepaal factor
                    double factor = 1;
                    if (bmp.Size.Width > maxwidth)
                    {
                        factor = maxwidth / (double)bmp.Size.Width;
                    }
                    else if (bmp.Size.Height > maxheight)
                    {
                        factor = maxheight / (double)bmp.Size.Height;
                    }
                    //maak size van 
                    newWidth = (int)((double)bmp.Size.Width * factor);
                    newHeight = (int)((double)bmp.Size.Height * factor);
                    if (newHeight == 0) newHeight = 1;
                    if (newWidth == 0) newWidth = 1;
                }
                Size newSize = new Size(newWidth, newHeight);

                using (Bitmap newBmp = new Bitmap((System.Drawing.Image)bmp, newSize))
                {
                    using (Graphics g = Graphics.FromImage(newBmp)) // Create Graphics object from original Image
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                        //Set Image codec of JPEG type, the index of JPEG codec is "1"
                        System.Drawing.Imaging.ImageCodecInfo codec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()[1];

                        //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                        System.Drawing.Imaging.EncoderParameters eParams = new System.Drawing.Imaging.EncoderParameters(1);
                        eParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);

                        //Now draw the image on the instance of thumbnail Bitmap object
                        g.DrawImage(bmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height));

                        //newBmp.Save(newFileName, codec, eParams);
                        newBmp.Save(newFileName, Image.FromFile(fileName).RawFormat);
                    }
                }
            }
        }

        public static void ResizeImage(Stream fileStream, string newFileName, int maxwidth, int maxheight)
        {
            ResizeImage(new Bitmap(fileStream), newFileName, maxwidth, maxheight);
        }

        public static void ResizeImage(Bitmap bmp, string newFileName, int maxwidth, int maxheight)
        {
            //string thumbnailFilePath = fileName + ".thumb.jpg";
            string fileExtension = newFileName.Substring(newFileName.Length - 3, 3).ToLower();
            using (bmp)
            {
                //bepaal factor
                int newWidth = bmp.Size.Width;
                int newHeight = bmp.Size.Height;
                if (maxwidth != 0 && bmp.Size.Width > maxwidth)
                {
                    double factor = maxwidth / (double)bmp.Size.Width; ;
                    //if (bmp.Size.Width > maxwidth)
                    //{
                    //    factor = maxwidth / (double)bmp.Size.Width;
                    //}
                    //else if (bmp.Size.Height > maxheight)
                    //{
                    //    factor = maxheight / (double)bmp.Size.Height;
                    //}
                    //maak size van 
                    newWidth = (int)((double)bmp.Size.Width * factor);
                    newHeight = (int)((double)bmp.Size.Height * factor);
                    if (newHeight == 0) newHeight = 1;
                    if (newWidth == 0) newWidth = 1;
                }
                Size newSize = new Size(newWidth, newHeight);

                using (Bitmap newBmp = new Bitmap((System.Drawing.Image)bmp, newSize))
                {
                    using (Graphics g = Graphics.FromImage(newBmp)) // Create Graphics object from original Image
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                        g.DrawImage(bmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height));
                        if (fileExtension == "jpg")
                        {
                            //Set Image codec of JPEG type, the index of JPEG codec is "1"
                            System.Drawing.Imaging.ImageCodecInfo codec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()[1];

                            //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                            System.Drawing.Imaging.EncoderParameters eParams = new System.Drawing.Imaging.EncoderParameters(1);
                            eParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);

                            newBmp.Save(newFileName, codec, eParams);
                        }
                        else
                        {
                            newBmp.Save(newFileName, Image.FromHbitmap(bmp.GetHbitmap()).RawFormat);
                        }
                    }
                }
            }
        }
        
        
        public static void CreateThumbnail(string fileName, int width)
        {
            //standaard waarde nemen
            if (width == 0) width = 80;
            using (Bitmap bmp = new Bitmap(fileName))
            {
                //bepaal factor
                string newFileName = GetThumbnailFileName(fileName);
                
                double factor = (double) width / (double)bmp.Size.Width;
                double height = factor * bmp.Size.Height;
                //if (bmp.Size.Width > bmp.Size.Height)
                //{
                //    double height = (double)width * 0.75;
                    

                //    ResizeImage(fileName, newFileName, width, (int)height);
                //}
                //else 
                //{
                //    double height = (double)width * 0.75;
                //    double newwidth = (double)height * 0.75;
                    ResizeImage(fileName, newFileName, (int)width, (int)height);
                //}
            }
            
        }

        public static string GetThumbnailFileName(string fileName)
        {
            string newFileName = fileName.ToLower().Replace(".jpg", "_thumb.jpg");
            newFileName = newFileName.Replace(".gif", "_thumb.gif");
            newFileName = newFileName.Replace(".png", "_thumb.png");
            return newFileName;
        }
        public static void CreateThumbnailOld(string fileName)
        {
            string thumbnailFilePath = fileName + ".thumb.jpg";

            //Size newSize = new Size(120, 90); // Thumbnail size (width = 120) (height = 90)

            using (Bitmap bmp = new Bitmap(fileName))
            {
                //bepaal factor
                double factor = 1;
                if(bmp.Size.Width > 120){
                    factor = 120 / (double) bmp.Size.Width;
                }
                else if (bmp.Size.Height > 90){
                    factor = 90 / (double) bmp.Size.Height;
                }
                //maak size van 
                int newWidth = (int)((double)bmp.Size.Width * factor);
                int newHeight = (int)((double)bmp.Size.Height * factor);
                Size newSize = new Size(newWidth, newHeight);
                   
                using (Bitmap thumb = new Bitmap((System.Drawing.Image)bmp, newSize))
                {
                    using (Graphics g = Graphics.FromImage(thumb)) // Create Graphics object from original Image
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;

                        //Set Image codec of JPEG type, the index of JPEG codec is "1"
                        System.Drawing.Imaging.ImageCodecInfo codec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()[1];

                        //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                        System.Drawing.Imaging.EncoderParameters eParams = new System.Drawing.Imaging.EncoderParameters(1);
                        eParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                        //Now draw the image on the instance of thumbnail Bitmap object
                        g.DrawImage(bmp, new Rectangle(0, 0, thumb.Width, thumb.Height));

                        thumb.Save(thumbnailFilePath, codec, eParams);
                    }
                }
            }


        }

        public static bool HasToBeResized(Stream stream, int maxwidth)
        {
            Bitmap bmp = new Bitmap(stream);

            return (maxwidth != 0 && bmp.Size.Width > maxwidth);
        }

        public static bool IsImage(string filename)
        {
            return (filename.ToLower().EndsWith(".jpg") ||
                filename.ToLower().EndsWith(".gif") ||
                filename.ToLower().EndsWith(".png"));
        }

        public static string ImageToBase64(Image image,
  System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static void CreateCroppedThumbnail(string Img, CmsImage CmsImg)
        {
            try
            {
                using (Image OriginalImage = Image.FromFile(Img))
                {
                    double OrginalRatioFactor = 1;
                    if (OriginalImage.Size.Width > 400 || OriginalImage.Size.Height > 400)
                    {
                        if (OriginalImage.Size.Width > OriginalImage.Size.Height)
                        {
                            OrginalRatioFactor = OriginalImage.Size.Width / (double)400;
                        }
                        else
                        {
                            OrginalRatioFactor = OriginalImage.Size.Height / (double)400;
                        }

                        CmsImg.x = (int)Math.Round(CmsImg.x * OrginalRatioFactor);
                        CmsImg.y = (int)Math.Round(CmsImg.y * OrginalRatioFactor);
                    }

                    OrginalRatioFactor = (double)CmsImg.previewZoomWidth / OriginalImage.Size.Width;
                    CmsImg.x = (int)Math.Round(CmsImg.x * OrginalRatioFactor);
                    OrginalRatioFactor = (double)CmsImg.previewZoomHeight / OriginalImage.Size.Height;
                    CmsImg.y = (int)Math.Round(CmsImg.y * OrginalRatioFactor);

                    byte[] bitImage = ResizeImageToByte(Img, CmsImg.previewZoomWidth, CmsImg.previewZoomHeight);
                    MemoryStream msImage = new MemoryStream(bitImage);
                    Image OrginalRatioImage = new Bitmap(msImage);

                    using (Bitmap bmp = new Bitmap(CmsImg.width, CmsImg.height))
                    {
                        bmp.SetResolution(OrginalRatioImage.HorizontalResolution, OrginalRatioImage.VerticalResolution);
                        using (Graphics Graphic = Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(OrginalRatioImage, new Rectangle(0, 0, CmsImg.width, CmsImg.height), CmsImg.x, CmsImg.y, CmsImg.width, CmsImg.height, GraphicsUnit.Pixel);
                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, OrginalRatioImage.RawFormat);
                            bmp.Save(GetThumbnailFileName(Img), OrginalRatioImage.RawFormat);
                            OrginalRatioImage.Dispose();
                            ms.Dispose();
                            msImage.Dispose();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }


        public static byte[] ResizeImageToByte(string fileName, int width, int height)
        {
            using (Bitmap bmp = (Bitmap)new Bitmap(fileName).Clone())
            {
                Size newSize = new Size(width, height);

                using (Bitmap newBmp = new Bitmap((System.Drawing.Image)bmp, newSize))
                {
                    using (Graphics g = Graphics.FromImage(newBmp)) // Create Graphics object from original Image
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                        //Set Image codec of JPEG type, the index of JPEG codec is "1"
                        System.Drawing.Imaging.ImageCodecInfo codec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()[1];

                        //Set the parameters for defining the quality of the thumbnail... here it is set to 100%
                        System.Drawing.Imaging.EncoderParameters eParams = new System.Drawing.Imaging.EncoderParameters(1);
                        eParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);

                        //Now draw the image on the instance of thumbnail Bitmap object
                        g.DrawImage(bmp, new Rectangle(0, 0, newBmp.Width, newBmp.Height));

                        //newBmp.Save(newFileName, codec, eParams);
                        //newBmp.Save(newFileName, Image.FromFile(fileName).RawFormat);
                        MemoryStream ms = new MemoryStream();
                        newBmp.Save(ms, bmp.RawFormat);
                        return ms.GetBuffer();
                    }
                }
            }
        }
    }
}
