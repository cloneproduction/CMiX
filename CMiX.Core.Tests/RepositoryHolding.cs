using CMiX.Core.Prefabs;

namespace CMiX.Core.Tests
{
    internal static class RepositoryHolding
    {
        public static int CollectionManagerCount(ControlRepository repository) =>
            repository.Controls.SelectMany(repository.CollectionManagersHolding).Distinct().Count();
    }
}
