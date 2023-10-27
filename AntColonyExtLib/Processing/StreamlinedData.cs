using AntColonyExtLib.DataModel.DataForUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntColonyExtLib.Processing
{
    public class StreamlinedData
    {
        //delegate void AddHandler(string message);
        
        //public event AddHandler Notify;

        public StreamlinedData()
        {
            MessageData = new List<MessageForUser>();
            _monitor = new object();
        }
        /// <summary>
        /// Список сообщений с промежуточными результатами
        /// </summary>
        public List<MessageForUser> MessageData { get; set; }

        /// <summary>
        /// Монитор для ограничения критической секции
        /// </summary>
        private object _monitor { get; set; }

        /// <summary>
        /// Добавление промежуточного результата
        /// </summary>
        /// <param name="mas">Промежуточный результат</param>
        public void AddToList(string mas, long percentage)
        {
            Monitor.Enter(_monitor);
            MessageForUser newMessageForUser = new MessageForUser();
            newMessageForUser.Message = mas;
            newMessageForUser.Percentage = percentage;
            MessageData.Add(newMessageForUser); // добавляем промежуточный результат в список
            
            //if (Notify != null) Notify("Произошло добавление");
            
            Monitor.Exit(_monitor);
        }

        /// <summary>
        /// Вывод результата пользователю
        /// </summary>
        /// <returns></returns>
        public MessageForUser GetMessage()
        {
            Monitor.Enter(_monitor);
            if (MessageData.Count > 0)
            {

                MessageForUser result = MessageData[0];
                MessageData.RemoveAt(0);
                Monitor.Exit(_monitor);
                return result;
            }
            Monitor.Exit(_monitor);
            return null;
        }
    }
}
