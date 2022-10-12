// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class TransformSRTModel : IPrefabModel, IModifierModel
    {
        public TransformSRTModel()
        {
            this.ID = Guid.NewGuid();

            Translate = new TranslateModel();
            Scale = new ScaleModel();
            Rotation = new RotationModel();
            Visible = new ToggleButtonModel(true);
            Uniform = new SliderModel(0.0f);

            Mode = new ComboBoxModel<ModifierMode>(ModifierMode.ToSpread);

            TransformModifier = new ModifierManagerModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public SliderModel Uniform { get; set; }
        public TranslateModel Translate { get; set; }
        public ScaleModel Scale { get; set; }
        public RotationModel Rotation { get; set; }

        public ToggleButtonModel Visible { get; set; }
        public ComboBoxModel<ModifierMode> Mode { get; internal set; }
        public ModifierManagerModel TransformModifier { get; internal set; }
    }
}
