// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;

namespace CMiX.Core.Presentation.ViewModels.Modifiers
{
    public class ModifierManagerModel : IModel
    {
        public ModifierManagerModel()
        {
            this.ID = Guid.NewGuid();
            Visibility = new BooleanValueModel(true);
        }

        public Guid ID { get; set; }
        public BooleanValueModel Visibility { get; internal set; }
    }
}
