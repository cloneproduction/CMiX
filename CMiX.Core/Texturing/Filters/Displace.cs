// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Modulation;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Texturing.Sources;
using static CMiX.Core.ControlExtensions;

namespace CMiX.Core.Texturing.Filters
{
    public partial class Displace : TextureFilterBase
    {
        public Displace(PrefabService prefabService,
                        PrefabManager textureSelector,
                        ModulatableVector2 offset,
                        ModulatableVector2 offsetScale,
                        Blend blend,
                        ModulatorHost modulators)
            : base(prefabService, blend, modulators)
        {
            TextureSelector = textureSelector;
            Offset = offset;
            OffsetScale = offsetScale;

            Bindables = new List<ModulatableValue<float>>
            {
                offset.X, offset.Y,
                offsetScale.X, offsetScale.Y
            };

            TextureSelector.CanAddExistingItem = control => control is not ITextureSource texture || !WouldLoop(texture);
        }

        // True when the texture, or a texture that it displaces with, has this filter. Taking it would
        // make the filter displace with itself.
        public bool WouldLoop(ITextureSource texture) => Reaches(texture, new HashSet<ITextureSource>());

        private bool Reaches(ITextureSource texture, HashSet<ITextureSource> visited)
        {
            if (!visited.Add(texture))
                return false;

            var filters = texture.TextureModifierManager?.ManagerData.Items.OfType<Displace>();
            if (filters == null)
                return false;

            foreach (var filter in filters)
            {
                if (ReferenceEquals(filter, this))
                    return true;

                if (filter.TextureSelector.ManagerData.Items.OfType<ITextureSource>().Any(source => Reaches(source, visited)))
                    return true;
            }

            return false;
        }

        public PrefabManager TextureSelector { get; set; }

        public override Guid CompositionID
        {
            get => base.CompositionID;
            set
            {
                base.CompositionID = value;
                TextureSelector.CompositionID = value;
            }
        }
        public ModulatableVector2 Offset { get; }
        public ModulatableVector2 OffsetScale { get; }

        public override IControlModel ToModel()
        {
            var model = new DisplaceModel
            {
                TextureSelector = (PrefabManagerModel)TextureSelector.ToModel(),
                Offset = (ModulatableVector2Model)Offset.ToModel(),
                OffsetScale = (ModulatableVector2Model)OffsetScale.ToModel()
            };
            PopulateBaseModel(model);
            return model;
        }

        public override void FromModel(IControlModel model)
        {
            var m = (DisplaceModel)model;
            LoadBaseModel(m);
            Offset.FromModel(m.Offset);
            OffsetScale.FromModel(m.OffsetScale);

            LoadManager(TextureSelector, m.TextureSelector);
        }

        // The texture this filter displaces with is reachable through this selector alone, so a
        // filter torn down without disposing it leaves its repository and its deleter
        // registrations behind.
        public override void Dispose()
        {
            base.Dispose();
            DisposeAll(TextureSelector);
        }
    }
}
