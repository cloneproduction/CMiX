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
            Visible = new BooleanValue(feedBackModel.Visible);
            Factor = new FloatValue(feedBackModel.Factor);
        }


        public TextureFilterName Name { get; set; }
        public bool Enabled { get; set; }
        public BooleanValue Visible { get; set; }
        public Guid ID { get; set; }
        public FloatValue Factor { get; set; }


        public IModel GetModel()
        {
            FeedbackModel feedbackModel = new FeedbackModel();

            feedbackModel.ID = ID;
            feedbackModel.Name = Name;
            feedbackModel.Visible = (BooleanValueModel)Visible.GetModel();
            feedbackModel.Factor = (FloatValueModel)Factor.GetModel();

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
