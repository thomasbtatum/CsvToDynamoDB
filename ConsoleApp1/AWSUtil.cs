using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    public class AWSUtil
    {

        public static Table CreateDynamoDbTable()
        {
            var tableName = string.Format("Rates{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            var client = new AmazonDynamoDBClient(new BasicAWSCredentials("", ""), RegionEndpoint.USEast1);

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
                AttributeType = "N"
            }
                    },
                    KeySchema = new List<KeySchemaElement>
            {
            new KeySchemaElement
            {
                AttributeName = "Id",
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
            System.Threading.Thread.Sleep(5000);
            return table;

        }


        public static void LoadJsonDataToTable(Table table, List<Rate> Rates)
        {
            foreach (Rate rate in Rates)
            {
                var item = Document.FromJson(JsonConvert.SerializeObject(rate, Formatting.Indented));

                table.PutItem(item);
                System.Threading.Thread.Sleep(1000);

            }
        }

    }
}
