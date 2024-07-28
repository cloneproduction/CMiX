// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Rendering
{
    public class LocalReflectionModel : IControlModel
    {
        public LocalReflectionModel()
        {
            IsEnabled = new GenericValueModel<bool>(false);
        }

        public GenericValueModel<bool> IsEnabled { get; set; }
        public Guid ID { get; set; }
    }
}
