// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public class Feedback : ObservableObject, ITextureFilter
    {
        public Feedback(FeedbackModel feedBackModel, CompositionService compositionService)
        {
            ID = feedBackModel.ID;
            Name = feedBackModel.Name;
            Visible = new BooleanValue(feedBackModel.Visible, compositionService);
            Factor = new FloatValue(feedBackModel.Factor, compositionService);
        }


        public TextureFilterName Name { get; set; }
        public BooleanValue Visible { get; set; }
        public Guid ID { get; set; }
        public FloatValue Factor { get; set; }


        public void Dispose()
        {

        }
    }
}
