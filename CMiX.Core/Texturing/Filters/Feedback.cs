// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public class Feedback : ObservableObject, IModifier
    {
        public Feedback(FeedbackModel feedBackModel)
        {
            ID = feedBackModel.ID;
            Name = feedBackModel.Name;
            Visible = new BooleanValue(feedBackModel.Visible);
            Factor = new FloatValue(feedBackModel.Factor);
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
