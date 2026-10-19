using CMiX.Core.Prefabs;

namespace CMiX.Core.Tests
{
    internal static class HeldCollectionManagers
    {
        public static int Count(ControlRepository repository) =>
            repository.Controls.SelectMany(repository.GetCollectionManagers).Distinct().Count();
    }
}
