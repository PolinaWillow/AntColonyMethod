﻿using AntColonyExtLib.DataModel;
using System.Text.Json;

namespace AntColonyExtLib.ClusterInteraction.Models.Calculation
{
    public class Calculation
    {
        //Отправляемые данные
        public SendData sendData { get; set; }

        public string idAgent { get; set; }

        public string JSON_Data { get; set; }

        public double result { get; set; }

        public Calculation(int[] way = null, InputData inputData = null, string id = null)
        {
            sendData = new SendData(way, inputData);
            idAgent = id;
            JSON_Data = JsonSerializer.Serialize(sendData);
            result = 0;
        }

        public string TypeOf()
        {
            return "Calculation";
        }
    }
}
