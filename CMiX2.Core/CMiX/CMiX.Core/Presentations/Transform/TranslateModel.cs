// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Modifiers;

namespace CMiX.Core.Presentations.Transform
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
