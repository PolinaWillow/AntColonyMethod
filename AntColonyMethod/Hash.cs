using System;
using System.Security.Cryptography;
using System.Text;


namespace AntColonyMethod
{
    class Hash
    {
        public static string GetHash(int[] way) //Получение Хэша
        {
            string StringWay = string.Join(",", way); //Получение строки
            byte[] StrSource = ASCIIEncoding.ASCII.GetBytes(StringWay); //Получение массива байтов 
            byte[] StrHash = new MD5CryptoServiceProvider().ComputeHash(StrSource); //Получение Хэша
            //Преобразование Хэша в строку
            string HashWay = ByteArrayToString(StrHash);

            return HashWay;
        }

        public static string ByteArrayToString(byte[] arrInput) //Преобрахование Хэша в строку
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
