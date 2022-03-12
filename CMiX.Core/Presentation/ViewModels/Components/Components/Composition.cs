// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using CMiX.Core.Models;
using CMiX.Core.Models.Beat;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Network;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Composition : Component, IBeatable
    {
        public Composition(CompositionModel compositionModel)
        {
            ID = compositionModel.ID;
            Transition = new Slider(nameof(Transition), compositionModel.TransitionModel);
            MasterBeat = new MasterBeat(new MasterBeatModel());

            //Camera = new Camera(MasterBeat, compositionModel.CameraModel);
            //Visibility = new Visibility(project.Visibility, compositionModel.VisibilityModel);
        }


        public Camera Camera { get; set; }
        public Slider Transition { get; set; }


        public override IComponentModel GetModel()
        {
            CompositionModel model = new CompositionModel(this.ID);

            model.Name = this.Name;
            model.ID = this.ID;
            model.MasterBeatModel = (MasterBeatModel)this.MasterBeat.GetModel();
            //model.IsVisible = this.IsVisible;
            //model.CameraModel = (CameraModel)this.Camera.GetModel();
            model.TransitionModel = (SliderModel)this.Transition.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel());

            return model;
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            this.MasterBeat = masterBeat;
            IEnumerable beatables = this.Components.Select(x => x.GetType() == typeof(IBeatable));

            foreach (IBeatable beatable in beatables)
                beatable.SetMasterBeat(this.MasterBeat);
        }

        public override void SetViewModel(IComponentModel model)
        {
            CompositionModel compositionModel = model as CompositionModel;
            this.ID = compositionModel.ID;
            this.MasterBeat.SetViewModel(compositionModel.MasterBeatModel);

            //this.Camera.SetViewModel(compositionModel.CameraModel);
            this.Transition.SetViewModel(compositionModel.TransitionModel);

            this.Components.Clear();
            foreach (var componentModel in compositionModel.ComponentModels)
            {
                //var newComponent = this.ComponentFactory.CreateComponent(compositionModel);
                //this.AddComponent(newComponent);
            }
        }
    }
}
