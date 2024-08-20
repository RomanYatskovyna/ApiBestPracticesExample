namespace ApiBestPracticesExample.Test.Integration.Fixtures;
public sealed class ExternalConnectionProvider:ConnectionProviderBase
{
    private readonly string _sqlConnectionStr;

    public ExternalConnectionProvider(string sqlConnectionStr)
    {
        _sqlConnectionStr = sqlConnectionStr;
    }
    public override string GetDbConnectionString()
    {
        return _sqlConnectionStr;
    }
}
