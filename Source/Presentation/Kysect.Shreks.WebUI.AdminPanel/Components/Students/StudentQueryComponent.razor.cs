using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.WebApi.Sdk;

namespace Kysect.Shreks.WebUI.AdminPanel.Components.Students;

public partial class StudentQueryComponent : IDisposable
{
    private readonly List<QueryParameter<StudentQueryParameter>> _parameters;

    public StudentQueryComponent()
    {
        _parameters = new List<QueryParameter<StudentQueryParameter>>();
    }

    public IReadOnlyCollection<QueryParameter<StudentQueryParameter>> Parameters => _parameters;

    public IEnumerable<StudentQueryParameter> AvailableParameters => Enum
        .GetValues<StudentQueryParameter>()
        .Where(x => Parameters.Any(xx => xx.Type.Equals(x)) is false);

    public bool IsFull => AvailableParameters.Count() is 0;

    public void Add()
    {
        var type = AvailableParameters.First();

        var parameter = new QueryParameter<StudentQueryParameter>(type, string.Empty);
        _parameters.Add(parameter);

        _visible = true;
    }

    public void Remove(QueryParameter<StudentQueryParameter> parameter)
    {
        _parameters.Remove(parameter);
    }

    public void Update()
        => StateHasChanged();

    public QueryConfiguration<StudentQueryParameter> Build()
    {
        QueryParameter<StudentQueryParameter>[] parameters = _parameters
            .Where(x => string.IsNullOrEmpty(x.Pattern) is false)
            .ToArray();

        return new QueryConfiguration<StudentQueryParameter>(parameters);
    }

    public void Dispose()
        => _cts.Dispose();
}