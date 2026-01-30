var results = await client.SearchAsync<object>(
    new SearchMethodParams {
        Requests = new List<SearchQuery> {
            new SearchQuery(new SearchForHits { IndexName = "index_name", Query = "query" })
        }
    }
);
