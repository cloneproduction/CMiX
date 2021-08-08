// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class PostFX : ObservableObject, IControl
    {
        public PostFX(PostFXModel postFXModel)
        {
            ID = postFXModel.ID;
            Feedback = new Slider(nameof(Feedback), postFXModel.Feedback);
            Blur = new Slider(nameof(Blur), postFXModel.Blur);

            Transforms = postFXModel.Transforms;
            View = postFXModel.View;
        }


        public Guid ID { get; set; }
        public Slider Feedback { get; set; }
        public Slider Blur { get; set; }


        private string _transforms;
        public string Transforms
        {
            get => _transforms;
            set
            {
                SetProperty(ref _transforms, value);

            }
        }

        private string _view;
        public string View
        {
            get => _view;
            set
            {
                SetProperty(ref _view, value);

            }
        }


        public void SetViewModel(IModel model)
        {
            PostFXModel postFXModel = model as PostFXModel;
            this.ID = postFXModel.ID;
            this.Transforms = postFXModel.Transforms;
            this.View = postFXModel.View;
            this.Feedback.SetViewModel(postFXModel.Feedback);
            this.Blur.SetViewModel(postFXModel.Blur);
        }

        public IModel GetModel()
        {
            PostFXModel postFXModel = new PostFXModel();
            postFXModel.ID = this.ID;
            postFXModel.Feedback = (SliderModel)this.Feedback.GetModel();
            postFXModel.Blur = (SliderModel)this.Blur.GetModel();
            postFXModel.Transforms = this.Transforms;
            postFXModel.View = this.View;
            return postFXModel;
        }
    }
}
