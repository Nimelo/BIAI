using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication;

namespace BitmapHelper
{
    public static class BitmapExtension
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using(var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using(var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Bitmap ResizeImage(this Image image, Size size)
        {
            return ResizeImage(image, size.Width, size.Height);
        }

        public static IEnumerable<Bitmap> ResizeImages(this IEnumerable<Bitmap> images, Size size)
        {
            return ResizeImages(images, size.Width, size.Height);
        }

        public static IEnumerable<Bitmap> ResizeImages(this IEnumerable<Bitmap> images, int width, int height)
        {
            foreach(var item in images)
            {
                yield return item.ResizeImage(width, height);
            }
        }


        public static IEnumerable<IEnumerable<Bitmap>> ShredImages(this IEnumerable<Bitmap> images, int dimension)
        {
            foreach(var item in images)
            {
                yield return item.ShredImage(dimension);
            }
        }

        /// <summary>
        /// Shreds image to squares from left to right.
        /// </summary>
        /// <param name="image">The image to shred.</param>
        /// <param name="dimension">Demension of square.</param>
        /// <returns>Shredded images</returns>
        public static IEnumerable<Bitmap> ShredImage(this Bitmap image, int dimension)
        {
            if(image.Height % dimension != 0
                || image.Width % dimension != 0)
                throw new ShredImageException();

            for(int x = 0; x < image.Width; x += dimension)
            {
                for(int y = 0; y < image.Height; y += dimension)
                {
                    Rectangle cropArea = new Rectangle(x, y, dimension, dimension);
                    yield return ( (Bitmap)image ).Clone(cropArea, image.PixelFormat);
                }
            }
        }

        public static Color AverageColor(this Bitmap image)
        {

            int r = 0, g = 0, b = 0;
            for(int x = 0; x < image.Width; x++)
            {
                for(int y = 0; y < image.Height; y++)
                {
                    Color tmpColor = ( (Bitmap)image ).GetPixel(x, y);

                    r += tmpColor.R;
                    g += tmpColor.G;
                    b += tmpColor.B;
                }
            }

            int amountOfPixels = image.Width * image.Height;

            b = b / amountOfPixels;
            g = g / amountOfPixels;
            r = r / amountOfPixels;

            return Color.FromArgb(r, g, b);
        }

        public static Color AverageColorFast(this Bitmap image)
        {

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            System.Drawing.Imaging.BitmapData imageData =
                image.LockBits(rect,
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    image.PixelFormat);

            IntPtr ptr = imageData.Scan0;

            int bytes = imageData.Stride * image.Height;
            byte[] rgbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr,
                           rgbValues, 0, bytes);

            int r = 0, g = 0, b = 0;

            for(int x = 0; x < image.Width; x++)
            {
                for(int y = 0; y < image.Height; y++)
                {
                    int position = ( y * imageData.Stride ) + ( x * Image.GetPixelFormatSize(imageData.PixelFormat) / 8 );
                    b += rgbValues[position];
                    g += rgbValues[position + 1];
                    r += rgbValues[position + 2];
                }
            }

            image.UnlockBits(imageData);

            int amountOfPixels = image.Width * image.Height;

            b = b / amountOfPixels;
            g = g / amountOfPixels;
            r = r / amountOfPixels;

            return Color.FromArgb(r, g, b);
        }

        public static Size AverageSize(this IEnumerable<Image> images)
        {
            Size size = new Size();

            foreach(var item in images)
            {
                size += item.Size;
            }

            size.Height /= images.Count();
            size.Width /= images.Count();

            return size;
        }

    }
}
