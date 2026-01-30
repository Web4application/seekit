using Algolia.Search.Clients;
using OllamaSharp;
using Microsoft.SemanticKernel;

class SeekitUltimate {
    private static SearchClient _algolia = new SearchClient(new SearchConfig("APP_ID", "API_KEY"));
    private static OllamaApiClient _llama = new OllamaApiClient(new Uri("http://localhost:3000"));
    
    // Memory: Simplified User History
    private static string userPreference = "The user loves high-end gaming gear.";

    static async Task Main() {
        // --- STEP 1: VISION (Letting SEEKIT "See") ---
        // Imagine the user uploads a photo of a glowing keyboard. 
        // We use LLaVA (Vision Model) to describe it.
        Console.WriteLine("SEEKIT is analyzing your image...");
        _llama.SelectedModel = "llava"; // Pull this model: 'ollama pull llava'
        var imageDescription = await _llama.GenerateAsync("Describe this image in 3 keywords for a search engine.");
        
        string searchTerms = "Gaming Keyboard RGB"; // Result from Vision

        // --- STEP 2: SEARCH + MEMORY (Smart Retrieval) ---
        // We add the user's past preferences to the search query
        var searchParams = new SearchMethodParams {
            Requests = new List<SearchQuery> {
                new SearchQuery(new SearchForHits { 
                    IndexName = "products", 
                    Query = searchTerms,
                    OptionalFilters = new List<string> { "Category:Gaming" } // Personalization
                })
            }
        };

        var results = await _algolia.SearchAsync<Product>(searchParams);
        var topProduct = results.Results[0].AsSearchResponse<Product>().Hits.First();

        // --- STEP 3: ACTION (The Agent) ---
        // Use Llama to decide if we should buy it or email it
        _llama.SelectedModel = "llama3";
        string actionPrompt = $@"
            User likes: {userPreference}
            Found Item: {topProduct.Name} - {topProduct.Description}
            
            Action: Should I 'EMAIL' this to the user or just 'DISPLAY' it? 
            Explain why in one sentence.";

        await foreach (var token in _llama.GenerateAsync(actionPrompt)) {
            Console.Write(token.Response);
        }
        
        // Example Agent Action
        if (actionPrompt.Contains("EMAIL")) {
            await SendEmailAgent(topProduct.Name);
        }
    }

    static async Task SendEmailAgent(string productName) {
        Console.WriteLine($"\n[AGENT]: Sending email about {productName} to user...");
        // Use System.Net.Mail here to finalize the action
    }
}
