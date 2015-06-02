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


        public static IEnumerable<Bitmap> ShredImages(this Bitmap image, int dimension)
        {
            return image.ShredImage(dimension);
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


            for(int y = 0; y < image.Height; y += dimension)
            {
                for(int x = 0; x < image.Width; x += dimension)
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
        public static Bitmap MakeGrayscale3(this Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][] 
      {
         new float[] {.3f, .3f, .3f, 0, 0},
         new float[] {.59f, .59f, .59f, 0, 0},
         new float[] {.11f, .11f, .11f, 0, 0},
         new float[] {0, 0, 0, 1, 0},
         new float[] {0, 0, 0, 0, 1}
      });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
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

        public static Bitmap Merge(this IEnumerable<Image> images, Size desinationSize, Size elementSize)
        {
            Bitmap returnBitmap = new Bitmap(desinationSize.Width, desinationSize.Height);

            int el = 0;
            using(var graphics = Graphics.FromImage(returnBitmap))
            {
                for(int y = 0; y < desinationSize.Height; y += elementSize.Height)
                {
                    for(int x = 0; x < desinationSize.Width; x += elementSize.Width)
                    {
                        Bitmap tmp = (Bitmap)images.ElementAt(el++).Clone();
                        using(var grap = Graphics.FromImage(tmp))
                        {
                           // grap.DrawRectangle(new Pen(Brushes.Red, 5), new Rectangle(0, 0, tmp.Width, tmp.Height));
                        }
                        graphics.DrawImage(tmp, x, y);
                    }
                }
            }


            return returnBitmap;
        }

        public static IEnumerable<Bitmap> Average(this IEnumerable<Bitmap> images)
        {
            foreach(var img in images)
            {
                Color color = img.AverageColorFast();
                Bitmap bmp = new Bitmap(img.Width, img.Height);

                using(var g = Graphics.FromImage(bmp))
                {
                    g.FillRectangle(new SolidBrush(color), 0, 0, bmp.Width, bmp.Height);
                }

                yield return bmp;
            }
        }

        #region threshould
        // function is used to compute the q values in the equation
        private static float Px(int init, int end, int[] hist)
        {
            int sum = 0;
            int i;
            for(i = init; i <= end; i++)
                sum += hist[i];

            return (float)sum;
        }

        // function is used to compute the mean values in the equation (mu)
        private static float Mx(int init, int end, int[] hist)
        {
            int sum = 0;
            int i;
            for(i = init; i <= end; i++)
                sum += i * hist[i];

            return (float)sum;
        }

        // finds the maximum element in a vector
        private static int findMax(float[] vec, int n)
        {
            float maxVec = 0;
            int idx = 0;
            int i;

            for(i = 1; i < n - 1; i++)
            {
                if(vec[i] > maxVec)
                {
                    maxVec = vec[i];
                    idx = i;
                }
            }
            return idx;
        }

        // simply computes the image histogram
        unsafe static private void getHistogram(byte* p, int w, int h, int ws, int[] hist)
        {
            hist.Initialize();
            for(int i = 0; i < h; i++)
            {
                for(int j = 0; j < w * 3; j += 3)
                {
                    int index = i * ws + j;
                    hist[p[index]]++;
                }
            }
        }

        // find otsu threshold
        public static int getOtsuThreshold(this Bitmap bmp)
        {
            byte t = 0;
            float[] vet = new float[256];
            int[] hist = new int[256];
            vet.Initialize();

            float p1, p2, p12;
            int k;

            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* p = (byte*)(void*)bmData.Scan0.ToPointer();

                getHistogram(p, bmp.Width, bmp.Height, bmData.Stride, hist);

                // loop through all possible t values and maximize between class variance
                for(k = 1; k != 255; k++)
                {
                    p1 = Px(0, k, hist);
                    p2 = Px(k + 1, 255, hist);
                    p12 = p1 * p2;
                    if(p12 == 0)
                        p12 = 1;
                    float diff = ( Mx(0, k, hist) * p2 ) - ( Mx(k + 1, 255, hist) * p1 );
                    vet[k] = (float)diff * diff / p12;
                    //vet[k] = (float)Math.Pow((Mx(0, k, hist) * p2) - (Mx(k + 1, 255, hist) * p1), 2) / p12;
                }
            }
            bmp.UnlockBits(bmData);

            t = (byte)findMax(vet, 256);

            return t;
        }

        // simple routine to convert to gray scale
        public static void Convert2GrayScaleFast(this Bitmap bmp)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* p = (byte*)(void*)bmData.Scan0.ToPointer();
                int stopAddress = (int)p + bmData.Stride * bmData.Height;
                while((int)p != stopAddress)
                {
                    p[0] = (byte)( .299 * p[2] + .587 * p[1] + .114 * p[0] );
                    p[1] = p[0];
                    p[2] = p[0];
                    p += 3;
                }
            }
            bmp.UnlockBits(bmData);
        }

        // simple routine for thresholdin
        public static  void threshold(this Bitmap bmp, int thresh)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* p = (byte*)(void*)bmData.Scan0.ToPointer();
                int h = bmp.Height;
                int w = bmp.Width;
                int ws = bmData.Stride;

                for(int i = 0; i < h; i++)
                {
                    byte* row = &p[i * ws];
                    for(int j = 0; j < w * 3; j += 3)
                    {
                        row[j] = (byte)( ( row[j] > (byte)thresh ) ? 255 : 0 );
                        row[j + 1] = (byte)( ( row[j + 1] > (byte)thresh ) ? 255 : 0 );
                        row[j + 2] = (byte)( ( row[j + 2] > (byte)thresh ) ? 255 : 0 );
                    }
                }
            }
            bmp.UnlockBits(bmData);
        }
        #endregion
    }
}
