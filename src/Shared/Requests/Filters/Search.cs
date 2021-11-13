namespace FSH.BlazorWebAssembly.Shared.Requests.Filters
{
    public class Search
    {
        public List<string> Fields { get; set; } = new List<string>();
        public string Keyword { get; set; }
    }
}