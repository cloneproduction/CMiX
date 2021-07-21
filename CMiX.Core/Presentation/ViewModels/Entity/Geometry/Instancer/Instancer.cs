// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CMiX.Core.Models;
using CMiX.Core.Network.Communicators;
using CMiX.Core.Network.Messages;
using CMiX.Core.Presentation.ViewModels.Beat;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Presentation.ViewModels
{
    public class Instancer : ObservableObject, IControl
    {
        public Instancer(MasterBeat beat, InstancerModel instancerModel)
        {
            this.ID = instancerModel.ID;

            Factory = new TransformModifierFactory(beat);
            Transform = new Transform(instancerModel.Transform);

            NoAspectRatio = false;
            TransformModifiers = new ObservableCollection<ITransformModifier>();
            Factory = new TransformModifierFactory(beat);
            CreateTransformModifierCommand = new RelayCommand<TransformModifierNames>(CreateTransformModifier);
            RemoveTransformModifierCommand = new RelayCommand<ITransformModifier>(RemoveTransformModifier);
        }


        public void SetCommunicator(Communicator communicator)
        {
            Communicator = new Communicator(this);
            Communicator.SetCommunicator(communicator);
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            Communicator.UnsetCommunicator(communicator);
        }


        public Guid ID { get; set; }
        public Communicator Communicator { get; set; }
        public TransformModifierFactory Factory { get; set; }
        public Transform Transform { get; set; }
        public ICommand CreateTransformModifierCommand { get; set; }
        public ICommand AddTransformModifierCommand { get; set; }
        public ICommand RemoveTransformModifierCommand { get; set; }


        private bool _noAspectRatio;
        public bool NoAspectRatio
        {
            get => _noAspectRatio;
            set => SetProperty(ref _noAspectRatio, value);
        }

        private ObservableCollection<ITransformModifier> _transformModifiers;
        public ObservableCollection<ITransformModifier> TransformModifiers
        {
            get => _transformModifiers;
            set => SetProperty(ref _transformModifiers, value);
        }


        public void CreateTransformModifier(TransformModifierNames transformModifierNames)
        {
            ITransformModifier transformModifier = Factory.CreateTransformModifier(transformModifierNames);
            AddTransformModifier(transformModifier);
            //return transformModifier;
        }

        public void CreateTransformModifier(ITransformModifierModel transformModifierModel)
        {
            ITransformModifier transformModifier = Factory.CreateTransformModifier(transformModifierModel);
            AddTransformModifier(transformModifier);
            //return transformModifier;
        }


        public void AddTransformModifier(ITransformModifier transformModifier)
        {
            TransformModifiers.Add(transformModifier);
            Communicator?.SendMessage(new MessageAddTransformModifier(transformModifier.GetModel() as ITransformModifierModel));
        }

        public void RemoveTransformModifier(ITransformModifier transformModifier)
        {
            this.TransformModifiers.Remove(transformModifier);
        }


        public void SetViewModel(IModel model)
        {
            InstancerModel instancerModel = model as InstancerModel;
            this.ID = instancerModel.ID;
            this.Transform.SetViewModel(instancerModel.Transform);
            this.NoAspectRatio = instancerModel.NoAspectRatio;
        }

        public IModel GetModel()
        {
            InstancerModel model = new InstancerModel();
            model.ID = this.ID;
            model.Transform = (TransformModel)this.Transform.GetModel();
            model.NoAspectRatio = this.NoAspectRatio;
            return model;
        }
    }
}
