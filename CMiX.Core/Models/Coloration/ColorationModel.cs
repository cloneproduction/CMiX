// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class ColorationModel : IPrefabModel
    {
        public ColorationModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            ModifierManager = new ModifierManagerModel();
            ColorSelector = new ColorSelectorModel("#FFFFFFFF");
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ModifierManagerModel ModifierManager { get; set; }
        public ColorSelectorModel ColorSelector { get; set; }
    }
}
