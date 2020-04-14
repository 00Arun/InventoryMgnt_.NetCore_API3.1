using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryBooks.Infrastructure
{
    public class DynamoDbContext<T> : DynamoDBContext, IDynamoDbContext<T> where T : class
    {
        private readonly DynamoDBOperationConfig _config;
        private readonly IAmazonDynamoDB client;

        public DynamoDbContext(IAmazonDynamoDB client, string tableName) : base(client)
        {
            _config = new DynamoDBOperationConfig()
            {
                OverrideTableName = tableName
            };
            this.client = client;
            CreateTable(_config).ConfigureAwait(false);
        }

        private async Task CreateTable(DynamoDBOperationConfig config)
        {
            var tables = await client.ListTablesAsync();
            if (!tables.TableNames.Contains(config.OverrideTableName))
            {
                await client.CreateTableAsync(new CreateTableRequest()
                {
                    TableName = config.OverrideTableName,
                    AttributeDefinitions = new List<AttributeDefinition>()
                    {
                        new AttributeDefinition
                        {
                            AttributeName = "Id",
                            AttributeType = "S"
                        }
                    },
                    KeySchema = new List<KeySchemaElement>()
                    {
                        new KeySchemaElement
                        {
                            AttributeName = "Id",
                            KeyType= "HASH" // Partition key
                        }
                    },
                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 10,
                        WriteCapacityUnits = 5
                    }
                });
                string status = null;
                do
                {
                    Thread.Sleep(5000);
                    try
                    {
                        var res = client.DescribeTableAsync(new DescribeTableRequest
                        {
                            TableName = config.OverrideTableName
                        });
                        status = res.Result.Table.TableStatus;
                    }
                    catch (ResourceNotFoundException ex)
                    {
                        status = ex.Message;
                    }
                } while (status != "ACTIVE");
            }
        }

        public async Task DeleteByIdAsync(T item)
        {
            await base.DeleteAsync(item, _config);
        }

        public async Task DeleteByIdAsync(string id)
        {
            await base.DeleteAsync(id, _config);
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            var scan = new ScanOperationConfig
            {
                Limit = 1,
                Filter = null
            };
            return await base.FromScanAsync<T>(scan, _config).GetRemainingAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(ScanFilter scanConditions)
        {
            var scan = new ScanOperationConfig
            {
                Limit = 1,
                Filter = scanConditions
            };
            return await base.FromScanAsync<T>(scan, _config).GetRemainingAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await base.LoadAsync<T>(id, _config);
        }

        public int GetCount()
        {
            ScanFilter filter = new ScanFilter();
            var table = base.GetTargetTable<T>(_config);
            return table.Scan(filter).Count;
        }

        public async Task SaveAsync(T item)
        {
            await base.SaveAsync(item, _config);
        }
    }
}
