using CMiX.Core.Prefabs.Managers;

namespace CMiX.Core
{
    public static class ControlExtensions
    {
        public static void LoadManager(PrefabManager manager, PrefabManagerModel model)
        {
            manager.FromModel(model);
            foreach (var item in model.ManagerData.Items)
                manager.LoadItem(item);
        }
    }
}
