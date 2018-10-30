using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var rates = ReadCSV(args[0]);
            var table = AWSUtil.CreateDynamoDbTable();
            AWSUtil.LoadJsonDataToTable(table, rates);
        }

        private static List<Rate> ReadCSV(string filePath)
        {
            List<Rate> rates = File.ReadAllLines(filePath).Skip(1).Select(v => Rate.FromCSV(v)).ToList();
            var convertedJson = JsonConvert.SerializeObject(rates, Formatting.Indented);
            int var = 1;
            return rates;
        }

        

    }
}
