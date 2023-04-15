// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation
{
    public class TranslateModel : IModifierModel
    {
        public TranslateModel()
        {
            ID = Guid.NewGuid();
            XYZ = new Vector3Model();
            Visible = new BooleanValueModel(true);
        }

        public Guid ID { get; set; }
        public Vector3Model XYZ { get; internal set; }
        public BooleanValueModel Visible { get; set; }
    }
}
