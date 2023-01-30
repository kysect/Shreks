using Kysect.Shreks.Application.Dto.Querying;
using Microsoft.AspNetCore.Components;

namespace Kysect.Shreks.WebUI.AdminPanel.Components.Query;

public partial class QueryComponent<TParameter, TValue> : IDisposable where TParameter : struct, Enum
{
    private readonly List<QueryParameter<TParameter>> _addedParameters;

    public QueryComponent()
    {
        _addedParameters = new List<QueryParameter<TParameter>>();
    }

    [Parameter]
    public IEnumerable<TParameter>? SupportedParameters { get; set; }

    [Parameter]
    public Func<QueryConfiguration<TParameter>, Task<IReadOnlyCollection<TValue>>>? QueryExecutor { get; set; }

    public IEnumerable<QueryParameter<TParameter>> AddedParameters => _addedParameters
        .Where(x => SupportedParameters?.Contains(x.Type) ?? true);

    public IEnumerable<TParameter> AvailableParameters => (SupportedParameters ?? Enum.GetValues<TParameter>())
        .Where(x => AddedParameters.Any(xx => xx.Type.Equals(x)) is false);

    public bool IsFull => AvailableParameters.Count() is 0;

    public void Dispose()
    {
        _cts.Dispose();
    }

    public void Add()
    {
        TParameter type = AvailableParameters.First();

        var parameter = new QueryParameter<TParameter>(type, string.Empty);
        _addedParameters.Add(parameter);

        _visible = true;
    }

    public void Add(QueryParameter<TParameter> parameter)
    {
        _addedParameters.Add(parameter);
    }

    public void Remove(QueryParameter<TParameter> parameter)
    {
        _addedParameters.Remove(parameter);
    }

    public void Update()
    {
        StateHasChanged();
    }

    public Task<IReadOnlyCollection<TValue>> ExecuteQueryAsync()
    {
        if (QueryExecutor is null)
            return Task.FromResult<IReadOnlyCollection<TValue>>(Array.Empty<TValue>());

        QueryConfiguration<TParameter> configuration = Build();
        return QueryExecutor.Invoke(configuration);
    }

    private QueryConfiguration<TParameter> Build()
    {
        QueryParameter<TParameter>[] parameters = _addedParameters
            .Where(x => string.IsNullOrEmpty(x.Pattern) is false)
            .ToArray();

        return new QueryConfiguration<TParameter>(parameters);
    }
}