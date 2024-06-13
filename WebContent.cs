using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace VideoView
{
    /// <summary>
    /// Класс для подключения и получения результата запроса
    /// </summary>
    internal class WebContent
    {
        private string apiAddress = "http://demo.macroscop.com:8080/mobile?login=root";
        private string channelId = "2016897c-8be5-4a80-b1a3-7f79a9ec729c";
        private int resolutionX = 640;
        private int resolutionY = 480;
        private byte fps = 25;
        private HttpClient client;
        private bool isRunning = false;
        private PictureBox pbVideo;

        /// <summary>
        /// Конструктор для дальнейшей работы с различными каналами
        /// </summary>
        /// <param name="channelId">ID канала для подключения</param>
        public WebContent(string channelId, PictureBox pbVideo)
        {
            this.channelId = channelId;
            this.pbVideo = pbVideo;
            client = new HttpClient();
        }

        /// <summary>
        /// Конструктор для работы с захардкоженным ID канала
        /// </summary>
        public WebContent(PictureBox pbVideo)
        {
            client = new HttpClient();
            this.pbVideo = pbVideo;
        }

        /// <summary>
        /// Метод для получения результата HTTP-запроса
        /// </summary>
        public async Task GetHttpContent()
        {
            string param = $"&channelId={channelId}&resolutionX={resolutionX}&resolutionY={resolutionY}&fps={fps}";
            var streamResponse = await client.GetStreamAsync(apiAddress + param);

            byte[] buffer = new byte[4096]; // Буфер для чтения данных
            while (isRunning)
            {
                int bytesRead = await streamResponse.ReadAsync(buffer, 0, buffer.Length); // Чтение данных из потока

                if (bytesRead == 0)
                    break; // Прекращение чтения, если данные не получены

                // Преобразование считанных данных в строку
                string header = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                int contentLengthIndex = header.IndexOf("Content-Length:");
                if (contentLengthIndex != -1)
                {
                    // Получение значения Content-Length
                    int startIndex = contentLengthIndex + "Content-Length:".Length;
                    int endIndex = header.IndexOf('\r', startIndex);
                    string contentLengthValue = header.Substring(startIndex, endIndex - startIndex).Trim();
                    if (int.TryParse(contentLengthValue, out int contentLength))
                    {
                        int newLineIndex = header.IndexOf("\r\n\r\n", endIndex);
                        int headerLength = newLineIndex + 4; // длина заголовков, включая \r\n\r\n
                        int imageStartIndex = headerLength;

                        // Вычисление оставшихся байт для чтения изображения
                        int remainingBytes = contentLength - (bytesRead - imageStartIndex);
                        byte[] imageData = new byte[contentLength];
                        Array.Copy(buffer, imageStartIndex, imageData, 0, bytesRead - imageStartIndex);

                        // Чтение оставшихся байт изображения
                        while (remainingBytes > 0)
                        {
                            int read = await streamResponse.ReadAsync(buffer, 0, Math.Min(buffer.Length, remainingBytes));

                            Array.Copy(buffer, 0, imageData, contentLength - remainingBytes, read);
                            remainingBytes -= read;
                        }

                        // Установка нового изображения
                        SetNewImage(imageData);
                    }
                }
            }
        }

        /// <summary>
        /// Установить новое изображение
        /// </summary>
        /// <param name="buffer">Байты изображения</param>
        private void SetNewImage(byte[] buffer)
        {
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
