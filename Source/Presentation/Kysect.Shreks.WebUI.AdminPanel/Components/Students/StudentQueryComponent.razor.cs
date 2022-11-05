using Kysect.Shreks.WebApi.Sdk;

namespace Kysect.Shreks.WebUI.AdminPanel.Components.Students;

public partial class StudentQueryComponent
{
    private readonly List<StudentQueryParameterQueryParameter> _parameters;

    public StudentQueryComponent()
    {
        _parameters = new List<StudentQueryParameterQueryParameter>();
    }

    public IReadOnlyCollection<StudentQueryParameterQueryParameter> Parameters => _parameters;

    public IEnumerable<StudentQueryParameter> AvailableParameters => Enum
        .GetValues<StudentQueryParameter>()
        .Where(x => Parameters.Any(xx => xx.Type.Equals(x)) is false);

    public bool IsFull => AvailableParameters.Count() is 0;

    public void Add()
    {
        var type = AvailableParameters.First();

        var parameter = new StudentQueryParameterQueryParameter { Type = type, Pattern = string.Empty, };
        _parameters.Add(parameter);

        _visible = true;
    }

    public void Remove(StudentQueryParameterQueryParameter parameter)
    {
        _parameters.Remove(parameter);
    }

    public void Update()
        => StateHasChanged();

    public StudentQueryParameterQueryConfiguration Build()
    {
        var parameters = _parameters
            .Where(x => string.IsNullOrEmpty(x.Pattern) is false)
            .ToArray();

        return new StudentQueryParameterQueryConfiguration { Parameters = parameters, };
    }
}