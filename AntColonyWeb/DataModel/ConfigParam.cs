using System.Data;

namespace AntColonyWeb.DataModel
{
    public class ConfigParam
    {
        /// <summary>
        /// Номер параметра
        /// </summary>
        public int numParam { get; set; }

        /// <summary>
        /// Тип параметра
        /// </summary>
        public string typeParam { get; set; }

        /// <summary>
        /// Количество значений параметра
        /// </summary>
        public int valueCountParam { get; set; }

        /// <summary>
        /// Список значений параметра
        /// </summary>
        public List<string> valuesParam { get; set; }

        public ConfigParam() {
            valuesParam = new();
            numParam= 0;
            typeParam = "";
            valueCountParam = 0;
        }

        public void ClearP() {
            valuesParam = new();
            numParam = 0;
            typeParam = "";
            valueCountParam = 0;
        }

        public override string ToString()
        {
            return "numParam: " + numParam + "   typeParam: " + typeParam + "   valueCountParam: " + valueCountParam;
        }

    }
}
