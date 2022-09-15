// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models.Component
{
    public class OutputSettingsModel : IModel
    {
        public OutputSettingsModel()
        {
            ID = Guid.NewGuid();
            ResolutionX = new CounterModel(1920);
            ResolutionY = new CounterModel(1080);
            BackgroundColor = new ColorSelectorModel("#FF000000");
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public CounterModel ResolutionX { get; set; }
        public CounterModel ResolutionY { get; set; }
        public ColorSelectorModel BackgroundColor { get; internal set; }
    }
}
