// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Presentation.Prefab
{
    public class PrefabManagerModel : IModel
    {
        public PrefabManagerModel()
        {
            this.ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
    }
}
