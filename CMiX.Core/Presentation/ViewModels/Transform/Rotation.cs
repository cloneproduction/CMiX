// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Rotation : ObservableObject, IControl
    {
        public Rotation(string name, RotationModel rotationModel)
        {
            this.ID = rotationModel.ID;
            X = new Slider(nameof(X), rotationModel.X);
            Y = new Slider(nameof(Y), rotationModel.Y);
            Z = new Slider(nameof(Z), rotationModel.Z);
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }
        public Slider X { get; set; }
        public Slider Y { get; set; }
        public Slider Z { get; set; }


        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);

            X.SetCommunicator(Communicator);
            Y.SetCommunicator(Communicator);
            Z.SetCommunicator(Communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);

            X.UnsetCommunicator(Communicator);
            Y.UnsetCommunicator(Communicator);
            Z.UnsetCommunicator(Communicator);
        }


        public IModel GetModel()
        {
            RotationModel model = new RotationModel();
            model.ID = this.ID;
            model.X = (SliderModel)this.X.GetModel();
            model.Y = (SliderModel)this.Y.GetModel();
            model.Z = (SliderModel)this.Z.GetModel();
            return model;
        }

        public void SetViewModel(IModel model)
        {
            RotationModel rotationModel = model as RotationModel;
            this.ID = rotationModel.ID;
            this.X.SetViewModel(rotationModel.X);
            this.Y.SetViewModel(rotationModel.Y);
            this.Z.SetViewModel(rotationModel.Z);
        }
    }
}
