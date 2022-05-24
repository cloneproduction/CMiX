// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class BlendModeModel : IModel
    {
        public BlendModeModel()
        {
            this.ID = Guid.NewGuid();
            Mode = ((BlendModeEnum)0).ToString();
        }

        public BlendModeModel(BlendModeEnum blendModeEnum)
        {
            this.ID = Guid.NewGuid();
            Mode = blendModeEnum.ToString();
        }

        public string Mode { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
    }
}
