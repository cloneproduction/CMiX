// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Texturing.Filters
{
    public class ShiftRGBModel : IPrefabModel
    {
        public ShiftRGBModel()
        {
            ID = Guid.NewGuid();
            Factor = new GenericValueModel<float>(1.0f);
            PrefabService = new PrefabServiceModel();
            Factor = new GenericValueModel<float>(1.0f);
            Direction = new GenericValueModel<float>(0.25f);
            Shift = new GenericValueModel<float>(0.2f);
            Hue = new GenericValueModel<float>(0.0f);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel PrefabService { get; set; }
        public GenericValueModel<float> Factor { get; set; }
        public GenericValueModel<float> Direction { get; set; }
        public GenericValueModel<float> Shift { get; set; }
        public GenericValueModel<float> Hue { get; set; }
    }
}
