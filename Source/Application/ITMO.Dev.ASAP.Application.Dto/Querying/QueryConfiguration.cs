namespace ITMO.Dev.ASAP.Application.Dto.Querying;

public record QueryConfiguration<T>(IReadOnlyCollection<QueryParameter<T>> Parameters);