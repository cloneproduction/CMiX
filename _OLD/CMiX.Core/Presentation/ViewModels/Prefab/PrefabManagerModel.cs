// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Presentation.ViewModels.Prefab
{
    public class PrefabManagerModel : IPrefabManagerModel
    {
        public PrefabManagerModel()
        {
            this.ID = Guid.NewGuid();
            Enabled = true;
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
    }
}
