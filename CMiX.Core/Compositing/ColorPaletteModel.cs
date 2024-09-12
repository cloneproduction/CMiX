// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Compositing
{
    public class ColorPaletteModel : IPrefabModel
    {
        public ColorPaletteModel()
        {
            
        }
        public PrefabServiceModel PrefabService { get; set; }
        public Guid ID { get; set; }
    }
}
