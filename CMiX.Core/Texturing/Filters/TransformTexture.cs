// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections.Specialized;
using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;
using CMiX.Core.Modulation.Modulators;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Transformation;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public partial class TransformTexture : TextureFilterBase, IDisposable
    {
        public TransformTexture(PrefabService prefabService,
                                SamplerState samplerState,
                                PrefabManager modulatorManager,
                                ModulatableFloat locationX,
                                ModulatableFloat locationY,
                                ModulatableFloat scaleX,
                                ModulatableFloat scaleY,
                                ModulatableFloat rotation,
                                ModulatableFloat uniform,
                                GenericValue<float> control,
                                Blend blend)
            : base(prefabService, control, blend)
        {
            SamplerState = samplerState;
            ModulatorManager = modulatorManager;
            Location = new ModulatableVector2(locationX, locationY);
            Scale = new ModulatableVector2(scaleX, scaleY);
            rotation.Label = "Rotation";
            uniform.Label = "Uniform";
            Bindables = new List<ModulatableFloat> { locationX, locationY, scaleX, scaleY, rotation, uniform };

            ModulatorManager.ManagerData.Items.CollectionChanged += OnModulatorManagerItemsChanged;
        }

        public SamplerState SamplerState { get; set; }
        public PrefabManager ModulatorManager { get; set; }
        public List<ModulatableFloat> Bindables { get; set; } = new();

        public ModulatableVector2 Location { get; }
        public ModulatableVector2 Scale { get; }
        public ModulatableFloat Rotation => Bindables[4];
        public ModulatableFloat Uniform => Bindables[5];

        private void OnModulatorManagerItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null) return;

            var bindables = Bindables.Cast<IModulatorBindable>();
            foreach (IControl removed in e.OldItems)
                foreach (var bindable in bindables.Where(b => b.ModulatorID == removed.ID))
                    bindable.SetModulatorCommand.Execute(null);
        }

        public void Dispose()
        {
            ModulatorManager.ManagerData.Items.CollectionChanged -= OnModulatorManagerItemsChanged;
            ModulatorManager.Dispose();
        }

        public override IControlModel ToModel()
        {
            var model = new TransformTextureModel
            {
                SamplerState = (SamplerStateModel)SamplerState.ToModel(),
                ModulatorManager = (PrefabManagerModel)ModulatorManager.ToModel(),
                Bindables = Bindables.Select(c => (ModulatableFloatModel)c.ToModel()).ToList()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (TransformTextureModel)model;
            LoadBaseModel(m);
            SamplerState.FromModel(m.SamplerState);
            LoadManager(ModulatorManager, m.ModulatorManager);

            for (int i = 0; i < Bindables.Count && i < m.Bindables.Count; i++)
                Bindables[i].FromModel(m.Bindables[i]);

            foreach (var bindable in Bindables)
            {
                if (bindable.ModulatorID.Value is not { } modulatorId) continue;
                bindable.BoundModulator = ModulatorManager.ManagerData.Items
                    .OfType<IModulator>()
                    .FirstOrDefault(mod => mod.ID == modulatorId);
            }
        }
    }
}
