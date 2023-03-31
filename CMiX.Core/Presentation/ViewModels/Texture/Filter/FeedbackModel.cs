// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class FeedbackModel : ITextureFilterModel
    {
        public FeedbackModel()
        {
            ID = Guid.NewGuid();
            Name = TextureFilterName.Feedback;
            Visible = new BooleanValueModel(true);
            Factor = new FloatValueModel();
        }

        public BooleanValueModel Visible { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public TextureFilterName Name { get; set; }
        public FloatValueModel Factor { get; set; }
    }
}
