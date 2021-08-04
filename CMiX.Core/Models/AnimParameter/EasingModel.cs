// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Mathematics;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class EasingModel : IModel
    {
        public EasingModel()
        {
            this.ID = Guid.NewGuid();
            IsEnabled = false;
        }

        public bool Enabled { get; set; }
        public bool IsEnabled { get; set; }
        public Guid ID { get; set; }
        public Easings.Functions SelectedEasing { get; set; }
        public EasingFunction EasingFunction { get; set; }
        public EasingMode EasingMode { get; set; }
    }
}
