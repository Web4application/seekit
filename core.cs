using Algolia.Search.Clients;
using Algolia.Search.Models.Search;
using OllamaSharp;
using System.Text;

public class Product 
{
    public string ObjectID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
}

class SeekitEngine
{
    private static readonly SearchClient _algolia = new SearchClient(new SearchConfig("APP_ID", "API_KEY"));
    private static readonly OllamaApiClient _llama = new OllamaApiClient(new Uri("http://localhost:11434"));

    static async Task Main()
    {
        string userQuery = "I need a budget laptop for gaming under $800";
        string indexName = "products";

        Console.WriteLine($"SEEKIT is searching for: '{userQuery}'...");

        // 1. RETRIEVE: Get the most relevant data from Algolia
        var searchParams = new SearchMethodParams {
            Requests = new List<SearchQuery> {
                new SearchQuery(new SearchForHits { 
                    IndexName = indexName, 
                    Query = userQuery, 
                    HitsPerPage = 3 
                })
            }
        };

        var searchResponse = await _algolia.SearchAsync<Product>(searchParams);
        var hits = searchResponse.Results.AsSearchResponse<Product>().Hits;

        // 2. AUGMENT: Format the found data as context for the AI
        var contextBuilder = new StringBuilder();
        foreach (var hit in hits) {
            contextBuilder.AppendLine($"- {hit.Name}: {hit.Description} (Price: ${hit.Price})");
        }

        string context = contextBuilder.ToString();
        string prompt = $@"
            You are SEEKIT, a helpful AI assistant. 
            Based ONLY on the following product data, answer the user's question.
            If the data doesn't contain a good match, say so.

            CONTEXT DATA:
            {context}

            USER QUESTION: 
            {userQuery}";

        // 3. GENERATE: Let Llama 3 process the data and answer
        Console.WriteLine("\nSEEKIT AI is thinking...\n");
        _llama.SelectedModel = "llama3"; // Ensure you have run 'ollama pull llama3'

        await foreach (var stream in _llama.GenerateAsync(prompt))
        {
            Console.Write(stream.Response); // Stream the response live
        }
    }
}
