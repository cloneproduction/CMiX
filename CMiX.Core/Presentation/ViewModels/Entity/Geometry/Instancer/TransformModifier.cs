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
    public class TransformModifier : ObservableObject, IControl
    {
        public TransformModifier(MasterBeat beat)
        {
            //this.ID = instancerModel.ID;
            TransformModifiers = new ObservableCollection<ITransformModifier>();
            Factory = new TransformModifierFactory(beat);

            CreateTransformModifierCommand = new RelayCommand<TransformModifierNames>(CreateTransformModifier);
            RemoveTransformModifierCommand = new RelayCommand<ITransformModifier>(RemoveTransformModifier);
        }


        public ICommand CreateTransformModifierCommand { get; set; }
        public ICommand AddTransformModifierCommand { get; set; }
        public ICommand RemoveTransformModifierCommand { get; set; }
        public TransformModifierFactory Factory { get; set; }


        private ObservableCollection<ITransformModifier> _transformModifiers;
        public ObservableCollection<ITransformModifier> TransformModifiers
        {
            get => _transformModifiers;
            set => SetProperty(ref _transformModifiers, value);
        }
        public Communicator Communicator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Guid ID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CreateTransformModifier(TransformModifierNames transformModifierNames)
        {
            ITransformModifier transformModifier = Factory.CreateTransformModifier(transformModifierNames);
            transformModifier.SetCommunicator(Communicator);
            AddTransformModifier(transformModifier);
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

        public void SetCommunicator(Communicator communicator)
        {
            throw new NotImplementedException();
        }

        public void UnsetCommunicator(Communicator communicator)
        {
            throw new NotImplementedException();
        }


        public IModel GetModel()
        {
            throw new NotImplementedException();
        }

        public void SetViewModel(IModel model)
        {
            throw new NotImplementedException();
        }
    }
}
