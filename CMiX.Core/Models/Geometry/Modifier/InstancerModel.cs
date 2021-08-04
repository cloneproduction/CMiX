// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models
{
    public class InstancerModel : IModel
    {
        public InstancerModel()
        {
            ID = Guid.NewGuid();
            Transform = new TransformModel();
            Counter = new CounterModel();
            TransformModifierModel = new TransformModifierModel();
            UniformScale = new AnimParameterModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public TransformModifierModel TransformModifierModel { get; set; }
        public AnimParameterModel UniformScale { get; set; }
        public TransformModel Transform { get; set; }
        public CounterModel Counter { get; set; }
        public bool NoAspectRatio { get; set; }
    }
}
