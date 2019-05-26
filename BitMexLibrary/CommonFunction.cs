using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary
{
  public  class CommonFunction
    {

        /// <summary>В какое время (в секундах отсчитываемых от 01 января 1970г.) истечёт действие запроса</summary>
        /// <returns>Время в секундах отсчитываемых от 01 января 1970г.</returns>
        public static long GetExpiresArg()
        {
            // Количество секунд прошедших с 01 января 1970г.
            long timestamp = (long)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);

            //// Добавляет 60 секунд и переводит в string
            //string expires = (timestamp + 60).ToString();

            return timestamp + 60;
        }

        /// <summary>Получение хеш SHA256 в строке шестнадцатеричного представления</summary>
        /// <param name="APISecret">Хеш ключ</param>
        /// <param name="APIExpires">Хешируемое сообщение</param>
        /// <returns>Строка шестнадцатеричного представления массива байт полученного Хеш</returns>
        public static string GetWebSocketSignatureString(string APISecret, string APIExpires)
        {
            // Хеш от APIExpires по ключу APISecret
            byte[] signatureBytes = HMACSHA256(Encoding.UTF8.GetBytes(APISecret), Encoding.UTF8.GetBytes("GET/realtime" + APIExpires));
            // Строка шестнадцатерично представления
            string signatureString = ByteArrayToString(signatureBytes);
            return signatureString;
        }
        public static string GetWebSocketSignatureString(string APISecret, long APIExpires)
             => GetWebSocketSignatureString(APISecret, APIExpires.ToString());

        /// <summary>Получает хэш-код с помощью хэш-функции SHA256</summary>
        /// <param name="keyByte">Ключ инициализации</param>
        /// <param name="messageBytes">Сообщение для которого вычисляется хеш</param>
        /// <returns>Вычисленный хэш-код</returns>
        public static byte[] HMACSHA256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        /// <summary>Переводит массив байт в строку шестнадцатиричного представления</summary>
        /// <param name="ba">Массив байт</param>
        /// <returns>Строка шестнадцатиричного представления</returns>
        public  static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

    }
}
