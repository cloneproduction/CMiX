// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Scheduling
{
    public sealed class JobModel : IControlModel
    {
        public JobModel()
        {
            ID = Guid.NewGuid();
            PrefabService = new PrefabServiceModel();
            FloatTest = new GenericValueModel<float>(0.9f);
        }

        public Guid ID { get; set; }
        public PrefabServiceModel  PrefabService { get; set; }
        public GenericValueModel<float> FloatTest { get; set; }
    }
}
