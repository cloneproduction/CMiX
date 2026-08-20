// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    public class OutputMapping : ObservableObject, IControl
    {
        public OutputMapping(GenericValue<string> name,
                             Integer2 resolution,
                             GenericValue<TexcoordSemantic> texcoordSemantic,
                             GenericValue<bool> visibility)
        {
            Name = name;
            Resolution = resolution;
            TexcoordSemantic = texcoordSemantic;
            Visibility = visibility;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<string> Name { get; set; }
        public Integer2 Resolution { get; set; }

        // Which UV channel this slot maps to on the 3D model. Fixed 1:1 with the slot's position
        // in OutputMappingManager.Items (slot 0 is Texcoord0, etc.), so it is not exposed for
        // editing, only carried through so the engine can read it.
        public GenericValue<TexcoordSemantic> TexcoordSemantic { get; set; }

        // Same naming/shape as PrefabService.Visibility used for Composition/Layer, so this
        // reads consistently with the rest of the codebase.
        public GenericValue<bool> Visibility { get; set; }

        public IControlModel ToModel() => new OutputMappingModel
        {
            ID = ID,
            Name = (GenericValueModel<string>)Name.ToModel(),
            Resolution = (Integer2Model)Resolution.ToModel(),
            TexcoordSemantic = (GenericValueModel<TexcoordSemantic>)TexcoordSemantic.ToModel(),
            Visibility = (GenericValueModel<bool>)Visibility.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            // ID intentionally left untouched: OutputMappingManager assigns it a fixed
            // ManagerIDs.OutputMappingSlots entry at construction, regardless of what an (older)
            // saved file contains.
            var m = (OutputMappingModel)model;
            Name.FromModel(m.Name);
            Resolution.FromModel(m.Resolution);
            TexcoordSemantic.FromModel(m.TexcoordSemantic);
            Visibility.FromModel(m.Visibility);
        }

        public override string ToString() => Name.Value;
    }
}
