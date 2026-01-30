// Create a list of your typed objects
var products = new List<Product>
{
    new Product { ObjectID = "1", Name = "Laptop", Category = "Electronics" },
    new Product { ObjectID = "2", Name = "Smartphone", Category = "Electronics" }
};

// Batch index the records
var batchResponse = await client.SaveObjectsAsync(indexName, products);
await client.WaitForTaskAsync(indexName, batchResponse.TaskId);
Console.WriteLine("Batch indexing complete!");
