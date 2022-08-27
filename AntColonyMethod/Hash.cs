﻿using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace AntColonyMethod
{
    class Hash
    {
        /// <summary>
        /// Монитор
        /// </summary>
        private object _monitor;

        public Hash()
        {
            _monitor = new object();
        }

        public void AddNewHash(string hashWay, int[] wayAgent, Hashtable hashTable)
        {
            Monitor.Enter(_monitor);
            hashTable.Add(hashWay, wayAgent);
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
            byte[] StrSource = ASCIIEncoding.ASCII.GetBytes(StringWay); //Получение массива байтов 
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
        public string ByteArrayToString(byte[] arrInput) //Преобрахование Хэша в строку
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