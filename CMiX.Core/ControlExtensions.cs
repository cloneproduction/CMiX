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
            manager.Collection.SelectedItemChanged(model.ManagerData.SelectedIndex);
        }

        // Sibling to LoadManager, for the common "Dispose owns nothing but a fixed set of child
        // managers/controls" shape - one place to change disposal behavior (e.g. continue past a
        // throwing Dispose instead of aborting) instead of the same few lines repeated per class.
        public static void DisposeAll(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
                disposable.Dispose();
        }
    }
}
