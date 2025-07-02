using Nest;

namespace SearchEngine.Domain;

[ElasticsearchType(RelationName = "SearchableDocument")]
public class SearchableDocument
{
    public Dictionary<string,string> Title { get; set; }
    public Dictionary<string,string> Content { get; set; }
    public Dictionary<string,string> Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string DefaultLanguage { get; set; }
    public string? AuthorId { get; set; }
    public string Type { get; set; }
    public HashSet<string> Categories { get; set; } = [];
    public HashSet<string> Subcategories { get; set; }= [];
    // every keyword in title and description
    public HashSet<string> Keywords { get; set; }= []; 
    //searching tags for the document , splited title , rank high
    public HashSet<string> Tags { get; set; } = []; 
    //reversed keyboard , dictation faults , normilized , rank high
    public HashSet<string> TranslitrationTags { get; set; }
    public HashSet<string> Synonyms { get; set; } = [];
    public double RankScore { get; set; } = 1;
    public int ViewCount { get; set; } = 1;
    public int LikeCount { get; set; } = 1;
    public double UserRating { get; set; } = 1;
    public int SearchHitCount { get; set; } = 0;
    public int Version { get; set; } = 1;
    public string SourceSystem { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    /// <summary>
    /// Geographic coordinates for spatial/vector combined search
    /// (lat, lon as doubles).
    /// </summary>
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    /// <summary>
    /// Indicates if this document is featured or promoted (boost ranking).
    /// </summary>
    public bool IsFeatured { get; set; } = false;
    /// <summary>
    /// Last interaction timestamp (e.g., last viewed or updated),
    /// useful to boost recent content.
    /// </summary>
    public DateTime? LastInteractionAt { get; set; }
    public HashSet<string> Regions { get; set; } = [];
    // For vector similarity search
    public float[] Embedding { get; set; } 
    public Dictionary<string, string> EntityMetadata { get; set; } = []; // e.g., {"location": "Paris"}
}