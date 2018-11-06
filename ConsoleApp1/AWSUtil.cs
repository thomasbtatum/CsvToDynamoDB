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

        public static Table CreateDynamoDbTable(string user, string pass)
        {
            var tableName = string.Format("Rates{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));

            var client = new AmazonDynamoDBClient(new BasicAWSCredentials(user, pass), RegionEndpoint.USEast1);

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
                AttributeName = "Age",
                AttributeType = "N"
            },
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
                AttributeName = "Age",
                KeyType = "Hash"
            },
            new KeySchemaElement
            {
                AttributeName = "Id",
                KeyType = "Range"
            }
            },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 10,
                        WriteCapacityUnits = 5
                    },
                };

                var response = client.CreateTable(request);

                Console.WriteLine(string.Format("Table {0} created with request ID: {1}",tableName, response.ResponseMetadata.RequestId));
                int var = 1;
            }

            var table = Table.LoadTable(client, tableName);
            System.Threading.Thread.Sleep(10000);
            return table;

        }


        public static void LoadJsonDataToTable(Table table, List<Rate> Rates)
        {
            int count = 0;
            foreach (Rate rate in Rates)
            {
                var item = Document.FromJson(JsonConvert.SerializeObject(rate, Formatting.Indented));

                var d = table.PutItem(item);
                int x = 1;
                System.Threading.Thread.Sleep(1000);
                count++;
                if (count % 20 == 0)
                {
                    Console.WriteLine(string.Format("Wrote {0} records to table.", count));

                }
            }
        }

    }
}
