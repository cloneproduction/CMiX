// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models.BaseControls;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class TypeWriterModel : IModel
    {
        public TypeWriterModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;

            StringControl = new StringControlModel();

            FontFamily = new ComboBoxModel<string>("Arial");

            FontSize = new SliderModel(0.45f);

            FontColor = new ColorSelectorModel("#ff000000");
            BackgroundColor = new ColorSelectorModel("#00000000");

            ResolutionX = new CounterModel(1024);
            ResolutionY = new CounterModel(1024);

            PositionX = new SliderModel();
            PositionY = new SliderModel();

            Style = new ComboBoxModel<FontStyle>(FontStyle.Normal);
        }

        public Guid ID { get; set; }
        public bool Enabled { get; set; }

        public StringControlModel StringControl { get; internal set; }

        public ColorSelectorModel FontColor { get; internal set; }
        public ColorSelectorModel BackgroundColor { get; internal set; }

        public CounterModel ResolutionX { get; internal set; }
        public CounterModel ResolutionY { get; internal set; }

        public SliderModel PositionX { get; internal set; }
        public SliderModel PositionY { get; internal set; }

        public SliderModel FontSize { get; internal set; }
        public ComboBoxModel<string> FontFamily { get; internal set; }
        public ComboBoxModel<FontStyle> Style { get; internal set; }
    }
}
