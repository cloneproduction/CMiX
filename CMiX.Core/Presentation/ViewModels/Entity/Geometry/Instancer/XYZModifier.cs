// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Observer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class XYZModifier : ObservableObject, IControl, IModifier, ISubject, IObserver
    {
        public XYZModifier(string name, Vector3D vector3D, MasterBeat beat, XYZModifierModel xYZModifierModel)
        {
            this.ID = xYZModifierModel.ID;
            Name = name;
            Observers = new List<IObserver>();

            //X = new AnimParameter(nameof(X), this, vector3D.X, beat);
            //Y = new AnimParameter(nameof(Y), this, vector3D.Y, beat);
            //Z = new AnimParameter(nameof(Z), this, vector3D.Z, beat);

            Attach(X);
            Attach(Y);
            Attach(Z);
        }


        private List<IObserver> Observers { get; set; }

        public void Attach(IObserver observer)
        {
            Observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            Observers.Remove(observer);
        }

        public void Notify(int count)
        {
            foreach (var observer in Observers)
            {
                observer.Update(count);
            }
        }

        public void Update(int count)
        {
            foreach (var observer in Observers)
            {
                observer.Update(count);
            }
        }

        public void SetViewModel(IModel model)
        {
            XYZModifierModel XYZModifierModel = model as XYZModifierModel;
            this.ID = XYZModifierModel.ID;
            this.Name = XYZModifierModel.Name;
            this.X.SetViewModel(XYZModifierModel.X);
            this.Y.SetViewModel(XYZModifierModel.Y);
            this.Z.SetViewModel(XYZModifierModel.Z);
        }

        public IModel GetModel()
        {
            XYZModifierModel model = new XYZModifierModel();
            model.ID = this.ID;
            model.Name = this.Name;
            model.X = (AnimParameterModel)this.X.GetModel();
            model.Y = (AnimParameterModel)this.Y.GetModel();
            model.Z = (AnimParameterModel)this.Z.GetModel();
            return model;
        }


        private int _isExpanded;
        public int IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private int _isExpandedX;
        public int IsExpandedX
        {
            get => _isExpandedX;
            set => SetProperty(ref _isExpandedX, value);
        }

        private int _isExpandedY;
        public int IsExpandedY
        {
            get => _isExpandedY;
            set => SetProperty(ref _isExpandedY, value);
        }

        private int _isExpandedZ;
        public int IsExpandedZ
        {
            get => _isExpandedZ;
            set => SetProperty(ref _isExpandedZ, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public AnimParameter X { get; set; }
        public AnimParameter Y { get; set; }
        public AnimParameter Z { get; set; }
        public Guid ID { get; set; }
    }
}
