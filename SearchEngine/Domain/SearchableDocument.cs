using Nest;

namespace SearchEngine.Domain;

[ElasticsearchType(RelationName = "SearchableDocument")]
public class SearchableDocument
{
    public Dictionary<string,string> Title { get; set; }
    public Dictionary<string,string> Content { get; set; }
    public Dictionary<string,string> Description { get; set; }
    public string Type { get; set; }
    public Dictionary<string,string> AdditionalData { get; set; }
    public HashSet<string> Keywords { get; set; }
    public HashSet<string> Groups { get; set; }
    public HashSet<string> Tags { get; set; }
}