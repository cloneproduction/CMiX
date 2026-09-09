// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Specialized;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs.Managers;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing
{
    // Every texture's own mandatory coordinate transform - not an item in a filter chain, so it
    // does not implement ITextureFilter and never appears in an "Add Filter" panel.
    public class TextureTexCoord : IControl, IDisposable
    {
        public TextureTexCoord(PrefabManager modulatorManager,
                               ModulatableValue<float> locationX, ModulatableValue<float> locationY,
                               ModulatableValue<float> scaleX, ModulatableValue<float> scaleY,
                               ModulatableValue<float> rotation, ModulatableValue<float> uniform)
        {
            ModulatorManager = modulatorManager;
            Transform = new TexCoordTransform(locationX, locationY, scaleX, scaleY, rotation, uniform);

            ModulatorManager.ManagerData.Items.CollectionChanged += OnModulatorManagerItemsChanged;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public PrefabManager ModulatorManager { get; set; }
        public TexCoordTransform Transform { get; }

        public ModulatableVector2 Location => Transform.Location;
        public ModulatableVector2 Scale => Transform.Scale;
        public ModulatableValue<float> Rotation => Transform.Rotation;
        public ModulatableValue<float> Uniform => Transform.Uniform;

        private void OnModulatorManagerItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null) return;

            var bindables = Transform.Bindables.Cast<IModulatorBindable>();
            foreach (IControl removed in e.OldItems)
                foreach (var bindable in bindables.Where(b => b.ModulatorID == removed.ID))
                    bindable.SetModulatorCommand.Execute(null);
        }

        public void Dispose()
        {
            ModulatorManager.ManagerData.Items.CollectionChanged -= OnModulatorManagerItemsChanged;
            ModulatorManager.Dispose();
        }

        public IControlModel ToModel() => new TextureTexCoordModel
        {
            ID = ID,
            ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel(),
            Bindables = Transform.Bindables.Select(c => (ModulatableValueModel<float>)c.ToModel()).ToList()
        };

        public void FromModel(IControlModel model)
        {
            var m = (TextureTexCoordModel)model;
            ID = m.ID;
            LoadManager(ModulatorManager, m.ModulatorManager);

            var bindables = Transform.Bindables;
            for (int i = 0; i < bindables.Count && i < m.Bindables.Count; i++)
                bindables[i].FromModel(m.Bindables[i]);

            foreach (var bindable in bindables)
            {
                if (bindable.ModulatorID is not { } modulatorId) continue;
                bindable.BoundModulator = ModulatorManager.ManagerData.Items
                    .OfType<IModulator>()
                    .FirstOrDefault(mod => mod.ID == modulatorId);
            }
        }
    }
}
