// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformModel : IPrefabModel
    {
        public TransformModel()
        {
            ID = Guid.NewGuid();
            Enabled = true;
            TransformModifier = new ModifierManagerModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }

        public ModifierManagerModel TransformModifier { get; internal set; }
    }
}
