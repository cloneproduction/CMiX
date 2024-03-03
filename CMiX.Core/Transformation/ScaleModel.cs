// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation
{
    public class ScaleModel : IControlModel, IModifierModel
    {
        public ScaleModel()
        {
            ID = Guid.NewGuid();
            Uniform = new GenericValueModel<float>(1.0f);
            XYZ = new Vector3Model(1.0f, 1.0f, 1.0f);
            Visible = new GenericValueModel<bool>(true);
        }

        public Guid ID { get; set; }
        public GenericValueModel<float> Uniform { get; set; }
        public Vector3Model XYZ { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
    }
}
