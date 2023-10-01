// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public class ModifierManagerModel : IControlModel
    {
        public ModifierManagerModel()
        {
            Visibility = new BooleanValueModel(true);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValueModel Visibility { get; internal set; }
    }
}
