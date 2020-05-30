using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Utils
{
    /// <summary>
    /// 转换器
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// 将Base64转为图像
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns>Bitmap</returns>
        public static Bitmap ConvertBase64ToImage(string base64String)
        {
            byte[] imageArray = Convert.FromBase64String(base64String);
            Bitmap image = null;
            using (MemoryStream stream = new MemoryStream(imageArray))
            {
                image = new Bitmap(stream);
            }
            return image;
        }


        /// <summary>
        /// 将图像转换为Base64
        /// </summary>
        /// <param name="image">转化图像</param>
        /// <param name="format">图像格式</param>
        /// <returns>Base64字符串</returns>
        public static async Task<string> ConvertImageToBase64(Bitmap image, ImageFormat format)
        {
            string base64String = "";
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, format);
                byte[] array = new byte[stream.Length];
                stream.Position = 0;
                await stream.ReadAsync(array, 0, (int)stream.Length);
                base64String = Convert.ToBase64String(array);
            }
            return base64String;
        }
    }
}
