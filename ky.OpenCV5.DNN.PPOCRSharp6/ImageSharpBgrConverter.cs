using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ky.OpenCV5.DNN.PPOCRSharp6
{
    public static class ImageSharpBgrConverter
    {
        public static byte[] ToBgrBytes(Image<Rgb24> image)
        {
            byte[] rgb = new byte[image.Width * image.Height * 3];
            image.CopyPixelDataTo(rgb);
            for (int i = 0; i < rgb.Length; i += 3)
            {
                (rgb[i], rgb[i + 2]) = (rgb[i + 2], rgb[i]);
            }
            return rgb;
        }
    }
}
