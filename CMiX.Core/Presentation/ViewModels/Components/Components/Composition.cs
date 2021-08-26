// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class Composition : Component, IRecipient<MessageMasterBeatChange>
    {
        public Composition(CompositionModel compositionModel)
        {
            ID = compositionModel.ID;
            Transition = new Slider(nameof(Transition), compositionModel.TransitionModel);
            //Camera = new Camera(MasterBeat, compositionModel.CameraModel);
            //Visibility = new Visibility(project.Visibility, compositionModel.VisibilityModel);
            SelectedBeatChangedCommand = new RelayCommand(SelectedBeatChanged);
        }

        public ICommand SelectedBeatChangedCommand { get; set; }



        public Camera Camera { get; set; }
        public Slider Transition { get; set; }

        public void SelectedBeatChanged()
        {
            this.UpdateChildMasterBeat(MasterBeat);
        }

        public void Receive(MessageMasterBeatChange message)
        {
            if (MasterBeat == null)
                return;

            if (MasterBeat.Equals(message.MasterBeat))
            {
                MasterBeat = message.MasterBeat;
                this.UpdateChildMasterBeat(MasterBeat);
            }
        }


        public override IComponentModel GetModel()
        {
            CompositionModel model = new CompositionModel(this.ID);

            model.Name = this.Name;
            //model.IsVisible = this.IsVisible;
            //model.MasterBeatModel = (MasterBeatModel)this.MasterBeat.GetModel();
            //model.CameraModel = (CameraModel)this.Camera.GetModel();
            model.TransitionModel = (SliderModel)this.Transition.GetModel();

            foreach (Component item in this.Components)
                model.ComponentModels.Add(item.GetModel());

            return model;
        }

        public override void SetViewModel(IComponentModel model)
        {
            CompositionModel compositionModel = model as CompositionModel;

            //this.MasterBeat.SetViewModel(compositionModel.MasterBeatModel);
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
