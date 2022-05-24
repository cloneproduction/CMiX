// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Feedback : ObservableObject, ITextureFilter
    {
        public Feedback(FeedbackModel feedBackModel)
        {
            ID = feedBackModel.ID;
            Name = feedBackModel.Name;
            Visible = new ToggleButton(feedBackModel.Visible);
            Factor = new Slider(nameof(Factor), feedBackModel.Factor);
        }


        public TextureFilterName Name { get; set; }
        public bool Enabled { get; set; }
        public ToggleButton Visible { get; set; }
        public Guid ID { get; set; }
        public Slider Factor { get; set; }


        public IModel GetModel()
        {
            FeedbackModel feedbackModel = new FeedbackModel();

            feedbackModel.ID = ID;
            feedbackModel.Name = Name;
            feedbackModel.Visible = (ToggleButtonModel)Visible.GetModel();
            feedbackModel.Factor = (SliderModel)Factor.GetModel();

            return feedbackModel;
        }

        public void SetViewModel(IModel model)
        {
            FeedbackModel feedbackModel = model as FeedbackModel;
            ID = feedbackModel.ID;
            Name = feedbackModel.Name;
            Visible.SetViewModel(feedbackModel.Visible);
            Factor.SetViewModel(feedbackModel.Factor);
        }

        public void Dispose()
        {

        }
    }
}
