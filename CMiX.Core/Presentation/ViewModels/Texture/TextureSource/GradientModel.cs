// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels
{
    public class GradientModel : IModel
    {
        public GradientModel()
        {
            Enabled = true;
            ID = Guid.NewGuid();
            ResolutionX = new CounterModel(512);
            ResolutionY = new CounterModel(512);
            From = new ColorSelectorModel("#FFFFFFFF");
            To = new ColorSelectorModel("#FF000000");
            Gamma = new SliderModel(2.2f);
            Horizontal = new ToggleButtonModel(false);
            Transform2D = new Transform2DModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public CounterModel ResolutionX { get; set; }
        public CounterModel ResolutionY { get; set; }
        public SliderModel Gamma { get; set; }
        public ColorSelectorModel From { get; set; }
        public ColorSelectorModel To { get; set; }
        public ToggleButtonModel Horizontal { get; set; }
        public Transform2DModel Transform2D { get; set; }
        public ColorSelectorModel BackgroundColor { get; set; }
    }
}
