// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public class ButtonModel : IControlModel
    {
        public ButtonModel()
        {
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
    }
}
