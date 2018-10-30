using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var rates = ReadCSV(args);
            CreateDynamoDbTable(rates);
            //LoadJsonDataToTable();
        }

        private static List<Rate> ReadCSV(string[] args)
        {
            List<Rate> rates = File.ReadAllLines(args[0]).Skip(1).Select(v => Rate.FromCSV(v)).ToList();
            var convertedJson = JsonConvert.SerializeObject(rates, Formatting.Indented);
            int var = 1;
            return rates;
        }

        private static void CreateDynamoDbTable(List<Rate> rates)
        {

            var tableName = string.Format("Rates{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
            //var credentials = 

            //new AmazonS3Client(credentials, RegionEndpoint.USEast1);
            var client = new AmazonDynamoDBClient(new BasicAWSCredentials("AKIAJV65WRVVJMKNNQUA", "IUtTCQtSJG0CHZsXbbF+L+8EinQJ1XFHgCGW9GWW"), RegionEndpoint.USEast1);
            
            Console.WriteLine("Getting list of tables");
            List<string> currentTables = client.ListTables().TableNames;
            Console.WriteLine("Number of tables: " + currentTables.Count);
            if (!currentTables.Contains("blah"))
            {
                var request = new CreateTableRequest
                {
                    TableName = tableName,
                    AttributeDefinitions = new List<AttributeDefinition>
            {
            new AttributeDefinition
            {
                AttributeName = "Id",
                // "S" = string, "N" = number, and so on.
                AttributeType = "N"
            }//,
            //new AttributeDefinition
            //{
            //    AttributeName = "Age",
            //    // "S" = string, "N" = number, and so on.
            //    AttributeType = "N"
            //},
            //new AttributeDefinition
            //{
            //    AttributeName = "TenYear",
            //    AttributeType = "N"
            //},
            //            new AttributeDefinition
            //{
            //    AttributeName = "TwentyYear",
            //    // "S" = string, "N" = number, and so on.
            //    AttributeType = "N"
            //},
            //new AttributeDefinition
            //{
            //    AttributeName = "AtAgeSixtyFive",
            //    // "S" = string, "N" = number, and so on.
            //    AttributeType = "N"
            //},
            //new AttributeDefinition
            //{
            //    AttributeName = "Male",
            //    // "S" = string, "N" = number, and so on.
            //    AttributeType = "S"
            //},
            //new AttributeDefinition
            //{
            //    AttributeName = "Smoker",
            //    // "S" = string, "N" = number, and so on.
            //    AttributeType = "S"
            //}

            },
                    KeySchema = new List<KeySchemaElement>
            {
            new KeySchemaElement
            {
                AttributeName = "Id",
                // "HASH" = hash key, "RANGE" = range key.
                KeyType = "HASH"
            }
            },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 10,
                        WriteCapacityUnits = 5
                    },
                };

                var response = client.CreateTable(request);

                Console.WriteLine("Table created with request ID: " +
                    response.ResponseMetadata.RequestId);
                int var = 1;
            }

            var table = Table.LoadTable(client, tableName);

            LoadJsonData(table, rates);
        }

        private static void LoadJsonData(Table table, List<Rate> Rates)
        {
            
            foreach (Rate rate in Rates)
            {
                var item = Document.FromJson(JsonConvert.SerializeObject(rate, Formatting.Indented));

                table.PutItem(item);
            }
        }
    }

    public class Rate
    {

        public int Id;
        public int Age;
        public decimal TenYear;
        public decimal TwentyYear;
        public decimal AtAgeSixtyFive;
        public bool Male;
        public bool Smoker;

        public static Rate FromCSV(string csvLine)
        {
            var values = Regex.Split(csvLine, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            
            var rate = new Rate();
            rate.Id = Convert.ToInt32(values[0]);
            rate.Age = Convert.ToInt32(values[1]);
            rate.TenYear = TreatDecimals(values[2]);
            rate.TwentyYear = TreatDecimals(values[3]);
            rate.AtAgeSixtyFive = TreatDecimals(values[4]);
            rate.Male = 'F' == Convert.ToChar(values[5]);
            rate.Smoker = 'Y' == Convert.ToChar(values[6]);
            return rate;

        }

        public static Decimal TreatDecimals(string decimalString)
        {
            if (decimalString == "N/A")
                return Decimal.Zero;
            else
                return Convert.ToDecimal(decimalString.Replace("\\", "").Replace("\"", ""));
        }

    }
}
