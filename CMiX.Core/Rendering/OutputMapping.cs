// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Rendering
{
    public class OutputMapping : ObservableObject, IControl
    {
        public OutputMapping(GenericValue<string> name,
                             Integer2 resolution,
                             GenericValue<TexcoordSemantic> texcoordSemantic)
        {
            Name = name;
            Resolution = resolution;
            TexcoordSemantic = texcoordSemantic;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<string> Name { get; set; }
        public Integer2 Resolution { get; set; }

        // Which UV channel this slot maps to on the 3D model. Fixed 1:1 with the slot's position
        // in OutputMappingManager.Items (slot 0 is Texcoord0, etc.), so it is not exposed for
        // editing, only carried through so the engine can read it.
        public GenericValue<TexcoordSemantic> TexcoordSemantic { get; set; }

        public IControlModel ToModel() => new OutputMappingModel
        {
            ID = ID,
            Name = (GenericValueModel<string>)Name.ToModel(),
            Resolution = (Integer2Model)Resolution.ToModel(),
            TexcoordSemantic = (GenericValueModel<TexcoordSemantic>)TexcoordSemantic.ToModel()
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
        }

        public override string ToString() => Name.Value;
    }
}
