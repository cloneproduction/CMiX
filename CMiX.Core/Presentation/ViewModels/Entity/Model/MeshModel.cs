// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Presentation.ViewModels
{
    public class MeshModel : IPrefabModel
    {
        public MeshModel()
        {
            MeshTypeSelector = new ComboBoxModel<MeshType>(MeshType.Plane);
            Scale = new VectorXYZModel(1.0f, 1.0f, 1.0f);
            Offset = new VectorXYZModel(0.0f, 0.0f, 0.0f);
            Radius = new SliderModel(1.0f);
            Height = new SliderModel(1.0f);
            Thickness = new SliderModel(1.0f);
            Tessellation = new CounterModel(16);
            TessellationX = new CounterModel(16);
            TessellationY = new CounterModel(16);
            GenerateBackFace = new ToggleButtonModel(true);
            Visibility = new ToggleButtonModel(true);
            TransformModifierManager = new ModifierManagerModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public ComboBoxModel<MeshType> MeshTypeSelector { get; internal set; }
        public VectorXYZModel Scale { get; internal set; }
        public VectorXYZModel Offset { get; internal set; }
        public SliderModel Radius { get; internal set; }
        public SliderModel Height { get; internal set; }
        public SliderModel Thickness { get; internal set; }
        public CounterModel Tessellation { get; internal set; }
        public CounterModel TessellationX { get; internal set; }
        public CounterModel TessellationY { get; internal set; }
        public ToggleButtonModel GenerateBackFace { get; internal set; }
        public ToggleButtonModel Visibility { get; internal set; }
        public ModifierManagerModel TransformModifierManager { get; internal set; }
    }
}
