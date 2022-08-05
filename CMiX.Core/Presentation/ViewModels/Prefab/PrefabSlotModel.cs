// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class PrefabSlotModel : IModel
    {
        public PrefabSlotModel()
        {

        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public bool IsSelected { get; internal set; }
    }
}
