using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PopoverGPMDP {
    public static class Utils {
        /// <summary>
        /// Tints the given image by a colour
        /// </summary>
        /// <param name="filename">The file to tint</param>
        /// <param name="color"></param>
        /// <returns>The tinted version of the image</returns>
        public static WriteableBitmap TintImage(string filename, int color) {
            // loads the specified image from the application data
            var image = new WriteableBitmap(new BitmapImage(new Uri("pack://application:,,,/PopoverGPMDP;component/img/" + filename, UriKind.Absolute)));

            var bytes = new byte[image.BackBufferStride * image.PixelHeight];
            image.CopyPixels(bytes, image.BackBufferStride, 0);

            for (var i = 0; i < bytes.Length; i++) {
                if (i % 4 != 3) // skip over the transparency byte
                    bytes[i] = (byte) (color >> 8 * (i % 4) & 0xff);
            }
                
            image.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), bytes, image.BackBufferStride, 0);

            return image;
        }
    }
}