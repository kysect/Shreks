namespace Kysect.Shreks.Application.Dto.Querying;

public record QueryConfiguration<T>(IReadOnlyCollection<QueryParameter<T>> Parameters);