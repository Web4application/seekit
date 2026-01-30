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

// model handle image descriptions and metadata.
public class Product {
    public string ObjectID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string ImageTags { get; set; } // For Vision search
}
