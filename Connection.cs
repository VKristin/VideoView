using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Linq;

namespace VideoView
{
    /// <summary>
    /// Класс для плдключения и получения результата запроса
    /// </summary>
    internal class WebContent
    {
        private string apiAddress = "http://demo.macroscop.com:8080/mobile?login=root";
        private string channelId = "2016897c-8be5-4a80-b1a3-7f79a9ec729c";
        private int resolutionX = 640;
        private int resolutionY = 480;
        private byte fps = 25;
        private HttpClient client;
        bool isRunning = false;
        PictureBox pbVideo;

       /// <summary>
       /// Конструктор для дальнейшей работы с различными каналами
       /// </summary>
       /// <param name="channelId">Айти канала для подключения</param>
       public WebContent(string channelId, PictureBox pbVideo)
        {
            this.channelId = channelId;
            this.pbVideo = pbVideo;
            client = new HttpClient();
        }

        /// <summary>
        /// Конструктор для работы с захардкоженным айди канала
        /// </summary>
        public WebContent(PictureBox pbVideo) 
        { 
            client = new HttpClient();
            this.pbVideo = pbVideo;
        }
        /// <summary>
        /// Метод для получения результата http-запроса
        /// </summary>
        public async Task GetHttpContent()
        {
            string param = $"&channelId={channelId}&resolutionX={resolutionX}&resolutionY={resolutionY}&fps={fps}";
            var streamResponse = await client.GetStreamAsync(apiAddress + param);
            using (var reader = new StreamReader(streamResponse))
            {
                while (isRunning)
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        // Если дошли до строки, в которой хранится количество байт (изображение)
                        if (line.StartsWith("Content-Length:"))
                        {
                            // Между именем параметра и его значением - пробел
                            int bytes = Int32.Parse(line.Split(' ')[1]);
                            // Пропуск пустой строки
                            reader.ReadLine();
                            byte[] buffer = new byte[bytes];
                            reader.BaseStream.Read(buffer, 0, bytes);
                            SetNewImage(buffer);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Установить новое изображение
        /// </summary>
        /// <param name="buffer">хбайты в символьном представлении</param>
        private void SetNewImage(byte[] buffer)
        {
            Encoding.UTF8.GetString(buffer);
            /*Encoding.UTF8.GetString(byteBuffer);
            ImageConverter imgConverter = new ImageConverter();
            pbVideo.Image = imgConverter.ConvertFrom(byteBuffer) as Image;*/
            //pbVideo.Image = (Bitmap)((new ImageConverter()).ConvertFrom(byteBuffer));
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                pbVideo.Image = Image.FromStream(ms);
            }

        }


        /// <summary>
        /// Запуск получения стрима
        /// </summary>
        public void StartGetContent()
        {
            isRunning = true;
        }

        /// <summary>
        /// Остановка получения стрима
        /// </summary>
        public void StopGetContent() 
        {
            isRunning = false;
        }
    }
}
