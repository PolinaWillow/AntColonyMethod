using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace AntColonyLib.Processing
{
    public class SqliteWork
    {
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        private string connectionString = "Data Source=../../../../SqliteHashBd/Hashdata.db; Mode=ReadWriteCreate";

        private SqliteConnection connection;

        /// <summary>
        /// Подключение к БД
        /// </summary>
        public void ConnectionToBd()
        {
            connection.Open();
            Console.WriteLine("Подключение к БД");
            DeleteTable();

            CreateTable();
        }

        /// <summary>
        /// Создание таблицы хранения Хэшей
        /// </summary>
        private void CreateTable()
        {
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = "CREATE TABLE Hashes(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, Hash TEXT NOT NULL)";
            command.ExecuteNonQuery();

            Console.WriteLine("Таблица Hashes создана");
        }


        public void DeleteTable()
        {
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = "DROP TABLE Hashes";
            command.ExecuteNonQuery();
            //connection.Close();
            Console.WriteLine("Таблица Hashes удалена");
        }

        /// <summary>
        /// Добавление hash в БД
        /// </summary>
        /// <param name="hash"></param>
        public void AddHashToTable(string hash)
        {
            if (FindHashInTable(hash) == 1)//Если Hash не записан в таблицу
            {

                //Console.WriteLine(hash);

                string sqlExpression = $"INSERT INTO Hashes (Hash) VALUES (\"{hash}\")";
                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                int res = command.ExecuteNonQuery();
                //Console.WriteLine("В таблицу Hashes добавлен новый объект" + res);
            }
            else
            {
                //Console.WriteLine("Данный hash уже добавлен в таблицу");
            }

        }

        /// <summary>
        /// Поиск Hash в таблице
        /// </summary>
        public int FindHashInTable(string hash = "")
        {
            string sqlExpression;
            if (hash != "")
            {
                sqlExpression = $"SELECT * FROM Hashes WHERE Hash='{hash}'";
            }
            else
            {
                sqlExpression = $"SELECT * FROM Hashes";
            }


            SqliteCommand command = new SqliteCommand(sqlExpression, connection);

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows) // Eсли есть данные
                {
                    return 0;
                }
                return 1; //Нет значения
            }

        }

        /// <summary>
        /// Определение количества записей в БД
        /// </summary>
        public int RecordsCount()
        {
            int recordCount = 0; //Общие число записей

            string sqlExpression = "SELECT * FROM Hashes";
            SqliteCommand command = new SqliteCommand(sqlExpression, connection);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    recordCount++;
                }
            }

            return recordCount;
        }

        /// <summary>
        /// Очистка таблицы
        /// </summary>
        public void ClearTable()
        {
            if (FindHashInTable() == 1)
            {
                Console.WriteLine("Таблица Пуста");
            }
            else
            {
                string sqlExpression = "DELETE  FROM Hashes";

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                int number = command.ExecuteNonQuery();
                Console.WriteLine($"Удалено объектов: {number}");
                Console.WriteLine("Таблица очищена");

            }
        }


        public SqliteWork()
        {
            connection = new SqliteConnection(connectionString);
        }

    }
}
