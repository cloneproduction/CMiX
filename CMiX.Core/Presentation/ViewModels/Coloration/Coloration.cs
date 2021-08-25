// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Coloration : ObservableObject, IControl
    {
        public Coloration(ColorationModel colorationModel, Guid componentID)
        {
            this.ID = colorationModel.ID;
            BeatModifier = new BeatModifier(colorationModel.BeatModifierModel, componentID);
            ColorSelector = new ColorSelector(colorationModel.ColorSelectorModel);
        }


        public Guid ID { get; set; }
        public ColorSelector ColorSelector { get; set; }
        public BeatModifier BeatModifier { get; set; }


        public void SetViewModel(IModel model)
        {
            ColorationModel colorationModel = model as ColorationModel;
            colorationModel.ID = this.ID;
            this.ColorSelector.SetViewModel(colorationModel.ColorSelectorModel);
            //this.BeatModifier.SetViewModel(colorationModel.BeatModifierModel);
        }

        public IModel GetModel()
        {
            ColorationModel model = new ColorationModel();
            model.ID = this.ID;
            model.ColorSelectorModel = (ColorSelectorModel)this.ColorSelector.GetModel();
            //colorationModel.BeatModifierModel = (BeatModifierModel)this.BeatModifier.GetModel();
            return model;
        }
    }
}
