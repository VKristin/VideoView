using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoView
{
    /// <summary>
    /// Класс для получения изображения из байтового представления
    /// </summary>
    internal static class JpegGetter
    {
        /// <summary>
        /// Метод для получения изображения из байтового представления
        /// </summary>
        /// <param name="buffer">массив байт</param>
        /// <returns>изображение</returns>
        public static Image GetImageFromBytes(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
