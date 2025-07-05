namespace SearchEngine.Domain;

public sealed class SearchableDocumentBuilder
{
    private readonly SearchableDocument _document = new();

    public SearchableDocumentBuilder WithTitle(string lang, string value)
    {
        _document.Title ??= new();
        _document.Title[lang] = value;
        AddTags(value);
        AddKeywords(value);
        AddTranslitrationTags(value);
        return this;
    }

    public SearchableDocumentBuilder WithContent(string lang, string value)
    {
        _document.Content ??= new();
        _document.Content[lang] = value;
        return this;
    }

    public SearchableDocumentBuilder WithDescription(string lang, string value)
    {
        _document.Description ??= new();
        _document.Description[lang] = value;
        AddKeywords(value);
        return this;
    }

    public SearchableDocumentBuilder WithType(string type)
    {
        _document.Type = type;
        return this;
    }

    public SearchableDocumentBuilder WithAuthor(string authorId)
    {
        _document.AuthorId = authorId;
        return this;
    }

    public SearchableDocumentBuilder WithLanguage(string defaultLanguage)
    {
        _document.DefaultLanguage = defaultLanguage;
        return this;
    }

    public SearchableDocumentBuilder WithCreatedAt(DateTime createdAt)
    {
        _document.CreatedAt = createdAt;
        return this;
    }

    public SearchableDocumentBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        _document.UpdatedAt = updatedAt;
        return this;
    }

    public SearchableDocumentBuilder WithCategory(params string[] categories)
    {
        foreach (var cat in categories)
        {
            _document.Categories.Add(cat);
            AddTags(cat);
            AddKeywords(cat);
        }
        
        return this;
    }
    
    public SearchableDocumentBuilder WithSubcategory(params string[] subcategories)
    {
        foreach (var sub in subcategories)
        {
            _document.Subcategories.Add(sub);
            AddTags(sub);
            AddKeywords(sub);
        }
        return this;
    }

    public SearchableDocumentBuilder WithKeyword(params string[] keywords)
    {
        foreach (var kw in keywords)
        {
            _document.Keywords.Add(kw);
            _document.Keywords.Add(kw.Normalize());
            _document.Keywords.Add(kw.GenerateReversedKeyboardVariant());
            _document.Keywords.Add(kw.GeneratePersianReversedKeyboardVariant());

            var dictationVariants = kw.GeneratePhoneticDictationVariants();
            foreach (var dictationVariant in dictationVariants)
            {
                _document.Keywords.Add(dictationVariant);
            }
        }
        return this;
    }

    public SearchableDocumentBuilder WithTag(params string[] tags)
    {
        foreach (var tag in tags)
        {
           AddTags(tag);
        }
        return this;
    }

    public SearchableDocumentBuilder WithTransliterationTags(params string[] tags)
    {
        foreach (var tag in tags)
            _document.TranslitrationTags.Add(tag);
        return this;
    }

    public SearchableDocumentBuilder WithSynonyms(params string[] synonyms)
    {
        foreach (var syn in synonyms)
        {
            _document.Synonyms.Add(syn);
           AddTags(syn);
           AddTranslitrationTags(syn);
           AddKeywords(syn);
        }
        
        return this;
    }

    public SearchableDocumentBuilder WithGeo(double latitude, double longitude)
    {
        _document.Latitude = latitude;
        _document.Longitude = longitude;
        return this;
    }

    public SearchableDocumentBuilder WithRegion(params string[] regions)
    {
        foreach (var region in regions)
        {
            _document.Regions.Add(region);
            AddTags(region);
        }
        return this;
    }

    public SearchableDocumentBuilder WithEntityMetadata(string key, string value)
    {
        _document.EntityMetadata[key] = value;
        AddTags(value);
        AddKeywords(value);
        return this;
    }

    public SearchableDocumentBuilder WithEmbedding(params float[] embedding)
    {
        _document.Embedding = embedding;
        return this;
    }

    public SearchableDocumentBuilder MarkAsFeatured(bool isFeatured = true)
    {
        _document.IsFeatured = isFeatured;
        return this;
    }

    public SearchableDocumentBuilder WithLastInteraction(DateTime time)
    {
        _document.LastInteractionAt = time;
        return this;
    }

    public SearchableDocumentBuilder WithSourceSystem(string system)
    {
        _document.SourceSystem = system;
        return this;
    }

    public SearchableDocumentBuilder WithRankScore(double score)
    {
        _document.RankScore = score;
        return this;
    }

    public SearchableDocumentBuilder WithMetrics(int views = 1, int likes = 1, double rating = 1.0, int hits = 0)
    {
        _document.ViewCount = views;
        _document.LikeCount = likes;
        _document.UserRating = rating;
        _document.SearchHitCount = hits;
        return this;
    }
    private void AddKeywords(string input)
    {
        var keywords = input.GetAllKeywordsTranslitration();
        foreach (var keyword in keywords)
        {
            _document.Keywords.Add(keyword);
        }
    }

    private void AddTags(string input)
    {
        var translitrationTags = input.GetAllTranslitrasion();
        foreach (var tag in translitrationTags)
        {
            _document.Tags.Add(tag);    
        }
    }

    private void AddTranslitrationTags(string input)
    {
        var translitrationTags = input.GetAllTranslitrasion();
        foreach (var tag in translitrationTags)
        {
            _document.TranslitrationTags.Add(tag);    
        }
    }

    public SearchableDocument Build()
    {
        return _document;
    }
}