// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Models
{
    public class InstancerModel : IModel
    {
        public InstancerModel()
        {
            ID = Guid.NewGuid();
            Transform = new TransformModel();
            Counter = new CounterModel();
            ModifierManagerModel = new ModifierManagerModel();
            UniformScale = new AnimParameterModel();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public ModifierManagerModel ModifierManagerModel { get; set; }
        public AnimParameterModel UniformScale { get; set; }
        public TransformModel Transform { get; set; }
        public CounterModel Counter { get; set; }
        public bool NoAspectRatio { get; set; }
    }
}
