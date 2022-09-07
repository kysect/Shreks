using GitHubJwt;

namespace Kysect.Shreks.Integration.Github.Helpers;

public class FullStringPrivateKeySource : IPrivateKeySource
{
    private readonly string _key;

    public FullStringPrivateKeySource(string key) => this._key = !string.IsNullOrEmpty(key) ? key : throw new ArgumentNullException(nameof (key));

    public TextReader GetPrivateKeyReader() => new StringReader(this._key);
}