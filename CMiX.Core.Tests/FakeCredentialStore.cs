using CMiX.Core.Networking;

namespace CMiX.Core.Tests
{
    // A credential store for tests. Add(name, user, password) fills it.
    public sealed class FakeCredentialStore : ICredentialStore
    {
        private readonly Dictionary<string, (string User, string Password)> credentials = new();

        public void Add(string name, string user, string password) => credentials[name] = (user, password);

        public bool TryRead(string name, out string user, out string password)
        {
            if (!string.IsNullOrEmpty(name) && credentials.TryGetValue(name, out var entry))
            {
                user = entry.User;
                password = entry.Password;
                return true;
            }

            user = string.Empty;
            password = string.Empty;
            return false;
        }
    }
}
