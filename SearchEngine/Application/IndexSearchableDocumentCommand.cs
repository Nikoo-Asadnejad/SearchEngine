using Elasticsearch.Net;
using MediatR;
using Nest;
using SearchEngine.Domain;
using IRequest = MediatR.IRequest;

namespace SearchEngine.Application;

public record IndexSearchableDocumentCommand : IRequest
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
    public HashSet<string> Keywords { get; set; }= []; 
    public HashSet<string> Tags { get; set; } = []; 
    public HashSet<string> TranslitrationTags { get; set; } = [];
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
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsFeatured { get; set; } = false;
    public DateTime? LastInteractionAt { get; set; }
    public HashSet<string> Regions { get; set; } = [];
    public float[] Embedding { get; set; } 
    public Dictionary<string, string> EntityMetadata { get; set; } = []; // e.g., {"location": "Paris"}
} 

internal sealed class IndexSearchableDocumentCommandHandler(IElasticClient elasticClient) : IRequestHandler<IndexSearchableDocumentCommand>
{
    public async Task Handle(IndexSearchableDocumentCommand request, CancellationToken cancellationToken)
    {
        var builder = new SearchableDocumentBuilder();

        foreach (var title in request.Title)
            builder.WithTitle(title.Key, title.Value);

        foreach (var content in request.Content)
            builder.WithContent(content.Key, content.Value);

        foreach (var desc in request.Description)
            builder.WithDescription(desc.Key, desc.Value);

        builder.WithType(request.Type)
            .WithLanguage(request.DefaultLanguage)
            .WithCreatedAt(request.CreatedAt)
            .WithUpdatedAt(request.UpdatedAt)
            .WithAuthor(request.AuthorId)
            .WithCategory(request.Categories.ToArray())
            .WithSubcategory(request.Subcategories.ToArray())
            .WithKeyword(request.Keywords.ToArray())
            .WithTag(request.Tags.ToArray())
            .WithTransliterationTags(request.TranslitrationTags.ToArray())
            .WithSynonyms(request.Synonyms.ToArray())
            .WithGeo(request.Latitude ?? 0, request.Longitude ?? 0)
            .WithRegion(request.Regions.ToArray())
            .WithEmbedding(request.Embedding)
            .WithSourceSystem(request.SourceSystem)
            .WithLastInteraction(request.LastInteractionAt ?? DateTime.UtcNow)
            .MarkAsFeatured(request.IsFeatured)
            .WithRankScore(request.RankScore)
            .WithMetrics(request.ViewCount, request.LikeCount, request.UserRating, request.SearchHitCount);

        foreach (var kv in request.EntityMetadata)
            builder.WithEntityMetadata(kv.Key, kv.Value);

        var document = builder.Build();
        
        var response= await elasticClient.IndexAsync(document, idx => idx
            .Index("searchable_documents")
            .Refresh(Elasticsearch.Net.Refresh.WaitFor)
        );

        if (!response.IsValid)
        {
            throw new Exception($"Indexing failed: {response.DebugInformation}");
        }
    }
} 