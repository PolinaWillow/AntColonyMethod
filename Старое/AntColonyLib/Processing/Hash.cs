using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using AntColonyLib.DataModel;

namespace AntColonyLib.Processing
{
    public class Hash
    {
        /// <summary>
        /// Монитор
        /// </summary>
        private object _monitor;

        public Hash()
        {
            _monitor = new object();
        }

        public void AddNewHash(string hashWay, int[] wayAgent, DataTask dataTask)
        {
            Monitor.Enter(_monitor);
            if (ChangeableParams.HASH_SAVE)
            {
                dataTask.squliteBD.AddHashToTable(hashWay);
            }
            else
            {
                dataTask.hashTable.Add(hashWay, wayAgent);
            }
            Monitor.Exit(_monitor);
        }

        /// <summary>
        /// Получение Хэша пути
        /// </summary>
        /// <param name="way">Путь</param>
        /// <returns></returns>
        public string GetHash(int[] way) //Получение Хэша
        {
            string StringWay = string.Join(",", way); //Получение строки
            byte[] StrSource = Encoding.ASCII.GetBytes(StringWay); //Получение массива байтов 
            byte[] StrHash = new MD5CryptoServiceProvider().ComputeHash(StrSource); //Получение Хэша
            //Преобразование Хэша в строку
            string HashWay = ByteArrayToString(StrHash);

            return HashWay;
        }

        /// <summary>
        /// Преобразование Хэша в строку
        /// </summary>
        /// <param name="arrInput">Хэш</param>
        /// <returns></returns>
        private string ByteArrayToString(byte[] arrInput) //Преобрахование Хэша в строку
        {
            int i;
            StringBuilder sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}
