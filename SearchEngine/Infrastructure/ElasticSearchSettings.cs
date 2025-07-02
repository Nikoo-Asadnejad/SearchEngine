namespace SearchEngine.Infrastructure;

public class ElasticSearchSettings
{
    public string Uri { get; set; } = default!;
    public string DefaultIndex { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}