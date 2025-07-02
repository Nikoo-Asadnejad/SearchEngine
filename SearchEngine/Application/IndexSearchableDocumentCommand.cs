using Elasticsearch.Net;
using MediatR;
using Nest;
using SearchEngine.Domain;
using IRequest = MediatR.IRequest;

namespace SearchEngine.Application;

public record IndexSearchableDocumentCommand(SearchableDocument Document) : IRequest;

internal sealed class IndexSearchableDocumentCommandHandler(IElasticClient elasticClient) : IRequestHandler<IndexSearchableDocumentCommand>
{
    public async Task Handle(IndexSearchableDocumentCommand request, CancellationToken cancellationToken)
    {
        var response= await elasticClient.IndexAsync(request.Document, idx => idx
            .Index("searchable_documents")
            .Refresh(Elasticsearch.Net.Refresh.WaitFor)
        );

        if (!response.IsValid)
        {
            throw new Exception($"Indexing failed: {response.DebugInformation}");
        }
    }
} 