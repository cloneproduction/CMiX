// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models
{
    public class PostFXModel : IModel
    {
        public PostFXModel()
        {
            ID = Guid.NewGuid();
            Feedback = new SliderModel();
            Blur = new SliderModel();

            Transforms = ((PostFXTransforms)0).ToString();
            View = ((PostFXView)0).ToString();
        }

        public bool Enabled { get; set; }
        public Guid ID { get; set; }
        public SliderModel Feedback { get; set; }
        public SliderModel Blur { get; set; }

        public string Transforms { get; set; }
        public string View { get; set; }
    }
}
